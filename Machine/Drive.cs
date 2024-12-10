using MathNet.Spatial.Euclidean;
using System.Net;

namespace Machine
{
    public class Drive
    {
        public string Name { get; set; }
        public Point3D MountPosition { get; set; }
        public double UnrolledCableLength { get; set; }
        public string EthernetIP { get; set; }
        public double AngleDistance { get; set; }

        public Drive(string name, Point3D mountPosition, float unrolledCableLength, string ethernetIP, double angleDistance)
        {
            MountPosition = mountPosition;
            Name = name;
            UnrolledCableLength = unrolledCableLength;
            EthernetIP = ethernetIP;
            AngleDistance = angleDistance;
        }
    }
}