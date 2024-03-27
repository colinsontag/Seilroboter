using System;
using System.Collections.Generic;

namespace GCode
{
    public abstract class GCodeCommand
    {
        public string Command { get; protected set; }
        public Dictionary<string, string> Parameters { get; protected set; }

        protected GCodeCommand(string command)
        {
            Command = command;
            Parameters = new Dictionary<string, string>();
        }

        public abstract void ParseParameters(string line);
    }

    public class G28Command : GCodeCommand
    {
        public bool X { get; private set; }
        public bool Y { get; private set; }
        public bool Z { get; private set; }

        public G28Command() : base("G28") { }

        public override void ParseParameters(string line)
        {
            // Parse parameters specific to G28 command
        }
    }

   

    public class G2Command : GCodeCommand
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }
        public float J { get; private set; }
        public float E { get; private set; }
        public float F { get; private set; }

        public G2Command() : base("G2") { }

        public override void ParseParameters(string line)
        {
            // Parse parameters specific to G2 command
        }
    }

    public class M107Command : GCodeCommand
    {
        public M107Command() : base("M107") { }

        public override void ParseParameters(string line)
        {
            // Parse parameters specific to M107 command
        }
    }
}
