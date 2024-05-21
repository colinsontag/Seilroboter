using System.Collections.Generic;
using System.Xml.Serialization;
using MathNet.Spatial;
using MathNet.Spatial.Euclidean;
namespace ConfigManger
{
    [XmlRoot("RobotConfig")]
    public class RobotConfig
    {
        [XmlElement("Drive1Position")]
        public Point3D Drive1Position { get; set; }

        [XmlElement("Drive2Position")]
        public Point3D Drive2Position { get; set; }

        [XmlElement("Drive3Position")]
        public Point3D Drive3Position { get; set; }
    }
}
