using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedTaskProcessing
{
    /// <summary>
    /// Providers functionality to add reigster workers
    /// and receive work progress updates.
    /// </summary>
    public interface ITaskServer
    {
        Guid RegisterClient(string endpointUrl);
        void WorkItemComplete(Guid clientId);
        void UnregisterClient(Guid clientId);
    }

    /// <summary>
    /// Accepts work from ITaskPrograms and distributes it to ITaskClients.
    /// </summary>
    public class TaskServer : ITaskServer
    {
        // Properties
        private Queue<WorkItemMessage> _workItems = null;
        private List<WorkItemMessage> _workItemsInProgress = new List<WorkItemMessage>();
        private List<ClientInformation> _clients = new List<ClientInformation>();
        private bool _clientUpdateReceived = false;



        // Public Methods
        public void DoWork(ITaskProgram program)
        {
            _workItems = new Queue<WorkItemMessage>(program.GetWorkItemMessages());

            WaitForClientUpdate(5000);

            while (_workItems.Count > 0 && _workItemsInProgress.Count > 0)
            {
                var clients = GetAvailableClients();
                foreach (var client in clients)
                {
                    client.CurrentWorkItem = _workItems.Dequeue();
                    _workItemsInProgress.Add(client.CurrentWorkItem);
                    TaskServer.SendWork(client, program);
                }

                WaitForClientUpdate(100);
            }
        }

        public Guid RegisterClient(string endpointUrl)
        {
            var clientInfo = new ClientInformation();
            clientInfo.EndpointLocation = endpointUrl;
            clientInfo.LastMessageTime = DateTime.Now;
            clientInfo.ClientId = Guid.NewGuid();

            this._clients.Add(clientInfo);

            return clientInfo.ClientId;
        }

        public void WorkItemComplete(Guid clientId)
        {
            var client = GetClientById(clientId);
            client.IsBusy = false;
            _workItemsInProgress.Remove(client.CurrentWorkItem);

            _clientUpdateReceived = true;
        }

        public void UnregisterClient(Guid clientId)
        {
            var client = GetClientById(clientId);
            _clients.Remove(client);
        }



        // Private Methods
        private ClientInformation[] GetAvailableClients()
        {
            return (from client in _clients
                    where !client.IsBusy && client.IsAlive
                    select client).ToArray();
        }

        private ClientInformation GetClientById(Guid clientId)
        {
            return (from client in _clients
                    where client.ClientId == clientId
                    select client).FirstOrDefault();
        }

        private void WaitForClientUpdate(int sleepInterval)
        {
            while (!_clientUpdateReceived)
                Thread.Sleep(sleepInterval);

            _clientUpdateReceived = false;
        }



        // Static Methods
        private static void SendWork(ClientInformation clientInfo, ITaskProgram program)
        {
            var proxy = WcfUtilities.GetServiceProxy<ITaskClient>(clientInfo.EndpointLocation);

            if (!proxy.HasProgram(program.Name))
                SendProgram(proxy, clientInfo, program);

            SerializeMessageStream(proxy.ExecuteWorkItem, clientInfo.CurrentWorkItem);
        }

        private static void SendProgram(ClientInformation clientInfo, ITaskProgram program)
        {
            var proxy = WcfUtilities.GetServiceProxy<ITaskClient>(clientInfo.EndpointLocation);
            SendProgram(proxy, clientInfo, program);
        }

        private static void SendProgram(ITaskClient proxy, ClientInformation clientInfo, ITaskProgram program)
        {
            var programMessage = new ProgramMessage();
            programMessage.Name = program.Name;
            programMessage.ProgramFiles = program.GetProgramFiles();

            SerializeMessageStream(proxy.ReceiveProgram, programMessage);
        }

        private static void SerializeMessageStream(Action<Stream> handler, object message)
        {
            var messageBytes = DataUtilities.Serialize(message);
            messageBytes = DataUtilities.Compress(messageBytes);

            var memoryStream = new MemoryStream(messageBytes);
            handler.Invoke(memoryStream);
        }
    }




}
