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
                Int32 port = 5000;
                IPAddress localAddr = IPAddress.Any;  // Accept connections from any IP address
                server = new TcpListener(localAddr, port);

                server.Start();
                Console.WriteLine("Server gestartet. Warte auf Verbindungen...");

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
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

            try
            {
                while (true)
                {
                    string data = reader.ReadLine();  // Read the incoming data as a string
                    if (data == null) break;  // Exit the loop if client disconnects

                    Console.WriteLine($"Empfangen: {data}");  // Print the received data

                    int counter;
                    if (int.TryParse(data, out counter))  // Try to parse the data to an integer
                    {
                        Console.WriteLine($"Empfangener Zählerwert: {counter}");
                    }
                    else
                    {
                        Console.WriteLine("Ungültige Daten empfangen.");
                    }

                    // Send a response back to the client (Arduino)
                    string response = "Befehl empfangen\n";
                    writer.WriteLine(response);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }
            finally
            {
                client.Close();
                Console.WriteLine("Client getrennt.");
            }
        }
    }
}