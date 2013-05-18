using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DistributedTaskProcessing
{
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
        public List<ClientInformation> Clients { get; set; }

        public void DoWork(ITaskProgram program)
        {
            var workItems = new Queue<WorkItemMessage>(program.GetWorkItemMessages());


        }

        public void RegisterClient(string endpointUrl)
        {
            var clientInfo = new ClientInformation();
            clientInfo.EndpointLocation = endpointUrl;
            clientInfo.LastMessage = DateTime.Now;

        }

        public void ReceiveWorkItemUpdate()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Providers functionality to add reigster workers
    /// and receive work progress updates.
    /// </summary>
    public interface ITaskServer
    {
        void RegisterClient(string endpointUrl);
        void ReceiveWorkItemUpdate();
    }

    /// <summary>
    /// Provides functionality for a client to receive work from a server.    
    /// </summary>
    public interface ITaskClient
    {
        bool HasProgram();
        void ReceiveProgram(Stream message);
        void ExecuteWorkItem(Stream message);
        bool IsAlive();
    }

    public class ClientInformation
    {
        // Properties
        public string EndpointLocation { get; set; }
        public DateTime LastMessage { get; set; }
        public bool IsAlive { get; set; }
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