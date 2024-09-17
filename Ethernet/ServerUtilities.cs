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
        public static void StartServer()
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
                        const double angleDistance = 5.1;
                        if (counter * angleDistance <= lenghtToReach - angleDistance)
                        {
                            Console.WriteLine("no break");  // Print the received data
                            response = 1;
                        }
                        else if (counter * angleDistance >= lenghtToReach + angleDistance)
                        {
                            Console.WriteLine("no break");  // Print the received data
                            response = 2;
                        }
                        else
                        {
                            response = 0;
                            Console.WriteLine("break");
                        }
                        Console.WriteLine($"Empfangener Zählerwert: {counter * angleDistance}");
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
                client.Close();
                //Console.WriteLine("Client getrennt.");
            }
        }
    }
}