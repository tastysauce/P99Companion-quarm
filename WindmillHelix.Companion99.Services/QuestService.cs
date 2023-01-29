using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WindmillHelix.Companion99.Common;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.Services
{
    public class QuestService : IQuestService
    {
        private object _lock = new object();
        private bool _isInitialized = false;

        private readonly Dictionary<int, QuestItem> _questItems = new Dictionary<int, QuestItem>();

        public QuestService()
        {
        }

        private void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                lock (_lock)
                {
                    if (_isInitialized)
                    {
                        return;
                    }

                    PerformInitialization();
                    _isInitialized = true;
                }
            }
        }

        private void PerformInitialization()
        {
            var classNames = Enum.GetNames(typeof(EverQuestClass));
            string xml;
            var assembly = Assembly.GetExecutingAssembly();
            var names = assembly.GetManifestResourceNames();

            var resourceName = typeof(ServicesAssemblyLocator).Namespace + ".Resources.Quests.xml";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                xml = reader.ReadToEnd();
            }

            var serializer = new XmlSerializer(typeof(QuestItemContainer));
            var container = (QuestItemContainer)serializer.Deserialize(new StringReader(xml));
            foreach(var item in container.Quests)
            {
                _questItems.Add(item.ItemId, item);
            }
        }

        public QuestItem GetQuestItem(int itemId)
        {
            EnsureInitialized();

            return _questItems[itemId];
        }

        [XmlRoot(ElementName = "Quests")]
        public class QuestItemContainer
        {
            [XmlElement(ElementName = "QuestItem")]
            public QuestItem[] Quests { get; set; }
        }
    }
}
