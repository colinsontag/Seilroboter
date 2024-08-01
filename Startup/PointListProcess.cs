using ConfigManger;
using Machine;
using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.I2c;
using System.Net;
using I2C_Receive;

namespace Startup
{
    internal static class PointListProcess
    {
        internal static void Start(List<Drive> drives, List<MachinePoint> pointList)
        {
            const double angleDistance = 3.2725;
            const int I2CBusIdController = 1;
            bool motorOn = false;
            bool motorPlus = false;
            // I2C-Bus-Nummer (1 für Raspberry Pi)
            const int busId = 1;

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

            foreach (Drive drive in drives)
            {
                Console.WriteLine($"Drive I2C:{drive.I2CBusId}:" + drive.UnrolledCableLength.ToString());
            }

            double cableLenghtToReach = 150;
            while (cableLenghtToReach != 666)
            {
                foreach (Drive drive in drives)
                {
                    if (Math.Abs(drive.UnrolledCableLength - cableLenghtToReach) >= angleDistance)
                    {
                        Console.WriteLine("Start Changeing Cable Lenght");
                        Console.WriteLine(drive.I2CBusId + "  \\  " + i2cDevice1.ConnectionSettings.BusId);
                        if (drive.I2CBusId == i2cDevice1.ConnectionSettings.DeviceAddress)
                        {
                            DriveInteraction.ChangeDriveCabelLenght(drive, angleDistance, I2CBusIdController, i2cDevice1, cableLenghtToReach);
                        }
                        else if (drive.I2CBusId == i2cDevice2.ConnectionSettings.DeviceAddress)
                        {
                            DriveInteraction.ChangeDriveCabelLenght(drive, angleDistance, I2CBusIdController, i2cDevice2, cableLenghtToReach);
                        }
                        else if (drive.I2CBusId == i2cDevice3.ConnectionSettings.DeviceAddress)
                        {
                            DriveInteraction.ChangeDriveCabelLenght(drive, angleDistance, I2CBusIdController, i2cDevice3, cableLenghtToReach);
                        }
                    }
                }
                motorOn = false;
                Console.WriteLine("Start Disabling Motors");
                DriveInteraction.SendMotor(i2cDevice1, motorOn, motorPlus);
                DriveInteraction.SendMotor(i2cDevice2, motorOn, motorPlus);
                DriveInteraction.SendMotor(i2cDevice3, motorOn, motorPlus);
            }
            Console.WriteLine("Neue Laenge Eingeben");
            cableLenghtToReach = Convert.ToDouble(Console.ReadLine());
        }
    }
}