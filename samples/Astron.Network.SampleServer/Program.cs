using System;
using System.Threading.Tasks;

using Astron.Network.SampleServer.Sockets;

namespace Astron.Network.SampleServer
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var socketServer = new NetworkServer(100000, 100000);
                await socketServer.Setup();
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
            }

            Console.WriteLine();
            Console.WriteLine("-- press any key to exit");
            Console.ReadLine();
        }
    }
}
