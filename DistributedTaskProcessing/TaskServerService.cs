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


        // Public Methods
        public static void OpenHost()
        {
            CloseHost();

            var address = new Uri("net.tcp://localhost:95/TaskServer");
            _serviceHost = new ServiceHost(typeof(TaskServer), address);
            _serviceHost.Open();
        }

        public static void CloseHost()
        {
            if (_serviceHost == null)
                return;

            _serviceHost.Close();
            _serviceHost = null;
        }
    }
}
