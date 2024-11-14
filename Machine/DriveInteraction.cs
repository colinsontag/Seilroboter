using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Machine
{
    public static class DriveInteraction
    {
        public static void ChangeDriveCabelLenght(Drive drive, double angleDistance, string ipAddress, int port, double cableLenghtToReach)
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
<<<<<<< HEAD
            SendMotor(i2cDevice, motorOn, motorPlus);
            drive.UnrolledCableLength = RefreshDrive(i2cDevice, drive.EthernetIP, I2CBusIdController, angleDistance);
            Console.WriteLine($"Drive {drive.EthernetIP} CableLenght: " + drive.UnrolledCableLength);
=======
            SendMotor(ipAddress, port, motorOn, motorPlus);
            drive.UnrolledCableLength = RefreshDrive(ipAddress, port, angleDistance);
            Console.WriteLine(drive.UnrolledCableLength);
>>>>>>> Ethernet_Testing
        }

        public static double RefreshDrive(string ipAddress, int port, double angleDistance)
        {
            var counterValue = GetCounterValue(ipAddress, port);
            Console.WriteLine($"Daten von Arduino {ipAddress}:{port}: {counterValue}");

            var cableLenght = counterValue * angleDistance;
            return cableLenght;
        }

        public static void SendMotor(string ipAddress, int port, bool motorOn, bool motorPlus)
        {
            byte dataToSend = 0;
            if (motorOn) dataToSend |= 0x01; // Setze Bit 0 für command1
            if (motorPlus) dataToSend |= 0x02;
            int retryCount = 25;
            int attempts = 0;
            bool success = false;
            while (attempts < retryCount && !success)
            {
                try
                {
                    using (TcpClient client = new TcpClient(ipAddress, port))
                    {
                        NetworkStream stream = client.GetStream();
                        stream.Write(new byte[] { dataToSend }, 0, 1);
                        success = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Attempts: " + attempts);
                    attempts++;
                    if (attempts >= retryCount)
                    {
                        throw new Exception($"Failed to send data after {retryCount} attempts.", ex);
                    }
                    System.Threading.Thread.Sleep(100);
                }
            }
        }

        public static int GetCounterValue(string ipAddress, int port)
        {
<<<<<<< HEAD
            byte[] receiveBuffer = new byte[4];

            int retryCount = 25;

=======
            Console.WriteLine("GetCounterValue Start");
            byte[] receiveBuffer = new byte[4];

            int retryCount = 5;
>>>>>>> Ethernet_Testing
            int attempts = 0;
            bool success = false;
            while (attempts < retryCount && !success)
            {
                try
                {
                    using (TcpClient client = new TcpClient(ipAddress, port))
                    {
                        NetworkStream stream = client.GetStream();
                        stream.Read(receiveBuffer, 0, receiveBuffer.Length);
                        success = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Attempts: " + attempts);
                    attempts++;
                    if (attempts >= retryCount)
                    {
                        throw new Exception($"Failed to receive data after {retryCount} attempts.", ex);
                    }
                    System.Threading.Thread.Sleep(100);
                }
            }

            Array.Reverse(receiveBuffer);
            return BitConverter.ToInt32(receiveBuffer, 0);
        }
    }
}