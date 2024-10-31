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
                Console.WriteLine("Server gestartet. Warte auf Verbindungen...");

                var tasksDic = new Dictionary<string, Task>();

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    string clientIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                    Console.WriteLine($"Verbindung von {clientIp}");

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
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e.Message);
            }
            finally
            {
                server?.Stop();
            }
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
                    string data = reader.ReadLine();  // Read the incoming data as a string
                    if (data == null) break;          // Exit the loop if client disconnects

                    if (int.TryParse(data, out int counter))  // Parse the counter data
                    {
                        Console.WriteLine($"Empfangener Zählerwert: {counter}");

                        const double angleDistance = 5.1;
                        double calculatedDistance = counter * angleDistance;

                        if (calculatedDistance <= lengthToReach - angleDistance)
                        {
                            Console.WriteLine("Motor On Up");
                            writer.WriteLine(1); // Signal to move motor up
                        }
                        else if (calculatedDistance >= lengthToReach + angleDistance)
                        {
                            Console.WriteLine("Motor On Down");
                            writer.WriteLine(2); // Signal to move motor down
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
            finally
            {
                client.Close();
            }
        }
    }
}