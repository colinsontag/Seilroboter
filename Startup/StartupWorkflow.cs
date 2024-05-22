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
            var tCP = new MachinePoint();
            InitProcessWorkflow.Start(out drives, out tCP);
            
            StartProcessWorkflow.Start(drives, tCP);
        }


    }
}
