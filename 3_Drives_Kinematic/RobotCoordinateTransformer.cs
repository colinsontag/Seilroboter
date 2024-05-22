using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Euclidean;

namespace _3_Drives_Kinematic
{

    public static class RobotCoordinateTransformer
    {
        public struct RobotPositon
        {
            public double x;
            public double y;
            public double z;

        }
       

        private static Point3D pointTransform(Point3D measuringPoint, Matrix<double> transformationMatrix)
        {
            var homogeneousPoint = ToHomogeneousCoordinates(measuringPoint);
            var transformedPoint = transformationMatrix * homogeneousPoint;
            return new Point3D(transformedPoint[0, 0], transformedPoint[1, 0], transformedPoint[2, 0]);
        }

        private static Matrix<double> matrixTransform(double x, double y, double z, double a, double b, double c)
        {
            var rotation = Matrix<double>.Build.DenseOfArray(new double[,] {
                { Math.Cos(b)*Math.Cos(c), -Math.Cos(b)*Math.Sin(c), Math.Sin(b) },
                { Math.Cos(a)*Math.Sin(c) + Math.Sin(a)*Math.Sin(b)*Math.Cos(c), Math.Cos(a)*Math.Cos(c) - Math.Sin(a)*Math.Sin(b)*Math.Sin(c), -Math.Sin(a)*Math.Cos(b) },
                { Math.Sin(a)*Math.Sin(c) - Math.Cos(a)*Math.Sin(b)*Math.Cos(c), Math.Sin(a)*Math.Cos(c) + Math.Cos(a)*Math.Sin(b)*Math.Sin(c), Math.Cos(a)*Math.Cos(b) }
            });
            var translation = DenseMatrix.OfArray(new double[,]
            {
            {1, 0, 0, x},
            {0, 1, 0, y},
            {0, 0, 1, z},
            {0, 0, 0, 1}
            });
            var transform = translation * rotation;
            return transform;
        }
        private static Matrix<double> ToHomogeneousCoordinates(Point3D point)
        {
            return DenseMatrix.OfArray(new double[,]
            {
            {point.X},
            {point.Y},
            {point.Z},
            {1}
            });
        }
    }
}


