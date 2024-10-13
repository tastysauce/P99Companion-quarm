using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WindmillHelix.Companion99.Common;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.Services
{
    public class LastZoneService : ILastZoneService
    {
        private List<CharacterZone> _items;
        private bool _isInitialized = false;
        private object _lock = new object();
        private string _fileName;

        public LastZoneService()
        {
            _items = new List<CharacterZone>();
            _fileName = Path.Combine(FileHelper.GetDataFolder(), "LastZone.xml");
        }

        public IReadOnlyCollection<CharacterZone> GetLastZones()
        {
            EnsureInitialized();
            return _items;
        }

        public void SetLastZone(string serverName, string characterName, string zoneName, string account)
        {
            UpdateEntry(
                serverName,
                characterName,
                a =>
                {
                    a.ZoneName = zoneName;
                    a.Account = account.ToLowerInvariant();
                });
        }

        public void SetSkyCorpseDate(string serverName, string characterName, DateTime dateOfDeath)
        {
            UpdateEntry(
                serverName,
                characterName,
                a =>
                {
                    a.SkyCorpseDate = dateOfDeath;
                });
        }

        private void UpdateEntry(string serverName, string characterName, Action<CharacterZone> action)
        {
            var item = _items.SingleOrDefault(
                x => x.ServerName.EqualsIngoreCase(serverName)
                && x.CharacterName.EqualsIngoreCase(characterName));

            if (item == null)
            {
                item = new CharacterZone
                {
                    ServerName = serverName,
                    CharacterName = FixCharacterCasing(characterName)
                };

                _items.Add(item);
            }

            action(item);

            var serializer = new XmlSerializer(typeof(CharacterZone[]));
            using (var fs = new FileStream(_fileName, FileMode.Create))
            {
                serializer.Serialize(fs, _items.ToArray());
            }
        }

        private string FixCharacterCasing(string characterName)
        {
            var fixedName = characterName.Substring(0, 1).ToUpper() + characterName.Substring(1).ToLower();
            return fixedName;
        }

        private void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                lock (_lock)
                {
                    if (!_isInitialized)
                    {
                        var serializer = new XmlSerializer(typeof(CharacterZone[]));
                        if (File.Exists(_fileName))
                        {
                            using (var fs = new FileStream(_fileName, FileMode.Open))
                            {
                                var items = (CharacterZone[])serializer.Deserialize(fs);
                                foreach (var item in items)
                                {
                                    item.CharacterName = FixCharacterCasing(item.CharacterName);
                                }

                                _items = items.ToList();
                            }
                        }

                        _isInitialized = true;
                    }
                }
            }
        }
    }
}
