using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedTaskProcessing.Server
{


    /// <summary>
    /// Accepts work from ITaskPrograms and distributes it to ITaskClients.
    /// </summary>
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
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

            while (_clients.Count == 0)
            {
                Logger.Trace("Waiting for clients...");
                WaitForClientUpdate();
            }

            while (_workItems.Count > 0 || _workItemsInProgress.Count > 0)
            {
                var clients = GetAvailableClients();
                Logger.Trace("Found " + clients.Length + " clients...");
                foreach (var client in clients)
                {                    
                    client.CurrentWorkItem = _workItems.Dequeue();
                    Logger.Trace("Sending work item " + client.CurrentWorkItem.WorkItemId + " to " + client.ClientId.ToString());
                    _workItemsInProgress.Add(client.CurrentWorkItem);
                    TaskServer.SendWork(client, program);
                }

                WaitForClientUpdate();
            }

            Logger.Trace("Work complete!");
        }

        public Guid RegisterClient(string endpointUrl)
        {
            var clientInfo = new ClientInformation();
            clientInfo.EndpointLocation = endpointUrl;
            clientInfo.LastMessageTime = DateTime.Now;
            clientInfo.ClientId = Guid.NewGuid();
            clientInfo.IsAlive = true;
            clientInfo.IsBusy = false;

            this._clients.Add(clientInfo);

            _clientUpdateReceived = true;

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

        private void WaitForClientUpdate()
        {
            Logger.Trace("Waiting for client update...");
            while (!_clientUpdateReceived)
                Thread.Sleep(1);

            _clientUpdateReceived = false;
            Logger.Trace("Client update received!");
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
            object testObject = DataUtilities.Deserialize(messageBytes, message.GetType());
            //messageBytes = DataUtilities.Compress(messageBytes);

            var memoryStream = new MemoryStream(messageBytes);
            handler.Invoke(memoryStream);
        }
    }




}
