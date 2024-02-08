using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Formatting = Newtonsoft.Json.Formatting;

namespace QsysDiscovery
{
    public static class QsysDiscoveryProtocol
    {
        public static async Task<JToken[]> DiscoverAsync(int timeoutInMilliseconds = 3000)
        {
            var multicastAddress = IPAddress.Parse("224.0.23.175");
            const int startPort = 2467;
            const int endPort = 2470;

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(timeoutInMilliseconds);

            var tasks = new Task[endPort - startPort + 1];
            var data = new List<JToken>();
            var deviceData = new Dictionary<string, JToken>();

            for (var port = startPort; port <= endPort; port++)
            {
                var localPort = port; // Local copy for the closure below
                tasks[port - startPort] = Task.Run(async () =>
                {
                    var result = await ListenOnPortAsync(multicastAddress, localPort, cancellationTokenSource.Token);
                    data.AddRange(result);
                }, cancellationTokenSource.Token);
            }

            await Task.WhenAll(tasks);

            foreach (var jToken in data)
            {
                if (!(jToken["QDP"] is JObject jObject)) continue;
                foreach (var property in jObject.Properties())
                    switch (property.Name)
                    {
                        case "device":
                            var deviceRef = property.Value["ref"]?.Value<string>();
                            if (deviceRef != null)
                                // Console.WriteLine($"Found {property.Name} with ref: {deviceRef}");
                                deviceData[deviceRef] = property.Value;
                            // Console.WriteLine($"Found {property.Name} with ref: {property.Value}");
                            break;
                    }
            }

            return deviceData.Values.ToArray();
        }

        private static async Task<JToken[]> ListenOnPortAsync(IPAddress multicastAddress, int port,
            CancellationToken cancellationToken = default)
        {
            var data = new List<JToken>();
            using (var udpClient = new UdpClient())
            {
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpClient.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, true);
                udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, port));
                udpClient.JoinMulticastGroup(multicastAddress);

                //Console.WriteLine($"Listening for multicast UDP packets on {multicastAddress}:{port}...");

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var receiveTask = udpClient.ReceiveAsync();
                        var delayTask =
                            Task.Delay(60000,
                                cancellationToken); // Wait for 5 seconds or until cancellation is requested

                        var completedTask = await Task.WhenAny(receiveTask, delayTask);

                        if (completedTask == receiveTask && !cancellationToken.IsCancellationRequested)
                        {
                            var receivedResult = await receiveTask; // Receive the data
                            var receivedData = Encoding.ASCII.GetString(receivedResult.Buffer);

                            var xmlDoc = new XmlDocument();
                            try
                            {
                                xmlDoc.LoadXml(receivedData);
                                var json = JsonConvert.SerializeXmlNode(xmlDoc, Formatting.Indented);
                                var jToken = JToken.Parse(json);
                                data.Add(jToken);
                            }
                            catch (XmlException xmlEx)
                            {
                                Console.WriteLine($"XML Parsing Error: {xmlEx.Message}");
                            }
                        }
                        else
                        {
                            // Timeout or cancellation requested
                            break;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine($"Listening on port {port} cancelled due to timeout.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    udpClient.DropMulticastGroup(multicastAddress);
                    //Console.WriteLine($"Stopped listening on port {port}.");
                }
            }

            return data.ToArray();
        }
    }
}