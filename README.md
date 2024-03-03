# Seilroboter Steuerung mit C# und Raspberry Pi

Dieses Repository enthält den C#-Code für die Steuerung eines Seilroboters mit einem Raspberry Pi 4. Der Seilroboter wird durch drei Motoren angetrieben, die Seile ziehen und somit die Position des Roboters beeinflussen. Ziel ist es, den Punkt, an dem die Seile zusammenlaufen (wo die Last befestigt ist), kartesisch zu steuern.

## Funktionalitäten

- **Inverse Kinematik:** Implementierung der inversen Kinematik, um die Gelenkwinkel der Motoren basierend auf einer gewünschten Zielposition zu berechnen.

- **Controller-Interface:** Eine Benutzeroberfläche (UI) in C#, um den Seilroboter kartesisch zu steuern. Dies ermöglicht die Eingabe von relativen Bewegungen vom aktuellen Punkt aus.

- **Simulation:** Eine Möglichkeit zur Simulation der Bewegungen des Seilroboters, ohne tatsächlich die Motoren zu bewegen. Dies erleichtert die Validierung und Fehlersuche.

## Voraussetzungen

- Raspberry Pi 4 mit Raspbian (oder einem vergleichbaren Betriebssystem).
- .NET 8.0 (oder eine neuere Version) auf dem Raspberry Pi installiert.
- (Optional) Visual Studio auf einem Entwicklungsrechner für die Codeentwicklung.


