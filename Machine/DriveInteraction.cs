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
        public static void ChangeDriveCabelLenght(Drive drive, double angleDistance, int I2CBusIdController, I2cDevice i2cDevice, double cableLenghtToReach)
        {
            bool motorOn = true;
            bool motorPlus = false;
            if (drive.UnrolledCableLength < cableLenghtToReach)
            {
                motorPlus = true;
            }
            else
            {
                motorPlus = false;
            }
            SendMotor(i2cDevice, motorOn, motorPlus);
            drive.UnrolledCableLength = RefreshDrive(i2cDevice, drive.I2CBusId, I2CBusIdController, angleDistance);
            Console.WriteLine(drive.UnrolledCableLength);
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
            byte dataToSend = 0;
            if (motorOn) dataToSend |= 0x01; // Setze Bit 0 für command1
            if (motorPlus) dataToSend |= 0x02;

            // Daten senden

            int retryCount = 5;
            // Daten senden
            // Attempt to send data, retrying if an error occurs
            int attempts = 0;
            bool success = false;
            while (attempts < retryCount && !success)
            {
                try
                {
                    i2cDevice.Write(new byte[] { dataToSend });
                    success = true; // If no exception, mark as successful
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Attempts: " + attempts);
                    attempts++;
                    if (attempts >= retryCount)
                    {
                        throw new Exception($"Failed to send I2C data after {retryCount} attempts.", ex);
                    }
                    // Optionally, you can log the exception or wait for some time before retrying
                    System.Threading.Thread.Sleep(100); // Wait for 100 ms before retrying
                }
            }
        }

        public static int GetCounterValue(I2cDevice i2cDevice)
        {
            Console.WriteLine("GetCounterValue Start");
            Console.WriteLine(i2cDevice.ConnectionSettings.BusId + " - " + i2cDevice.ConnectionSettings.DeviceAddress);
            byte[] receiveBuffer = new byte[4];
            i2cDevice.Read(receiveBuffer);
            Console.WriteLine("GetCounterValue After");
            Array.Reverse(receiveBuffer);
            return BitConverter.ToInt32(receiveBuffer, 0);
        }
    }
}