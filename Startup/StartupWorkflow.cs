using ConfigManger;
using Machine;
using MathNet.Spatial.Euclidean;
using Startup;

namespace Startup
{
    public class StartupWorkflow
    {
        public static void Start()
        {
            var drives = new List<Drive>();
            var pointList = new List<MachinePoint>();
            string pathToConfig = "";
            string pathToPointList = "";

            InitProcessWorkflow.Start(pathToConfig, pathToPointList, out drives, out pointList);
            SingleTCPProcess.Start(drives, pointList);
        }


    }
}
