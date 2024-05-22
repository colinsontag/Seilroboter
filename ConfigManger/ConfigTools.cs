using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConfigManger
{
    public class ConfigTools
    {
        public static RobotConfig LoadFromXML(string filepath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(RobotConfig));
            FileStream fileStream = new FileStream(filepath, FileMode.Open);
            return (RobotConfig)serializer.Deserialize(fileStream);
        }
    }
}
