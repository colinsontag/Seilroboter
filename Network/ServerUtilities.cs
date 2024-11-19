using Machine;
using MathNet.Spatial.Euclidean;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;

public class ServerUtilities
{
    private static Point3D currentPosition = new Point3D(2400, 2000, -1800); // Starting position of the connection point

    private static readonly List<Drive> drives = new List<Drive>
        {
            new Drive("Drive1", new Point3D(0, 0, 0), 0, "DE-AD-BE-EF-FE-ED"),
            new Drive("Drive2", new Point3D(0, 3000, 0), 0, "F4-12-FA-6E-CF-EC"),
            new Drive("Drive3", new Point3D(4800, 1500, 0), 0, "F4-12-FA-6E-97-BC")
        };

    private static readonly Dictionary<string, bool> deviceStatus = new Dictionary<string, bool>(); // Status jedes Geräts
    private static readonly Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>(); // Alle verbundenen Clients

    public static void StartServer()
    {
        TcpListener server = null;
        try
        {
            currentPosition = new Point3D(2400, 2400, -1800);
            int port = 5000;
            IPAddress localAddr = IPAddress.Any;

            // Berechnung der initialen Kabellängen (relativ zur aktuellen Position)
            CalculateLengths(-400, 0, 0);  // Beispiel: keine Veränderung der Ausgangsposition
            Console.WriteLine($"Initial position (already reached): {currentPosition}");

            server = new TcpListener(localAddr, port);
            server.Start();

            string localIP = GetLocalIPAddress();
            Console.WriteLine($"The server IP address is: {localIP} and port is {port}");

            Console.WriteLine("Server gestartet. Warte auf Verbindungen...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                string clientIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                string macAddress = getMacByIp(clientIp);
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
                    Console.WriteLine("Alle Geräte haben ihren Ausgangspunkt erreicht.");
                    Console.WriteLine("Bitte geben Sie die neuen X, Y und Z Koordinaten ein:");

                    double x = Convert.ToDouble(Console.ReadLine());
                    double y = Convert.ToDouble(Console.ReadLine());
                    double z = Convert.ToDouble(Console.ReadLine());

                    // Berechne die Kabellängen relativ zur aktuellen Position
                    CalculateLengths(x, y, z);

                    // Update der `currentPosition` auf den neuen Zielpunkt
                    currentPosition = new Point3D(x, y, z);
                    Console.WriteLine($"Neue Ausgangsposition gesetzt: {currentPosition}");

                    // Setze den Status zurück
                    deviceStatus.Keys.ToList().ForEach(key => deviceStatus[key] = false);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: {0}", e);
        }
    }

    public static string getMacByIp(string ip)
    {
        var macIpPairs = GetAllMacAddressesAndIppairs();
        int index = macIpPairs.FindIndex(x => x.IpAddress == ip);
        if (index >= 0)
        {
            return macIpPairs[index].MacAddress.ToUpper();
        }
        else
        {
            return null;
        }
    }

    public static List<MacIpPair> GetAllMacAddressesAndIppairs()
    {
        List<MacIpPair> mip = new List<MacIpPair>();
        System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
        pProcess.StartInfo.FileName = "arp";
        pProcess.StartInfo.Arguments = "-a ";
        pProcess.StartInfo.UseShellExecute = false;
        pProcess.StartInfo.RedirectStandardOutput = true;
        pProcess.StartInfo.CreateNoWindow = true;
        pProcess.Start();
        string cmdOutput = pProcess.StandardOutput.ReadToEnd();
        string pattern = @"(?<ip>([0-9]{1,3}\.?){4})\s*(?<mac>([a-f0-9]{2}-?){6})";

        foreach (Match m in Regex.Matches(cmdOutput, pattern, RegexOptions.IgnoreCase))
        {
            mip.Add(new MacIpPair()
            {
                MacAddress = m.Groups["mac"].Value,
                IpAddress = m.Groups["ip"].Value
            });
        }

        return mip;
    }

    public struct MacIpPair
    {
        public string MacAddress;
        public string IpAddress;
    }

    // Berechnung der Kabellängen relativ zur aktuellen Position
    private static void CalculateLengths(double x, double y, double z)
    {
        // Zielpunkt relativ zur aktuellen Position (nicht absolut)
        Point3D targetPosition = new Point3D(currentPosition.X + x, currentPosition.Y + y, currentPosition.Z + z);

        foreach (var drive in drives)
        {
            drive.UnrolledCableLength = CalculateDistance(drive.MountPosition, targetPosition);
            Console.WriteLine($"Motor: {drive.Name}, Zielposition: ({targetPosition.X:F2}, {targetPosition.Y:F2}, {targetPosition.Z:F2}), Berechnete Distanz: {drive.UnrolledCableLength:F2} Einheiten");
        }
        currentPosition = targetPosition;
    }

    // Berechnung der Distanz (zwischen den Punkten)
    private static double CalculateDistance(Point3D motorPosition, Point3D targetPosition)
    {
        var mountToActualPoistionDistance = motorPosition.DistanceTo(currentPosition);
        var targetDistance = motorPosition.DistanceTo(targetPosition);
        return targetDistance - mountToActualPoistionDistance;
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

                    double lengthToReach = GetLengthToReachForClient(clientIp);

                    Console.WriteLine($"{clientIp}");
                    Console.WriteLine($"  Empfangener Zählerwert: {counter}");
                    Console.WriteLine($"  Berechnete Distanz: {calculatedDistance}");
                    Console.WriteLine($"  Erforderliche Ziel-Distanz: {lengthToReach:F2} Einheiten");

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
        }
        catch (Exception e)
        {
            Console.WriteLine($"Fehler bei {clientIp}: {e.Message}");
        }
    }

    private static double GetLengthToReachForClient(string clientIp)
    {
        // Finde den Drive, der zur gegebenen IP-Adresse passt
        var drive = drives.FirstOrDefault(d => d.EthernetIP.ToString() == getMacByIp(clientIp));
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