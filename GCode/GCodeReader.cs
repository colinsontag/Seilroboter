using System;
using System.Collections.Generic;
using System.IO;

namespace GCode
{
    public class GCodeReader
    {
        public List<string> ReadGCodeFile(string filePath)
        {
            List<string> gCodeLines = new List<string>();

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        gCodeLines.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            return gCodeLines;
        }
    }

}
