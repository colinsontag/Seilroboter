using ConfigManger;
using Machine;
using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.I2c;

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
            int[] arduinoAddresses = { drive1Adress, drive2Adress, drive3Adress };
            RefreshDrives(drives, arduinoAddresses, I2CBusId);
            Console.WriteLine(drives[0].UnrolledCableLength);
            while (true)
            {
                RefreshDrives(drives, arduinoAddresses, I2CBusId);
                Console.WriteLine(drives[0].UnrolledCableLength);
                Thread.Sleep(500);
            }

        }
        private static void RefreshDrives(List<Drive> drives, int[] arduinoAddresses, int I2CBusId)
        {
            foreach (int address in arduinoAddresses)
            {
                var counterValue = GetCounterValue(I2CBusId, address);
                Console.WriteLine($"Daten von Arduino {address}: {counterValue}");
                if (address == 9)
                {
                    drives[0].UnrolledCableLength = counterValue * ((15 / 360) * 78.54);
                }
            }
            

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
