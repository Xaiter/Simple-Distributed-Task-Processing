using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedTaskProcessing;
using System.Reflection;
using System.Threading;
using MockObjects;

namespace DistributedTaskProcessing.ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var serverThread = new Thread(ServerMain);
            serverThread.Start();
            
            var clientThread = new Thread(ClientMain);
            clientThread.Start();
        }

        static void ServerMain()
        {
            var serverService = new TaskServerService();
            serverService.OpenHost();

            serverService.DoWork(new MockProgram());

            while (true)
                Thread.Sleep(1);
        }


        static void ClientMain()
        {
            var clientService = new TaskClientService();
            clientService.OpenHost();

            while (true)
                Thread.Sleep(1);
        }
    }



}
