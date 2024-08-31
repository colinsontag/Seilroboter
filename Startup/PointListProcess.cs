using ConfigManger;
using Machine;
using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Startup
{
    internal static class PointListProcess
    {
        internal static void Start(List<Drive> drives, List<MachinePoint> pointList)
        {
            const double angleDistance = 3.2725;

            // IP-Adressen und Ports der Arduinos
            string ipAddress1 = "192.168.1.100";
            int port1 = 80;

            string ipAddress2 = "192.168.1.101";
            int port2 = 80;

            string ipAddress3 = "192.168.1.102";
            int port3 = 80;

            foreach (Drive drive in drives)
            {
                Console.WriteLine($"Drive IP:{drive.I2CBusId}:" + drive.UnrolledCableLength.ToString());
            }

            double cableLenghtToReach = 150;
            while (cableLenghtToReach != 666)
            {
                foreach (Drive drive in drives)
                {
                    if (Math.Abs(drive.UnrolledCableLength - cableLenghtToReach) >= angleDistance)
                    {
                        Console.WriteLine(drive.I2CBusId + ":" + Math.Abs(drive.UnrolledCableLength - cableLenghtToReach));
                        if (drive.I2CBusId == 1) // drive.I2CBusId entspricht i2cDevice1
                        {
                            DriveInteraction.ChangeDriveCabelLenght(drive, angleDistance, ipAddress1, port1, cableLenghtToReach);
                        }
                        else if (drive.I2CBusId == 2) // drive.I2CBusId entspricht i2cDevice2
                        {
                            DriveInteraction.ChangeDriveCabelLenght(drive, angleDistance, ipAddress2, port2, cableLenghtToReach);
                        }
                        else if (drive.I2CBusId == 3) // drive.I2CBusId entspricht i2cDevice3
                        {
                            DriveInteraction.ChangeDriveCabelLenght(drive, angleDistance, ipAddress3, port3, cableLenghtToReach);
                        }
                    }
                }
            }

            bool motorOn = false;
            bool motorPlus = false;
            Console.WriteLine("Start Disabling Motors");

            DriveInteraction.SendMotor(ipAddress1, port1, motorOn, motorPlus);
            DriveInteraction.SendMotor(ipAddress2, port2, motorOn, motorPlus);
            DriveInteraction.SendMotor(ipAddress3, port3, motorOn, motorPlus);

            Console.WriteLine("Neue Laenge Eingeben");
            cableLenghtToReach = Convert.ToDouble(Console.ReadLine());
        }
    }
}