using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Astron.Network.SampleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
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

                    await Task.WhenAll(connections.Select(async c => await Task.Run(async () =>
                    {
                        var conn = await c;
                        await conn.Setup();
                    })));

                    Console.WriteLine("Continue ? : y / n");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            } while (Console.ReadLine() == "y");
        }
    }
}
