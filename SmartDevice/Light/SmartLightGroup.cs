using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SmartDevice.Light
{
    public class SmartLightGroup
    {
        [XmlAttribute]
        public string Name;
        [XmlArrayItem("LightId")]
        public uint[] LightIds;

        public SmartLightGroup()
        {
        }
    }
}
