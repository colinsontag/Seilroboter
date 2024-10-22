﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices.ComTypes;

namespace Ethernet
{
    public class ServerUtilities
    {
        private static int lenghtToReach = 0;

        public static void StartServer()
        {
            TcpListener server = null;
            try
            {
                Int32 port = 5000;
                var localAddr = IPAddress.Any;  // Accept connections from any IP address

                Console.WriteLine("Eingabe der zu erreichenden länge");
                lenghtToReach = Convert.ToInt32(Console.ReadLine());
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

                    if (!tasksDic.ContainsKey(clientIp))
                    {
                        var task = Task.Run(() => HandleClient(client, lenghtToReach));
                        tasksDic.Add(clientIp, task);
                    }
                    if (tasksDic.Values.All(t => t.Status == TaskStatus.RanToCompletion))
                    {
                        Console.WriteLine("Eingabe der zu erreichenden länge");
                        lenghtToReach = Convert.ToInt32(Console.ReadLine());
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

        private static bool HandleClient(TcpClient client, int lenghtToReach)
        {
            NetworkStream stream = client.GetStream();
            var reached = false;
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

            try
            {
                int response = 0;
                while (reached == false)
                {
                    string data = reader.ReadLine();
                    if (data == null) break;

                    Console.WriteLine($"Empfangen: {data}");

                    int counter;
                    if (int.TryParse(data, out counter))
                    {
                        Console.WriteLine("lenghtto: " + lenghtToReach);
                        const double angleDistance = 5.1;
                        if (counter * angleDistance <= lenghtToReach - angleDistance)
                        {
                            Console.WriteLine("no break");
                            response = 1;
                        }
                        else if (counter * angleDistance >= lenghtToReach + angleDistance)
                        {
                            Console.WriteLine("no break");
                            response = 2;
                        }
                        else
                        {
                            reached = true;
                            response = 0;
                            Console.WriteLine("break");
                        }
                        Console.WriteLine($"Empfangener Zählerwert: {counter * angleDistance}");
                    }
                    else
                    {
                        Console.WriteLine("Ungültige Daten empfangen.");
                    }
                    writer.WriteLine(response);
                    // Send a response back to the client (Arduino)
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                return false;
            }
        }
    }
}