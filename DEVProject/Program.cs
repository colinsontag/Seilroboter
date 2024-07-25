using System;
using System.Device.I2c;
using System.Threading.Tasks;

internal class Program
{
    private static void Main(string[] args)
    {
        // I2C-Bus-Nummer (1 für Raspberry Pi)
        const int busId = 1;

        // I2C-Adressen der Arduinos
        const int arduinoAddress1 = 0x08;
        const int arduinoAddress2 = 0x09;
        const int arduinoAddress3 = 0x0A;

        // Initialisiere I2C-Verbindung zu jedem Arduino
        var i2cSettings1 = new I2cConnectionSettings(busId, arduinoAddress1);
        var i2cDevice1 = I2cDevice.Create(i2cSettings1);

        var i2cSettings2 = new I2cConnectionSettings(busId, arduinoAddress2);
        var i2cDevice2 = I2cDevice.Create(i2cSettings2);

        var i2cSettings3 = new I2cConnectionSettings(busId, arduinoAddress3);
        var i2cDevice3 = I2cDevice.Create(i2cSettings3);

        // Beispiel: Daten an Arduino senden und empfangen
        byte[] writeBuffer = new byte[] { 0x01 }; // Beispiel-Befehl
        byte[] readBuffer = new byte[1];

        // Daten an Arduino 1 senden und Antwort lesen
        i2cDevice1.Write(writeBuffer);
        i2cDevice1.Read(readBuffer);
        Console.WriteLine($"Arduino 1 antwortet: {readBuffer[0]}");

        // Daten an Arduino 2 senden und Antwort lesen
        i2cDevice2.Write(writeBuffer);
        i2cDevice2.Read(readBuffer);
        Console.WriteLine($"Arduino 2 antwortet: {readBuffer[0]}");

        // Daten an Arduino 3 senden und Antwort lesen
        i2cDevice3.Write(writeBuffer);
        i2cDevice3.Read(readBuffer);
        Console.WriteLine($"Arduino 3 antwortet: {readBuffer[0]}");
    }
}