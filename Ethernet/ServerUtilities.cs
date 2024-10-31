using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ethernet
{
    public class ServerUtilities
    {
        private static int lengthToReach = 0;

        public static void StartServer()
        {
            TcpListener server = null;
            try
            {
                int port = 5000;
                IPAddress localAddr = IPAddress.Any; // Accept connections from any IP address

                Console.WriteLine("Bitte geben Sie die zu erreichende Länge ein:");
                lengthToReach = Convert.ToInt32(Console.ReadLine());
                server = new TcpListener(localAddr, port);
                server.Start();
                string localIP = GetLocalIPAddress();
                Console.WriteLine($"The server IP address is: {localIP} and port is {port}");

                Console.WriteLine("Server gestartet. Warte auf Verbindungen...");

                var tasksDic = new Dictionary<string, Task>();

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine(lenghtToReach);

                    foreach (var task in tasksDic)
                    {
                        if (task.Value.Status == TaskStatus.Running)
                        {
                            Console.WriteLine($"running device ip: {task.Key}");
                        }
                    }
                    string clientIp = ((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                    // Alte abgeschlossene Tasks entfernen
                    tasksDic = tasksDic.Where(t => t.Value.Status == TaskStatus.Running)
                                       .ToDictionary(t => t.Key, t => t.Value);

                    if (!tasksDic.ContainsKey(clientIp))
                    {
                        var task = Task.Run(() => HandleClient(client, lengthToReach));
                        tasksDic[clientIp] = task;
                    }

                    if (tasksDic.Values.All(t => t.IsCompleted))
                    {
                        Console.WriteLine("Bitte geben Sie die zu erreichende Länge ein:");
                        lengthToReach = Convert.ToInt32(Console.ReadLine());
                        tasksDic.Clear();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
        }

        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                // Return the first IPv4 address that is not a loopback address
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private static bool HandleClient(TcpClient client, int lengthToReach)
        {
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

            bool reached = false;

            try
            {
                while (!reached)
                {
                    string data = reader.ReadLine();
                    if (data == null) break;

                    Console.WriteLine($"Empfangen: {data}");

                    int counter;
                    if (int.TryParse(data, out counter))
                    {
                        Console.WriteLine($"Empfangener Zählerwert: {counter}");

                        const double angleDistance = 5.1;
                        double calculatedDistance = counter * angleDistance;

                        if (calculatedDistance <= lengthToReach - angleDistance)
                        {
                            Console.WriteLine("no break");
                            response = 1;
                        }
                        else if (calculatedDistance >= lengthToReach + angleDistance)
                        {
                            Console.WriteLine("no break");
                            response = 2;
                        }
                        else
                        {
                            reached = true;
                            Console.WriteLine("Ziel erreicht. Motor aus.");
                            writer.WriteLine(0); // Signal to stop motor
                        }
                    }
                    else
                    {
                        Console.WriteLine("Ungültige Daten empfangen.");
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Fehler: {0}", e.Message);
                return false;
            }
        }
    }
}