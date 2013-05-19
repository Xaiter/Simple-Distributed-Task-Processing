using DistributedTaskProcessing.Server.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DistributedTaskProcessing.Server
{
    public class TaskServerService
    {
        // Fields
        private ServiceHost _serviceHost = null;
        private TaskServer _serverInstance = null;


        // Public Methods
        public void DoWork(ITaskProgram program)
        {
            Logger.Trace("Starting work for program " + program.ToString());
            _serverInstance.DoWork(program);
        }

        public void OpenHost()
        {
            CloseHost();

            _serverInstance = new TaskServer();
            _serviceHost = new ServiceHost(_serverInstance, new Uri(Settings.Default.TcpAddress));
            _serviceHost.AddServiceEndpoint(typeof(ITaskServer), WcfUtilities.GetTcpBinding(), "Server");
            _serviceHost.Open();
            Logger.Trace("Opened Task Server Service");
        }

        public void CloseHost()
        {
            if (_serviceHost == null)
                return;
            
            _serviceHost.Close();
            _serviceHost = null;
            _serverInstance = null;
        }

    }
}
