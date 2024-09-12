using Machine;
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
                int lenghtToReach = 0;
                Console.WriteLine("Eingabe der zu erreichenden länge");
                lenghtToReach = Convert.ToInt32(Console.ReadLine());
                server.Start();
                Console.WriteLine("Server gestartet. Warte auf Verbindungen...");

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();

                    Task.Run(() => HandleClient(client, lenghtToReach));
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

        private static void HandleClient(TcpClient client, int lenghtToReach)
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
                    int response = 0;

                    Console.WriteLine($"Empfangen: {data}");  // Print the received data

                    int counter;
                    if (int.TryParse(data, out counter))  // Try to parse the data to an integer
                    {
                        const double angleDistance = 3.2725;
                        if (counter * angleDistance <= lenghtToReach)
                        {
                            Console.WriteLine("no break");  // Print the received data
                            response = 1;
                        }
                        else
                        {
                            response = 0;
                            Console.WriteLine("break");
                            break;
                        }
                        Console.WriteLine($"Empfangener Zählerwert: {counter}");
                    }
                    else
                    {
                        Console.WriteLine("Ungültige Daten empfangen.");
                    }

                    // Send a response back to the client (Arduino)

                    writer.WriteLine(response);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }
            finally
            {
                //client.Close();
                //Console.WriteLine("Client getrennt.");
            }
        }
    }
}