using System;
using System.Linq;
using System.Threading.Tasks;

namespace QsysDiscovery
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var results = await QsysDiscoveryProtocol.DiscoverAsync();
            var table = new ConsoleTable("Name", "Platform", "Product", "IP Address", "LLDP", "Design Name");
            foreach (var result in results.OrderByDescending(d => d.ControlInfo?.DesignName ?? string.Empty)
                         .ThenBy(d => d.PartNumber)
                         .ThenBy(d => StringHelper.IpAddressLabel(d.IpAddress)))
                table.AddRow(result.Name, result.Platform, result.PartNumber, result.IpAddress, result.LldpInfo,
                    result.ControlInfo?.DesignName ?? string.Empty);
            Console.WriteLine();
            Console.WriteLine($"Found {table.Count} devices:");
            Console.WriteLine();
            Console.WriteLine(table.ToString(true));
            /*using (var file = File.CreateText("QsysDiscovery.json"))
            {
                await file.WriteAsync(JsonConvert.SerializeObject(results, Formatting.Indented));
            }

            Console.WriteLine($"Found {results.Length} broadcasts. Wrote to QsysDiscovery.json.");
            */
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    public static class StringHelper
    {
        public static string IpAddressLabel(string ipAddress)
        {
            return string.Join(".", ipAddress.Split('.').Select(part => part.PadLeft(3, '0')));
        }
    }
}