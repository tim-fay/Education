using System;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;

namespace CustomOrleansHost
{
    /// <summary>
    /// Orleans test silo host
    /// </summary>
    internal static class Program
    {
        private static void Main()
        {
            // First, configure and start a local silo
            var siloConfig = ClusterConfiguration.LocalhostPrimarySilo();
            var silo = new SiloHost("TestSilo", siloConfig);
            silo.InitializeOrleansSilo();
            silo.StartOrleansSilo();

            Console.WriteLine("Silo started.");

            // Then configure and connect a client.
            //var clientConfig = ClientConfiguration.LocalhostSilo();
            //var client = new ClientBuilder().UseConfiguration(clientConfig).Build();
            //client.Connect().Wait();

            //Console.WriteLine("Client connected.");

            //
            // This is the place for your test code.
            //

            Console.WriteLine("\nPress Enter to terminate...");
            Console.ReadLine();

            // Shut down
            //client.Close();
            silo.ShutdownOrleansSilo();
        }
    }
}
