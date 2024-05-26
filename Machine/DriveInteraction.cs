using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.I2c;

namespace Machine
{
    public static class DriveInteraction
    {
        public static void ChangeDriveCabelLenght(List<Drive> drives, double angleDistance, int I2CBusIdController, List<I2cDevice> i2cDevices, double cableLenghtToReach, int i)
        {
            Console.WriteLine("ChangeDriveCabelLenght Start");
            bool motorOn = true;
            bool motorPlus = false;
            if (drives[i].UnrolledCableLength < cableLenghtToReach)
            {
                motorPlus = true;
            }
            else
            {
                motorPlus = false;
            }
            SendMotor(i2cDevices.ToArray()[i], motorOn, motorPlus);
            drives[0].UnrolledCableLength = RefreshDrive(i2cDevices.ToArray()[i], drives[i].I2CBusId, I2CBusIdController, angleDistance);
            Console.WriteLine(drives[0].UnrolledCableLength);
        }
        public static double RefreshDrive(I2cDevice i2cDevice, int arduinoAddress, int I2CBusId, double angleDistance)
        {
            var counterValue = GetCounterValue(i2cDevice);
            Console.WriteLine($"Daten von Arduino {arduinoAddress}: {counterValue}");

            var cableLenght = counterValue * angleDistance;
            return cableLenght;
        }
        public static void SendMotor(I2cDevice i2cDevice, bool motorOn, bool motorPlus)
        {
            Console.WriteLine("SendMotor Start");
            byte dataToSend = 0;
            if (motorOn) dataToSend |= 0x01; // Setze Bit 0 für command1
            if (motorPlus) dataToSend |= 0x02;

            // Daten senden
            i2cDevice.Write(new byte[] { dataToSend });
        }
        public static int GetCounterValue(I2cDevice i2cDevice)
        {
            Console.WriteLine("GetCounterValue Start");
            Console.WriteLine(i2cDevice.ConnectionSettings.BusId + " - " +i2cDevice.ConnectionSettings.DeviceAddress);
            byte[] receiveBuffer = new byte[4];
            i2cDevice.Read(receiveBuffer);
            Console.WriteLine("GetCounterValue After");
            Array.Reverse(receiveBuffer);
            return BitConverter.ToInt32(receiveBuffer, 0);
        }
    }
}
