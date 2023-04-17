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
    public class SpellsService : ISpellsService
    {
        private object _lock = new object();
        private bool _isInitialized = false;

        private readonly Dictionary<EverQuestClass, IReadOnlyCollection<SpellItem>> _classSpells 
            = new Dictionary<EverQuestClass, IReadOnlyCollection<SpellItem>>();

        public SpellsService()
        {

        }

        public EverQuestClass? DetermineClassFromSpellbook(IReadOnlyCollection<SpellbookItem> spellbookItems)
        {
            EnsureInitialized();

            if(spellbookItems == null || spellbookItems.Count == 0)
            {
                return null;
            }

            var classes = Enum.GetValues<EverQuestClass>();
            var matches = new List<Tuple<EverQuestClass, double>>();

            foreach(var item in classes)
            {
                if(!_classSpells.ContainsKey(item) || _classSpells[item].Count == 0)
                {
                    continue;
                }

                var spells = _classSpells[item];

                var count = spellbookItems.Count(x => spells.Any(s => SpellUtil.SpellNameEquals(s.Name, x.SpellName) && s.Level == x.Level));
                var percent = (100.0 * count) / spellbookItems.Count;

                matches.Add(Tuple.Create(item, percent));
            }

            var ordered = matches.OrderByDescending(x => x.Item2);
            if(ordered.First().Item2 > 85)
            {
                var selectedClass = ordered.First().Item1;
                var classSpells = _classSpells[selectedClass];
                var inBookButNotList = spellbookItems
                    .Where(x => ! classSpells.Any(s => !SpellUtil.SpellNameEquals(s.Name, x.SpellName)))
                    .ToList();

                if(inBookButNotList.Count > 0)
                {
                }    

                return selectedClass;
            }

            return null;
        }

        public IReadOnlyCollection<SpellItem> GetSpells(EverQuestClass everQuestClass)
        {
            if(!_classSpells.ContainsKey(everQuestClass))
            {
                return Array.Empty<SpellItem>();
            }

            return _classSpells[everQuestClass];
        }

        private void EnsureInitialized()
        {
            if(!_isInitialized)
            {
                lock(_lock)
                {
                    if(_isInitialized)
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

            var resourceName = typeof(ServicesAssemblyLocator).Namespace + ".Resources.Spells.xml";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                xml = reader.ReadToEnd();
            }

            var serializer = new XmlSerializer(typeof(SpellItemContainer));
            var container = (SpellItemContainer)serializer.Deserialize(new StringReader(xml));

            foreach(var className in classNames)
            {
                var everQuestClass = (EverQuestClass)Enum.Parse(typeof(EverQuestClass), className);
                var spells = container.Spells.Where(x => x.Class == everQuestClass).ToList();
                _classSpells.Add(everQuestClass, spells);
            }
        }

        [XmlRoot(ElementName = "Spells")]
        public class SpellItemContainer
        {
            [XmlElement(ElementName = "Spell")]
            public SpellItem[] Spells { get; set; }
        }
    }
}
