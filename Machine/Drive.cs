﻿using MathNet.Spatial.Euclidean;

namespace Machine
{
    public class Drive
    {
        public string Name { get; set; }
        public Point3D MountPosition { get; set; }
        public double UnrolledCableLength { get; set; }
        public int EthernetIP { get; set; }

        public Drive(string name, Point3D mountPosition, float unrolledCableLength, int ethernetIP)
        {
            MountPosition = mountPosition;
            Name = name;
            UnrolledCableLength = unrolledCableLength;
            EthernetIP = ethernetIP;
        }
    }
}