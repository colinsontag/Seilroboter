using System.Collections.Generic;
using MathNet.Spatial;
using MathNet.Spatial.Euclidean;
namespace ConfigManger
{
    public class ConfigSettings
    {
        public required Dictionary<string, Point3D> Drives { get; set; }
    }
}
