using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WindmillHelix.Companion99.Common;

namespace WindmillHelix.Companion99.Services.Models
{
    public class SpellItem
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public int Level { get; set; }

        [XmlAttribute("Class")]
        public EverQuestClass Class { get; set; }
    }
}
