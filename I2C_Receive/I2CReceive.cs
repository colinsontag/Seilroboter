using System;
using System.Threading;
using System.Device.I2c;

class Program
{
    private static bool askForCounterValues = true;
    static void Main(string[] args)
    {
        //I2C-Busnumber
        const int I2CBusId = 1;

        //Arduino I2C Addresses
        int[] arduinoAddresses = { 9, 10, 11 };
        while (askForCounterValues)
        {      
            foreach (int address in arduinoAddresses)
            {                             
                var counterValue = GetCounterValue(I2CBusId, address);
                Console.WriteLine($"Daten von Arduino {address}: {counterValue}");                
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
