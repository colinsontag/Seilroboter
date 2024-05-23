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
        private static bool askForCounterValues = true;
        internal static void Start(List<Drive> drives, MachinePoint tCP)
        {
            const int I2CBusId = 1;
            const int drive1Adress = 9;
            const int drive2Adress = 10;
            const int drive3Adress = 11;
            int[] arduinoAddresses = { drive1Adress, drive2Adress, drive3Adress };
            while (askForCounterValues)
            {
                foreach (int address in arduinoAddresses)
                {
                    var counterValue = GetCounterValue(I2CBusId, address);
                    Console.WriteLine($"Daten von Arduino {address}: {counterValue}");
                    switch (address)
                    {                        
                        case drive1Adress:                            
                            break;
                        case drive2Adress:
                            break;
                        case drive3Adress:
                            break;
                        default:
                            break;
                    }                    
                    Thread.Sleep(500);
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
