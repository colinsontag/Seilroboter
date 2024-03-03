using System;
using System.Threading;
using System.Device.I2c;

class Program
{
    static void Main(string[] args)
    {
        // I2C-Adresse des Arduino
        const int ArduinoI2CAddress = 9;

        // I2C-Busnummer auf dem Raspberry Pi
        const int I2CBusId = 1;

        // Erstellen Sie eine I2cConnectionSettings-Instanz für die Verbindung zum Arduino
        var connectionSettings = new I2cConnectionSettings(I2CBusId, ArduinoI2CAddress);

        // Erstellen Sie eine I2cDevice-Instanz für die Kommunikation über I2C
        using (I2cDevice i2cDevice = I2cDevice.Create(connectionSettings))
        {
            Console.WriteLine("Warte auf Daten...");

            while (true)
            {
                byte[] receiveBuffer = new byte[4];

                // Lesen Sie Daten vom Arduino
                i2cDevice.Read(receiveBuffer);

                // Umkehren der Reihenfolge der Bytes
                Array.Reverse(receiveBuffer);

                // Konvertieren Sie die empfangenen Daten in eine 32-Bit-Ganzzahl
                int counterValue = BitConverter.ToInt32(receiveBuffer, 0);

                // Anzeigen der empfangenen Daten
                Console.WriteLine("Empfangene Daten: " + counterValue);

                // Verzögerung zwischen den Lesevorgängen
                Thread.Sleep(1000);
            }
        }
    }
}
