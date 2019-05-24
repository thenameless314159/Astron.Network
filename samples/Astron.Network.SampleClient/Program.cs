using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Astron.Network.SampleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("Usage : on connections end, enter y if you want to continue.");
            do
            {
                try
                {
                    var sw = Stopwatch.StartNew();
                    Console.WriteLine("Please enter the number of connection to make :");
                    var connectionCount = int.Parse(Console.ReadLine());
                    Console.WriteLine($"Attempting to start {connectionCount} connections...");
                    Console.WriteLine("Press any key to start the connections...");
                    Console.ReadLine();
                    var connections = new List<Task<RemoteNetworkConnection>>(connectionCount);
                    for (var i = 0; i < connectionCount; i++)
                        connections.Add(RemoteNetworkConnection.Connect());

                    Parallel.ForEach(connections,
                        async connect => { await connect.ContinueWith(async t => await t.Result.Setup()); });
                }
                catch
                {

                }
            } while (Console.ReadLine() == "y");
        }
    }
}
