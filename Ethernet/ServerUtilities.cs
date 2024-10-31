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
                IPAddress localAddr = IPAddress.Any;

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
                    Console.WriteLine(lengthToReach);

                    foreach (var task in tasksDic)
                    {
                        if (task.Value.Status == TaskStatus.Running)
                        {
                            Console.WriteLine($"running device IP: {task.Key}");
                        }
                    }
                    string clientIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

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
                Console.WriteLine("Exception: {0}", e.Message);
            }
            finally
            {
                server?.Stop();
            }
        }

        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
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
                        int response;

                        if (calculatedDistance <= lengthToReach - angleDistance)
                        {
                            Console.WriteLine("Motor läuft hoch");
                            response = 1; // Signal to move motor up
                        }
                        else if (calculatedDistance >= lengthToReach + angleDistance)
                        {
                            Console.WriteLine("Motor läuft runter");
                            response = 2; // Signal to move motor down
                        }
                        else
                        {
                            reached = true;
                            Console.WriteLine("Ziel erreicht. Motor aus.");
                            response = 0; // Signal to stop motor
                        }

                        writer.WriteLine(response); // Send response to client
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
            finally
            {
                client.Close();
            }
        }
    }
}