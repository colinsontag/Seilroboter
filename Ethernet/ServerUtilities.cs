﻿using System;
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
        private static readonly Dictionary<string, bool> deviceStatus = new Dictionary<string, bool>(); // Status jedes Geräts

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
                    string clientIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                    Console.WriteLine($"Neue Verbindung von: {clientIp}");

                    // Alte abgeschlossene Tasks entfernen
                    tasksDic = tasksDic.Where(t => t.Value.Status == TaskStatus.Running)
                                       .ToDictionary(t => t.Key, t => t.Value);

                    if (!tasksDic.ContainsKey(clientIp))
                    {
                        var task = Task.Run(() => HandleClient(client, lengthToReach, clientIp));
                        tasksDic[clientIp] = task;
                    }

                    // Überprüfen, ob alle Geräte ihr Ziel erreicht haben
                    if (tasksDic.Values.All(t => t.IsCompleted))
                    {
                        // Überprüfen, ob alle Geräte erfolgreich ihr Ziel erreicht haben
                        if (deviceStatus.Values.All(status => status))
                        {
                            Console.WriteLine("Alle Geräte haben ihr Ziel erreicht. Bitte geben Sie die neue Länge ein:");
                            lengthToReach = Convert.ToInt32(Console.ReadLine());
                            deviceStatus.Clear(); // Status zurücksetzen für die neuen Berechnungen
                        }
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
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private static bool HandleClient(TcpClient client, int lengthToReach, string clientIp)
        {
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

            bool reached = false;

            try
            {
                Console.WriteLine($"\nVerbindung gestartet mit: {clientIp}");

                while (!reached)
                {
                    string data = reader.ReadLine();
                    if (data == null) break;

                    int counter;
                    if (int.TryParse(data, out counter))
                    {
                        const double angleDistance = 5.1;
                        double calculatedDistance = counter * angleDistance;

                        // Strukturierte Ausgabe mit eingerückten Zeilen
                        Console.WriteLine($"{clientIp}");
                        Console.WriteLine($"  Empfangener Zählerwert: {counter}");
                        Console.WriteLine($"  Berechnete Distanz: {calculatedDistance}");

                        int response;
                        if (calculatedDistance <= lengthToReach - angleDistance)
                        {
                            Console.WriteLine("  Status: Noch nicht erreicht");
                            response = 1;
                            deviceStatus[clientIp] = false; // Gerät hat Ziel nicht erreicht
                        }
                        else if (calculatedDistance >= lengthToReach + angleDistance)
                        {
                            Console.WriteLine("  Status: Über das Ziel hinaus");
                            response = 2;
                            deviceStatus[clientIp] = false; // Gerät hat Ziel nicht erreicht
                        }
                        else
                        {
                            reached = true;
                            Console.WriteLine("  Status: Ziel erreicht. Motor aus.");
                            response = 0;
                            deviceStatus[clientIp] = true; // Gerät hat Ziel erreicht
                        }

                        writer.WriteLine(response); // Antwort an den Arduino senden
                    }
                    else
                    {
                        Console.WriteLine($"{clientIp}\n  Status: Ungültige Daten empfangen.");
                    }
                }
                Console.WriteLine($"Verbindung beendet mit: {clientIp}\n");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Fehler bei {clientIp}: {e.Message}");
                return false;
            }
            finally
            {
                client.Close();
            }
        }
    }
}