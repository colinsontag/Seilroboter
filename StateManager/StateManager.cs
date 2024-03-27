using System;
using System.IO;
using System.Text.Json;
using MathNet.Spatial.Euclidean;

namespace StateManager
{
    public class StateManager
    {
        private readonly string stateFilePath;

        public StateManager(string filePath)
        {
            stateFilePath = filePath;
        }

        public void SaveState(Point3D position, float ropeLength)
        {
            StateVariables state = new StateVariables
            {
                Position = position,
                RopeLength = ropeLength
            };

            // Schreibvorgang in Blöcken optimieren
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true // Optional: Für formatierte JSON-Ausgabe
            };

            using (FileStream fileStream = new FileStream(stateFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough))
            {
                JsonSerializer.SerializeAsync(fileStream, state, options).Wait();
            }
        }
        public StateVariables LoadState()
        {
            if (File.Exists(stateFilePath))
            {
                string json = File.ReadAllText(stateFilePath);               
                if (JsonSerializer.Deserialize<StateVariables>(json) != null)
                {
                    return JsonSerializer.Deserialize<StateVariables>(json);
                }
            }

            // Falls die Datei nicht existiert, leere Standardeinstellungen zurückgeben
            return new StateVariables();
        }
    }

    public class StateVariables
    {
        public Point3D Position { get; set; }
        public float RopeLength { get; set; }
    }
}
