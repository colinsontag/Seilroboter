using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MathNet.Spatial.Euclidean;
using Machine;

namespace Ethernet
{
    public class ServerUtilities
    {
        private static readonly List<Drive> drives = new List<Drive>
        {
            new  Drive("Drive1", new Point3D(0, 0, 0), 0, 1),
            new Drive("Drive2", new Point3D(1, 0, 0), 0, 2),
            new Drive("Drive3", new Point3D(0, 1, 0), 0, 3)
        };

        private static readonly Dictionary<string, bool> deviceStatus = new Dictionary<string, bool>(); // Status jedes Geräts
        private static readonly Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>(); // Alle verbundenen Clients

        public static void StartServer()
        {
            TcpListener server = null;
            try
            {
                int port = 5000;
                IPAddress localAddr = IPAddress.Any;

                Console.WriteLine("Bitte geben Sie die gewünschten X, Y und Z Koordinaten ein:");
                double x = Convert.ToDouble(Console.ReadLine());
                double y = Convert.ToDouble(Console.ReadLine());
                double z = Convert.ToDouble(Console.ReadLine());

                // Berechne die Längen für jede Seilwinde basierend auf den X, Y und Z Koordinaten
                CalculateLengths(x, y, z);

                server = new TcpListener(localAddr, port);
                server.Start();

                string localIP = GetLocalIPAddress();
                Console.WriteLine($"The server IP address is: {localIP} and port is {port}");

                Console.WriteLine("Server gestartet. Warte auf Verbindungen...");

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    string clientIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                    Console.WriteLine($"Neue Verbindung von: {clientIp}");

                    // Prüfen, ob der Client schon verbunden ist
                    if (!clients.ContainsKey(clientIp))
                    {
                        clients[clientIp] = client; // Speichern des Clients
                        deviceStatus[clientIp] = false; // Anfangsstatus: Ziel nicht erreicht
                        Task.Run(() => HandleClient(client, clientIp));
                    }

                    // Überprüfen, ob alle Geräte ihr Ziel erreicht haben
                    if (deviceStatus.Values.All(status => status))
                    {
                        Console.WriteLine("Alle Geräte haben ihr Ziel erreicht. Bitte geben Sie die neuen X, Y und Z Koordinaten ein:");
                        x = Convert.ToDouble(Console.ReadLine());
                        y = Convert.ToDouble(Console.ReadLine());
                        z = Convert.ToDouble(Console.ReadLine());

                        // Berechne die neuen Längen
                        CalculateLengths(x, y, z);

                        deviceStatus.Keys.ToList().ForEach(key => deviceStatus[key] = false); // Status zurücksetzen
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
        }

        private static void CalculateLengths(double x, double y, double z)
        {
            Point3D targetPosition = new Point3D(x, y, z);

            foreach (var drive in drives)
            {
                drive.UnrolledCableLength = CalculateDistance(drive.MountPosition, targetPosition);
            }
        }

        private static double CalculateDistance(Point3D motorPosition, Point3D targetPosition)
        {
            // Berechne die euklidische Distanz zwischen dem Motor und dem Zielpunkt
            return motorPosition.DistanceTo(targetPosition);
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

        private static void HandleClient(TcpClient client, string clientIp)
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
                    if (data == null) break; // Verbindung unterbrochen

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
                        double lengthToReach = GetLengthToReachForClient(clientIp);

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
            }
            catch (Exception e)
            {
                Console.WriteLine($"Fehler bei {clientIp}: {e.Message}");
            }
            finally
            {
            }
        }

        private static double GetLengthToReachForClient(string clientIp)
        {
            // Finde den Drive, der zur gegebenen IP-Adresse passt
            var drive = drives.FirstOrDefault(d => d.EthernetIP.ToString() == clientIp);
            if (drive != null)
            {
                return drive.UnrolledCableLength;
            }
            else
            {
                throw new Exception($"Kein Drive mit der IP-Adresse {clientIp} gefunden.");
            }
        }
    }
}