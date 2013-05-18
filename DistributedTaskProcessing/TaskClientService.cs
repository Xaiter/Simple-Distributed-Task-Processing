using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DistributedTaskProcessing
{
    public static class TaskClientService
    {
        // Fields
        private static ServiceHost _serviceHost = null;
        private static TaskClient _serverInstance = null;


        static TaskClientService()
        {

        }


        // Public Methods
        public static void OpenHost()
        {
            if (_serviceHost != null)
                _serviceHost.Close();

            _serverInstance = new TaskClient();
            _serviceHost = new ServiceHost(_serverInstance, new Uri("net.tcp://localhost:95/TaskClient"));
            _serviceHost.Open();
        }

        public static void CloseHost()
        {
            if (_serverInstance == null)
                return;

            _serviceHost.Close();
            _serviceHost = null;
            _serverInstance = null;
        }

        public static string[] GetProgramAssemblyPaths(string programName)
        {
            return null; //todo
        }
    }
}
