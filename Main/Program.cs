using Startup;

namespace Main
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Seilroboter Anwendung Gestartet");
            try
            {
                StartupWorkflow.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);                
            }
            finally
            {
                Console.WriteLine("Seilroboter Anwendung Beendet");
                Console.WriteLine("Drücken Sie eine Taste zum Beenden...");
                Console.ReadKey();
            }
            
            
        }
    }
}
