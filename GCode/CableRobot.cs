using System;
using System.Collections.Generic;
using System.Numerics;

namespace GCode
{
    public class CableRobot
    {
        private List<Vector3> drivePositions;

        public CableRobot(List<Vector3> initialDrivePositions)
        {
            this.drivePositions = initialDrivePositions;
        }

        public void SetDrivePositions(List<Vector3> newDrivePositions)
        {
            this.drivePositions = newDrivePositions;
        }

        public List<float> CalculateCableLengths(Vector3 targetPosition)
        {
            List<float> cableLengths = new List<float>();

            foreach (Vector3 drivePosition in drivePositions)
            {
                float distance = Vector3.Distance(drivePosition, targetPosition);
                cableLengths.Add(distance);
            }

            return cableLengths;
        }
    }
}
