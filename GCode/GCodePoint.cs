using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Spatial.Euclidean;

namespace GCode
{
    public class GCodePoint
    {
        public Point3D Position { get; set; }
        public int FeedRate { get; set; }

        public GCodePoint(Point3D position, int feedRate)
        {
            Position = position;
            FeedRate = feedRate;
        }
    }
}
