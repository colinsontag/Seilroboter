using GCode;
using System.Numerics;
using System.Windows;


public class CableRobotController
{
    private CableRobot robot;

    public CableRobotController(CableRobot robot)
    {
        this.robot = robot;
    }

    public bool ValidateCableLengths(Vector3 targetPosition, float minLength, float maxLength)
    {
        List<float> cableLengths = robot.CalculateCableLengths(targetPosition);

        foreach (float length in cableLengths)
        {
            Console.WriteLine("Cable length: " + length);
        }

        return true;
    }
    public static void Main()
    {
        List<Vector3> drivePositions = new List<Vector3>();
        drivePositions.Add(new Vector3(0, 0, 0));
        drivePositions.Add(new Vector3(20, 20, 20));
        drivePositions.Add(new Vector3(0, 10, 0));
        CableRobot robot = new CableRobot(drivePositions);
        CableRobotController controller = new CableRobotController(robot);
        controller.ValidateCableLengths(new Vector3(10, 10, 10), 2, 2);
    }
}

