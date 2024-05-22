using ConfigManger;
using Microsoft.VisualBasic;

namespace Workflow
{
    public class Startup
    {
        public static void Start()
        {
            RobotConfig InitialRobotConfig = ConfigTools.LoadFromXML(@"W:\Seilroboter\ConfigManger\RobotConfig.xml");

            

        }
    }
}
