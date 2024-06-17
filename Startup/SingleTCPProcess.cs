﻿using ConfigManger;
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
            const int I2CBusIdController = 1;
            bool motorOn = false;
            bool motorPlus = false;
            List<I2cDevice> i2cDevices = new List<I2cDevice>();
            for (int i = 0; i < drives.Count - 1; i++)
            {
                var connectionSettings = new I2cConnectionSettings(I2CBusIdController, drives[i].I2CBusId);
                var device = I2cDevice.Create(connectionSettings);
                i2cDevices.Add(device);
                drives[i].UnrolledCableLength = RefreshDrive(device, device.ConnectionSettings.DeviceAddress, device.ConnectionSettings.BusId, angleDistance);
            }

            foreach (Drive drive in drives)
            {
                Console.WriteLine(drive.UnrolledCableLength.ToString());
            }

            double cableLenghtToReach = 150;
            while (cableLenghtToReach != 666)
            {
                for (int i = 0; i < i2cDevices.Count -1; i++)
                {
                    while (Math.Abs(drives[i].UnrolledCableLength - cableLenghtToReach) >= angleDistance)
                    {
                        ChangeDriveCabelLenght(drives, angleDistance, I2CBusIdController, i2cDevices, cableLenghtToReach, i);
                    }
                    motorOn = false;
                    SendMotor(i2cDevices.ToArray()[i], motorOn, motorPlus);
                }                
                Console.WriteLine("Neue Laenge Eingeben");
                cableLenghtToReach = Convert.ToDouble(Console.ReadLine());
            }
        }

        

       
    }
}
