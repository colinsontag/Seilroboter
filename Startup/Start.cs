using ConfigManger;
using Machine;
using MathNet.Spatial.Euclidean;

namespace Workflow
{
    public class Startup
    {
        public static void Start()
        {
            RobotConfig InitialRobotConfig = ConfigTools.LoadFromXML(@"W:\Seilroboter\ConfigManger\RobotConfig.xml");

            var drives = new List<Drive>
            {
                new Drive("Drive1", InitialRobotConfig.Drive1MountPosition, 0),
                new Drive("Drive2", InitialRobotConfig.Drive2MountPosition, 0),
                new Drive("Drive3", InitialRobotConfig.Drive3MountPosition, 0)
            };

            MachinePoint tCP = new MachinePoint
            {
                Name = "TCP",
                Position = new Point3D(0, 0, 0)
            };


        }
    }
}
