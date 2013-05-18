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


        static TaskClientService()
        {

        }


        // Public Methods
        public static void OpenHost()
        {
            if (_serviceHost != null)
                _serviceHost.Close();

            var address = new Uri("net.tcp://localhost:95/TaskClient");
            _serviceHost = new ServiceHost(typeof(TaskClient), address);
            _serviceHost.Open();
        }

        public static string[] GetProgramAssemblyPaths(string programName)
        {
            return null; //todo
        }
    }
}
