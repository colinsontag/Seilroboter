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

namespace Startup
{
    internal static class SingleTCPProcess
    {

        internal static void Start(List<Drive> drives, List<MachinePoint> pointList)
        {
            const double angleDistance = 3.2725;
            const int I2CBusId = 1;
            const int drive1Adress = 9;
            const int drive2Adress = 10;
            const int drive3Adress = 11;
            var connectionSettings = new I2cConnectionSettings(I2CBusId, drive1Adress);
            I2cDevice i2cDevice = I2cDevice.Create(connectionSettings);
            bool motorOn = false;
            bool motorPlus = false;
            drives[0].UnrolledCableLength = RefreshDrive(i2cDevice, drive1Adress, I2CBusId,angleDistance);
            Console.WriteLine(drives[0].UnrolledCableLength);

            double cableLenghtToReach = 150;
            while (cableLenghtToReach != 666)
            {
                while (Math.Abs(drives[0].UnrolledCableLength - cableLenghtToReach) >= angleDistance)
                {
                    motorOn = true;
                    if (drives[0].UnrolledCableLength < cableLenghtToReach)
                    {
                        motorPlus = true;
                    }
                    else
                    {
                        motorPlus = false;
                    }
                    SendMotor(i2cDevice, motorOn, motorPlus);
                    drives[0].UnrolledCableLength = RefreshDrive(i2cDevice, drive1Adress, I2CBusId,angleDistance);
                    Console.WriteLine(drives[0].UnrolledCableLength);
                    
                    motorOn = false;
                }
                SendMotor(i2cDevice, motorOn, motorPlus);
                Console.WriteLine("Neue Laenge Eingeben");
                cableLenghtToReach = Convert.ToDouble(Console.ReadLine());
            }
        }
        private static double RefreshDrive(I2cDevice i2cDevice, int arduinoAddress, int I2CBusId, double angleDistance)
        {
            var counterValue = GetCounterValue(i2cDevice);
            Console.WriteLine($"Daten von Arduino {arduinoAddress}: {counterValue}");

            var cableLenght = counterValue * angleDistance;
            return cableLenght;
        }
        private static void SendMotor(I2cDevice i2cDevice, bool motorOn, bool motorPlus)
        {
            byte dataToSend = 0;
            if (motorOn) dataToSend |= 0x01; // Setze Bit 0 für command1
            if (motorPlus) dataToSend |= 0x02;

            // Daten senden
            i2cDevice.Write(new byte[] { dataToSend });
        }
        private static int GetCounterValue(I2cDevice i2cDevice)
        {
            byte[] receiveBuffer = new byte[4];
            i2cDevice.Read(receiveBuffer);
            Array.Reverse(receiveBuffer);
            return BitConverter.ToInt32(receiveBuffer, 0);
        }
    }
}
