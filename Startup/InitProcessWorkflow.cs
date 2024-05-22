using ConfigManger;
using Machine;
using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Startup
{
    internal static class InitProcessWorkflow
    {
        internal static void Start(out List<Drive> drives,out MachinePoint tCP)
        {
            RobotConfig InitialRobotConfig = ConfigTools.LoadFromXML(@"W:\Seilroboter\ConfigManger\RobotConfig.xml");

            drives = new List<Drive>
            {
                new Drive("Drive1", InitialRobotConfig.Drive1MountPosition, 0),
                new Drive("Drive2", InitialRobotConfig.Drive2MountPosition, 0),
                new Drive("Drive3", InitialRobotConfig.Drive3MountPosition, 0)
            };

            tCP = new MachinePoint
            {
                Name = "TCP",
                Position = new Point3D(0, 0, 0)
            };
        }
    }
}
