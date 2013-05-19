using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DistributedTaskProcessing
{
    public class TaskClientService
    {
        // Fields
        private ServiceHost _serviceHost = null;
        private string _localTaskClientServiceUri = "net.tcp://localhost:95/TaskClient";
        private string _remoteTaskServiceUri = "net.tcp://localhost:95/TaskServer";
        private Guid? _clientId = null;


        // Public Methods
        public void OpenHost()
        {
            if (_serviceHost != null)
                _serviceHost.Close();

            RegisterClient();
            _serviceHost = new ServiceHost(typeof(TaskClient), new Uri(_localTaskClientServiceUri));
            _serviceHost.Open();
        }

        public void CloseHost()
        {
            if (_serviceHost == null)
                return;

            _serviceHost.Close();
            _serviceHost = null;
        }

        public string[] GetProgramAssemblyPaths(string programName)
        {
            return null; //todo
        }


        // Private Methods
        private void RegisterClient()
        {
            Logger.Trace("Registering client with server...");

            var proxy = WcfUtilities.GetServiceProxy<ITaskServer>(_remoteTaskServiceUri);
            _clientId = proxy.RegisterClient(_localTaskClientServiceUri);

            Logger.Trace("Registered! Assigned Client Id " + _clientId.ToString());
        }
    }
}
