using DistributedTaskProcessing.Client;
using DistributedTaskProcessing.Client.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedTaskProcessing.Client
{
    public class TaskClientService : IDisposable
    {
        // Fields
        private ServiceHost _serviceHost = null;
        private TaskClient _serverInstance = null;



        // Public Methods
        public void OpenHost()
        {
            CloseHost();

            _serverInstance = new TaskClient();
            _serviceHost = WcfUtilities.CreateServiceHost(Settings.Default.TcpAddress, typeof(ITaskClient), _serverInstance);
            _serviceHost.Open();
            Logger.Trace("Opened Task Client Service");

            while (_serverInstance.ClientId == null)
            {
                _serverInstance.ClientId = RegisterClient();
                Thread.Sleep(10000);
            }
        }

        public void CloseHost()
        {
            if (_serviceHost == null)
                return;

            UnregisterClient(_serverInstance.ClientId.Value);
            _serviceHost.Close();
            _serviceHost = null;
        }

        public string[] GetProgramAssemblyPaths(string programName)
        {
            return null; //todo
        }

        public static Guid? RegisterClient()
        {
            Logger.Trace("Registering client with server...");

            var proxy = WcfUtilities.GetServiceProxy<ITaskServer>(Settings.Default.ServerTcpAddress);
            var result = WcfUtilities.InvokeWcfProxyMethod((Func<string, string, Guid>)proxy.RegisterClient, Settings.Default.ClientName, Settings.Default.TcpAddress);

            if (!result.Success)
            {
                Logger.Trace("Failed to register with server at " + Settings.Default.ServerTcpAddress);
                return null;
            }

            var value = (Guid)result.ReturnValue;
            Logger.Trace("Registered! Assigned Client Id " + value.ToString());
            return value;
        }

        public static bool UnregisterClient(Guid clientId)
        {
            Logger.Trace("Unregisting Client Id " + clientId.ToString());

            var proxy = WcfUtilities.GetServiceProxy<ITaskServer>(Settings.Default.ServerTcpAddress);
            var result = WcfUtilities.InvokeWcfProxyMethod((Action<Guid>)proxy.UnregisterClient, clientId);

            if (result.Success)
                Logger.Trace("Unregistered client with server!");
            else
                Logger.Trace("Failed to unregister client with server!");

            return result.Success;
        }

        public static bool WorkItemComplete(Guid clientId, Guid workItemId, object returnValue)
        {
            Logger.Trace("Reporting assigned work for client Id " + clientId.ToString() + " is complete");

            var proxy = WcfUtilities.GetServiceProxy<ITaskServer>(Settings.Default.ServerTcpAddress);
            var result = WcfUtilities.InvokeWcfProxyMethod((Action<Guid, Guid, object>)proxy.WorkItemComplete, clientId, workItemId, returnValue);

            if (result.Success)
                Logger.Trace("Reported work item complete!");
            else
                Logger.Trace("Failed to report work item complete!");

            return result.Success;
        }



        // Service Host Thing
        void IDisposable.Dispose()
        {
            _serviceHost = null;
            _serverInstance = null;
        }
    }
}
