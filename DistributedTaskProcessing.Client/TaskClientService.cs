using DistributedTaskProcessing.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DistributedTaskProcessing.Client
{
    public class TaskClientService
    {
        // Constant
        private const string LOCAL_TCP_ADDRESS = "net.tcp://localhost:95/";

        // Fields
        private ServiceHost _serviceHost = null;
        private string _localTaskClientServiceUri = LOCAL_TCP_ADDRESS + "TaskClient";
        private string _remoteTaskServiceUri = LOCAL_TCP_ADDRESS + "TaskServer";
        private Guid? _clientId = null;


        // Public Methods
        public void OpenHost()
        {
            CloseHost();

            _serviceHost = new ServiceHost(typeof(TaskClient), new Uri(LOCAL_TCP_ADDRESS));
            _serviceHost.AddServiceEndpoint(typeof(ITaskClient), WcfUtilities.GetTcpBinding(), "TaskClient");
            _serviceHost.Open();
            Logger.Trace("Opened Task Client Service - " + _localTaskClientServiceUri);

            RegisterClient();
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
