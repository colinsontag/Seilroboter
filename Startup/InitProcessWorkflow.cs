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
        internal static void Start(string pathToConfigFile, string pathToPointList, out List<Drive> drives, out List<MachinePoint> pointList)
        {
            //RobotConfig InitialRobotConfig = ConfigTools.LoadFromXML(@pathToConfigFile);
            RobotConfig InitialRobotConfig = ConfigTools.LoadFromXML(@"/home/colin/Documents/Seilroboter/Seilroboter/ConfigManger/RobotConfig.xml");
            drives = new List<Drive>
            {
                new Drive("Drive1", InitialRobotConfig.Drive1MountPosition, 0, 8),
                new Drive("Drive2", InitialRobotConfig.Drive2MountPosition, 0, 9),
                new Drive("Drive3", InitialRobotConfig.Drive3MountPosition, 0, 10)
            };
            pointList = new List<MachinePoint>();
        }
    }
}