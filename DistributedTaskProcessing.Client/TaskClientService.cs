using DistributedTaskProcessing.Client;
using DistributedTaskProcessing.Client.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace DistributedTaskProcessing.Client
{
    public class TaskClientService
    {
        // Constant
        private const string LOCAL_TCP_ADDRESS = "net.tcp://localhost:95/";

        // Fields
        private ServiceEndpoint _currentEndpoint = null;
        private ServiceHost _serviceHost = null;
        private Guid? _clientId = null;


        // Public Methods
        public void OpenHost()
        {
            CloseHost();


            _currentEndpoint = WcfUtilities.CreateServiceEndpoint(Settings.Default.TcpAddress, typeof(TaskClient));
            _serviceHost = new ServiceHost(typeof(TaskClient), new Uri(Settings.Default.TcpAddress));
            _serviceHost.AddServiceEndpoint(_currentEndpoint);
            _serviceHost.Open();
            Logger.Trace("Opened Task Client Service");

            RegisterClient();
        }



        public void CloseHost()
        {
            if (_serviceHost == null)
                return;

            _serviceHost.Close();
            _serviceHost = null;
            _currentEndpoint = null;
        }

        public string[] GetProgramAssemblyPaths(string programName)
        {
            return null; //todo
        }


        // Private Methods
        private void RegisterClient()
        {
            Logger.Trace("Registering client with server...");


            var proxy = WcfUtilities.GetServiceProxy<ITaskServer>(Settings.Default.ServerTcpAddress);
            var result = WcfUtilities.InvokeWcfProxyMethod((Func<string, Guid>)proxy.RegisterClient, _currentEndpoint.Address.Uri.ToString());

            if (!result.Success)
            {
                Logger.Trace("Failed to register with server at " + Settings.Default.ServerTcpAddress);
                return;
            }

            _clientId = (Guid)result.ReturnValue;

            Logger.Trace("Registered! Assigned Client Id " + _clientId.Value.ToString());
        }

        private bool UnregisterClient()
        {
            Logger.Trace("Unregisting Client Id " + _clientId.ToString());

            var proxy = WcfUtilities.GetServiceProxy<ITaskServer>(Settings.Default.ServerTcpAddress);
            var result = WcfUtilities.InvokeWcfProxyMethod((Action<Guid>)proxy.UnregisterClient, _clientId.Value);

            if(result.Success)
                Logger.Trace("Unregistered client with server!");
            else
                Logger.Trace("Failed to unregister client with server!");

            return result.Success;
        }
    }
}
