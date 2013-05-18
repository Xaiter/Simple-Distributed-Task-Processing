using System;
using System.Collections.Generic;
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
        public List<ClientInformation> Clients { get; set; }
        public bool ClientUpdateReceived { get; private set; }


        // Public Methods
        public void DoWork(ITaskProgram program)
        {
            var workItems = new Queue<WorkItemMessage>(program.GetWorkItemMessages());
            var inProgress = new List<WorkItemMessage>();

            while (workItems.Count > 0 && inProgress.Count > 0)
            {
                var clients = GetAvailableClients();
                foreach (var client in clients)
                {
                    var workItem = workItems.Dequeue();
                    inProgress.Add(workItem);
                    client.CurrentWorkItem = workItem;
                    TaskServer.SendWork(client, program);
                }

                while (!ClientUpdateReceived)
                    Thread.Sleep(100);

                ClientUpdateReceived = false;
            }
        }

        public Guid RegisterClient(string endpointUrl)
        {
            var clientInfo = new ClientInformation();
            clientInfo.EndpointLocation = endpointUrl;
            clientInfo.LastMessageTime = DateTime.Now;
            clientInfo.ClientId = Guid.NewGuid();

            this.Clients.Add(clientInfo);

            return clientInfo.ClientId;
        }

        public void WorkItemComplete(Guid clientId)
        {
            var client = GetClientById(clientId);
            client.IsBusy = false;

            ClientUpdateReceived = true;
        }

        public void UnregisterClient(Guid clientId)
        {
            var client = GetClientById(clientId);
            Clients.Remove(client);
        }



        // Private Methods
        private ClientInformation[] GetAvailableClients()
        {
            return (from client in Clients
                    where !client.IsBusy && client.IsAlive
                    select client).ToArray();
        }

        private ClientInformation GetClientById(Guid clientId)
        {
            return (from client in Clients
                    where client.ClientId == clientId
                    select client).FirstOrDefault();
        }


        // Static Methods
        public static void SendWork(ClientInformation clientInfo, ITaskProgram program)
        {

        }

        public static void SendProgram(ClientInformation clientInfo, ITaskProgram program)
        {

        }
    }

    

   
}
