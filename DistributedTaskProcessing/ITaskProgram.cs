using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedTaskProcessing
{
    public static class TaskClientMessenger
    {
        public static void SendWork(ClientInformation clientInfo, ITaskProgram program)
        {

        }

        public static void SendProgram(ClientInformation clientInfo, ITaskProgram program)
        {

        }
    }

    /// <summary>
    /// A program to be loaded by a TaskServer (probably through a user interface).
    /// </summary>
    public interface ITaskProgram
    {
        string Name { get; set; }
        Assembly[] GetAssemblies();
        WorkItemMessage[] GetWorkItemMessages();
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
                    TaskClientMessenger.SendWork(client, program);
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
    }

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
    /// Provides functionality for a client to receive work from a server.    
    /// </summary>
    public interface ITaskClient
    {
        bool HasProgram(string programName);
        void ReceiveProgram(Stream message);
        void ExecuteWorkItem(Stream message);
        bool IsAlive();
    }


    public class ClientInformation
    {
        // Properties
        public Guid ClientId { get; set; }
        public string EndpointLocation { get; set; }
        public DateTime LastMessageTime { get; set; }
        public bool IsAlive { get; set; }
        public bool IsBusy { get; set; }
        public WorkItemMessage CurrentWorkItem { get; set; }
    }

    public class WorkItemMessage
    {
        public Guid WorkItemId { get; set; }
        public byte[] WorkData { get; set; }
        public string WorkDataType { get; set; }

        public string ProgramName { get; set; }
        public string WorkerType { get; set; }
        public string WorkerMethodName { get; set; }
    }

    public class ProgramMessage
    {
        public string Name { get; set; }
        public FileData[] Assemblies { get; set; }
    }

    public class FileData
    {
        public string Filename { get; set; }
        public byte[] Data { get; set; }
    }
}