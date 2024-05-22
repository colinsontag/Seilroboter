using System.Collections.Generic;
using System.Xml.Serialization;
using MathNet.Spatial;
using MathNet.Spatial.Euclidean;
namespace ConfigManger
{
    [XmlRoot("RobotConfig")]
    public class RobotConfig
    {
        [XmlElement("Drive1MountPosition")]
        public Point3D Drive1MountPosition { get; set; }

        [XmlElement("Drive2MountPosition")]
        public Point3D Drive2MountPosition { get; set; }

        [XmlElement("Drive3MountPosition")]
        public Point3D Drive3MountPosition { get; set; }
    }
}
