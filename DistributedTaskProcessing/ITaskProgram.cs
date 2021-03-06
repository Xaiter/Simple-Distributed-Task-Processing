﻿using System;
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
    /// <summary>
    /// A program to be loaded by a TaskServer (probably through a user interface).
    /// </summary>
    public interface ITaskProgram
    {
        string Name { get; set; }
        FileData[] GetProgramFiles();
        WorkItemMessage[] GetWorkItemMessages();
        void OnWorkItemProcessed(WorkItemMessage message, object returnValue);
    }

    public class ClientInformation
    {
        // Properties
        public Guid ClientId { get; set; }
        public string Name { get; set; }
        public string EndpointLocation { get; set; }
        public DateTime LastMessageTime { get; set; }
        public bool IsAlive { get; set; }
        public bool IsBusy { get; set; }
        public WorkItemMessage CurrentWorkItem { get; set; }
    }

    [Serializable]
    public class WorkItemMessage
    {
        public Guid WorkItemId { get; set; }
        public byte[] WorkData { get; set; }
        public string WorkDataType { get; set; }

        public string ProgramName { get; set; }
        public string WorkerAssemblyName { get; set; }
        public string WorkerType { get; set; }

        public void SetWorkData(object data)
        {
            // yeah, I am totally not going to propogate generics
            // the friggin' way up tonight, nope.

            WorkData = DataUtilities.Serialize(data);
            WorkDataType = data.GetType().ToString();
        }

        public T GetWorkData<T>()
        {
            return DataUtilities.Deserialize<T>(WorkData);
        }
    }

    [Serializable]
    public class ProgramMessage
    {
        public string Name { get; set; }
        public FileData[] ProgramFiles { get; set; }
    }

    [Serializable]
    public class FileData
    {
        public string Filename { get; set; }
        public byte[] Data { get; set; }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public FileData()
        {

        }

        /// <summary>
        /// Loads a file from disk.
        /// </summary>
        /// <param name="filePath"></param>
        public FileData(string filePath)
        {
            Filename = Path.GetFileName(filePath);
            Data = File.ReadAllBytes(filePath);
        }
    }
}