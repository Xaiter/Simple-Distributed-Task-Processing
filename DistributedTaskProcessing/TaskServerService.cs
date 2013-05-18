using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DistributedTaskProcessing
{
    public static class TaskServerService
    {
        // Fields
        private static ServiceHost _serviceHost = null;
        private static TaskServer _serverInstance = new TaskServer();


        // Public Methods
        public static void DoWork(ITaskProgram program)
        {
            _serverInstance.DoWork(program);
        }

        public static void OpenHost()
        {
            CloseHost();

            _serverInstance = new TaskServer();
            _serviceHost = new ServiceHost(_serverInstance, new Uri("net.tcp://localhost:95/TaskServer"));
            _serviceHost.Open();
        }

        public static void CloseHost()
        {
            if (_serviceHost == null)
                return;
            
            _serviceHost.Close();
            _serviceHost = null;
            _serverInstance = null;
        }
    }
}
