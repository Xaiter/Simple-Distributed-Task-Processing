using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedTaskProcessing;
using System.Reflection;
using System.Threading;
using DistributedTaskProcessing.Server;
using DistributedTaskProcessing.Client;
using System.IO;

namespace DistributedTaskProcessing.ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            args = new string[] { "server" };
            if (args == null || args.Length != 1)
                return;

            Thread serviceThread = null;

            string mode = args[0];
            if (mode.Equals("server", StringComparison.OrdinalIgnoreCase))
                serviceThread = new Thread(ServerMain);
            else if (mode.Equals("client", StringComparison.OrdinalIgnoreCase))
                serviceThread = new Thread(ClientMain);

            serviceThread.Start();

            bool done = false;
            Console.WriteLine("Press [Esc] to exit, Press [Backspace] to clear");
            
            while (!done)
            {
                var key = Console.ReadKey(true).Key;

                switch(key)
                {
                    case ConsoleKey.Backspace:
                        Console.Clear();
                        Console.WriteLine("Press [Esc] to exit, Press [Backspace] to clear");
                        break;

                    case ConsoleKey.Escape:
                        done = true;
                        break;
                }

                Thread.Sleep(1);
            }

            serviceThread.Abort();
        }

        static void ServerMain()
        {
            var serverService = new TaskServerService();
            serverService.OpenHost();
            
            var type = Type.GetType("MockObjects.MockProgram, MockObjects");
            var program = (ITaskProgram)Activator.CreateInstance(type);
            serverService.DoWork(program);

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
