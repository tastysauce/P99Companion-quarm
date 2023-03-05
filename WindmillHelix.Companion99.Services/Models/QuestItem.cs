using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WindmillHelix.Companion99.Services.Models
{
    public class QuestItem
    {
        public QuestItem()
        {
            Behavior = SubQuestItemBehavior.All;
        }

        [XmlAttribute]
        public int ItemId { get; set; }

        [XmlAttribute]
        public string ItemName { get; set; }

        [XmlAttribute]
        public SubQuestItemBehavior Behavior { get; set; }

        [XmlAttribute]
        public string Url { get; set; }

        public QuestItem[] SubQuestItems { get; set; }
    }
}
