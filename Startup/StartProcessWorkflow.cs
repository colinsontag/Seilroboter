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
            var connectionSettings = new I2cConnectionSettings(I2CBusId, drive1Adress);
            I2cDevice i2cDevice = I2cDevice.Create(connectionSettings);
            bool motorOn = false;
            bool motorPlus = false;
            drives[0].UnrolledCableLength = RefreshDrive(i2cDevice,drive1Adress, I2CBusId);
            Console.WriteLine(drives[0].UnrolledCableLength);
            double cableLenghtToReach = 150;
            while (Math.Abs(drives[0].UnrolledCableLength - cableLenghtToReach) > 1)
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
                SendMotor(i2cDevice,drive1Adress, I2CBusId, motorOn,motorPlus);
                drives[0].UnrolledCableLength = RefreshDrive(i2cDevice,drive1Adress, I2CBusId);
                Console.WriteLine(drives[0].UnrolledCableLength);
                Thread.Sleep(500);
                motorOn = false;
            }            
            SendMotor(i2cDevice,drive1Adress, I2CBusId, motorOn, motorPlus);

        }
        private static double RefreshDrive(I2cDevice i2cDevice,int arduinoAddress, int I2CBusId)
        {
            var counterValue = GetCounterValue(i2cDevice,I2CBusId, arduinoAddress);
            Console.WriteLine($"Daten von Arduino {arduinoAddress}: {counterValue}");
            
            var cableLenght = counterValue * 3.2725;
            return cableLenght;
            
        }
        private static void SendMotor(I2cDevice i2cDevice,int I2CBusId, int address, bool motorOn, bool motorPlus)
        {
            
            byte dataToSend = 0;
            if (motorOn) dataToSend |= 0x01; // Setze Bit 0 für command1
            if (motorPlus) dataToSend |= 0x02; // Setze Bit 1 für command2

            // Daten senden
            i2cDevice.Write(new byte[] { dataToSend });

           
        }
        private static int GetCounterValue(I2cDevice i2cDevice,int I2CBusId, int address)
        {
            

            byte[] receiveBuffer = new byte[4];

            i2cDevice.Read(receiveBuffer);
            Array.Reverse(receiveBuffer);
            
            return BitConverter.ToInt32(receiveBuffer, 0);
        }
    }
}
