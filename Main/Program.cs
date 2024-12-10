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
                ServerUtilities.StartServer();
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
    }
}