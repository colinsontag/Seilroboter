using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Seilroboter Anwendung Gestartet");
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();
                // Server starten
                StartServer();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                stopwatch.Stop();
                Console.WriteLine("Seilroboter Anwendung Beendet");
                Console.WriteLine("Ausgeführte Zeit: " + stopwatch.Elapsed);
                Console.WriteLine("Drücken Sie eine Taste zum Beenden...");
                Console.ReadKey();
            }
        }

        private static void StartServer()
        {
            TcpListener server = null;
            try
            {
                // Server auf Port 5000 starten
                Int32 port = 5000;
                IPAddress localAddr = IPAddress.Parse("192.168.1.1"); // Raspberry Pi IP
                //IPAddress localAddr = IPAddress.Any; // Raspberry Pi IP
                server = new TcpListener(localAddr, port);

                server.Start();
                Console.WriteLine("Server gestartet. Warte auf Verbindungen...");

                // Endlosschleife für den Serverbetrieb
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Client verbunden!");

                    Task.Run(() => HandleClient(client));
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server?.Stop();
            }
        }

        private static void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[256];
            int bytesRead;

            try
            {
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Empfangen: {data}");
                    
                    string response = "Befehl empfangen";
                    byte[] msg = Encoding.ASCII.GetBytes(response);
                    stream.Write(msg, 0, msg.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }
            finally
            {
                client.Close();
            }
        }
    }
}