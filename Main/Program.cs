using Startup;
using System.Device.I2c;
using System;
using System.Diagnostics;
using Machine;

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
                StartupWorkflow.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                stopwatch.Stop();
                const int busId = 1;
                Console.WriteLine("motor stop routine");
                // I2C-Adressen der Arduinos
                const int arduinoAddress1 = 0x08;
                const int arduinoAddress2 = 0x09;
                const int arduinoAddress3 = 0x0A;
                // Initialisiere I2C-Verbindung zu jedem Arduino
                var i2cSettings1 = new I2cConnectionSettings(busId, arduinoAddress1);
                var i2cDevice1 = I2cDevice.Create(i2cSettings1);

                var i2cSettings2 = new I2cConnectionSettings(busId, arduinoAddress2);
                var i2cDevice2 = I2cDevice.Create(i2cSettings2);

                var i2cSettings3 = new I2cConnectionSettings(busId, arduinoAddress3);
                var i2cDevice3 = I2cDevice.Create(i2cSettings3);
                var motorOn = false;
                var motorPlus = false;
                Console.WriteLine("Start Disabling Motors");
                DriveInteraction.SendMotor(i2cDevice1, motorOn, motorPlus);
                DriveInteraction.SendMotor(i2cDevice2, motorOn, motorPlus);
                DriveInteraction.SendMotor(i2cDevice3, motorOn, motorPlus);

                Console.WriteLine("Seilroboter Anwendung Beendet");
                Console.WriteLine("Ausgeführte Zeit: " + stopwatch.Elapsed);
                Console.WriteLine("Drücken Sie eine Taste zum Beenden...");
                Console.ReadKey();
            }
        }
    }
}