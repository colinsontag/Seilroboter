using Machine;
using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I2C_Receive
{
    public static class I2CConnectUtil
    {
        public static I2cDevice CreateI2CDevice(List<Drive> drives, int I2CBusIdController, int i)
        {
            var connectionSettings = new I2cConnectionSettings(I2CBusIdController, drives[i].I2CBusId);
            var device = I2cDevice.Create(connectionSettings);
            return device;
        }
    }
}
