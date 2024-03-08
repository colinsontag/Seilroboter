using System;
using System.Threading;
using System.Device.I2c;

class Program
{
    static void Main(string[] args)
    {
        // I2C-Busnummer auf dem Raspberry Pi
        const int I2CBusId = 1;

        // Arduino I2C Addresses
        int[] arduinoAddresses = { 9, 10, 11 };
        while (true)
        {            
        
            // Schleife durch die verschiedenen Arduinos
            foreach (int address in arduinoAddresses)
            {
                // Erstellen Sie eine I2cConnectionSettings-Instanz für die Verbindung zum Arduino
                var connectionSettings = new I2cConnectionSettings(I2CBusId, address);

                // Erstellen Sie eine I2cDevice-Instanz für die Kommunikation über I2C
                using (I2cDevice i2cDevice = I2cDevice.Create(connectionSettings))
                {
                    Console.WriteLine($"Warte auf Daten von Arduino {address}...");

                    
                    byte[] receiveBuffer = new byte[4];

                    // Lesen Sie Daten vom aktuellen Arduino
                    i2cDevice.Read(receiveBuffer);

                    // Umkehren der Reihenfolge der Bytes
                    Array.Reverse(receiveBuffer);

                    // Konvertieren Sie die empfangenen Daten in eine 32-Bit-Ganzzahl
                    int counterValue = BitConverter.ToInt32(receiveBuffer, 0);

                    // Anzeigen der empfangenen Daten
                    Console.WriteLine($"Empfangene Daten von Arduino {address}: {counterValue}");

                    // Verzögerung zwischen den Lesevorgängen
                    Thread.Sleep(500);
                    
                }
            }
        }
    }
}
