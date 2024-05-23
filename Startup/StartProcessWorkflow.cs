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
    internal static class StartProcessWorkflow
    {

        internal static void Start(List<Drive> drives, MachinePoint tCP)
        {
            const int I2CBusId = 1;
            const int drive1Adress = 9;
            const int drive2Adress = 10;
            const int drive3Adress = 11;
            bool motorOn = false;
            drives[0].UnrolledCableLength = RefreshDrive(drive1Adress, I2CBusId);
            Console.WriteLine(drives[0].UnrolledCableLength);
            double cableLenghtToReach = 150;
            while (drives[0].UnrolledCableLength >= cableLenghtToReach)
            {
                motorOn = true;
                SendMotor(drive1Adress, I2CBusId, motorOn);
                drives[0].UnrolledCableLength = RefreshDrive(drive1Adress, I2CBusId);
                Console.WriteLine(drives[0].UnrolledCableLength);
                Thread.Sleep(500);
                motorOn = false;
            }            
            SendMotor(drive1Adress, I2CBusId, motorOn);

        }
        private static double RefreshDrive(int arduinoAddress, int I2CBusId)
        {
            var counterValue = GetCounterValue(I2CBusId, arduinoAddress);
            Console.WriteLine($"Daten von Arduino {arduinoAddress}: {counterValue}");
            
            var cableLenght = counterValue * 3.2725;
            return cableLenght;
            
        }
        private static void SendMotor(int I2CBusId, int address, bool motorOn)
        {
            var connectionSettings = new I2cConnectionSettings(I2CBusId, address);
            I2cDevice i2cDevice = I2cDevice.Create(connectionSettings);
            byte[] dataToSend = new byte[] { motorOn ? (byte)1 : (byte)0 };
            i2cDevice.Write(dataToSend);

        }
        private static int GetCounterValue(int I2CBusId, int address)
        {
            var connectionSettings = new I2cConnectionSettings(I2CBusId, address);
            I2cDevice i2cDevice = I2cDevice.Create(connectionSettings);

            byte[] receiveBuffer = new byte[4];

            i2cDevice.Read(receiveBuffer);
            Array.Reverse(receiveBuffer);

            return BitConverter.ToInt32(receiveBuffer, 0);
        }
    }
}
