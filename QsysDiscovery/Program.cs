using System;
using System.Threading.Tasks;
using Formatting = Newtonsoft.Json.Formatting;

namespace QsysDiscovery
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var results = await QsysDiscoveryProtocol.DiscoverAsync();
            foreach (var result in results)
                Console.WriteLine("Found device:\r\n" + result.ToString(Formatting.Indented));
        }
    }
}