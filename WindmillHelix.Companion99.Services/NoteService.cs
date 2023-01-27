using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WindmillHelix.Companion99.Common;
using WindmillHelix.Companion99.Common.Threading;
using WindmillHelix.Companion99.Services.Events;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.Services
{
    public class NoteService : INoteService
    {
        private Dictionary<string, NoteItem> _notes = new Dictionary<string, NoteItem>();
        private bool _isInitialized = false;
        private object _lock = new object();

        private string _fileName;
        private readonly IEventService _eventService;

        public NoteService(IEventService eventService)
        {
            _fileName = Path.Combine(FileHelper.GetDataFolder(), "FriendNotes.xml");
            _eventService = eventService;
        }

        private void EnsureInitialized()
        {
            if(!_isInitialized)
            {
                lock(_lock)
                {
                    if(!_isInitialized)
                    {
                        var serializer = new XmlSerializer(typeof(NoteItem[]));
                        if(File.Exists(_fileName))
                        {
                            using (var fs = new FileStream(_fileName, FileMode.Open))
                            {
                                var notes = (NoteItem[])serializer.Deserialize(fs);
                                foreach(var note in notes)
                                {
                                    note.CharacterName = FixCharacterCasing(note.CharacterName);
                                    var key = GetKey(note.ServerName, note.CharacterName);
                                    _notes.Add(key, note);
                                }
                            }
                        }

                        _isInitialized = true;
                    }
                }
            }
        }

        private string GetKey(string serverName, string characterName)
        {
            string key = serverName.ToLowerInvariant() + "." + characterName.ToLowerInvariant();
            return key;
        }

        public string GetNote(string serverName, string characterName)
        {
            EnsureInitialized();
            var key = GetKey(serverName, characterName);
            var result = _notes.ContainsKey(key) ? _notes[key].Note : null;
            return result;
        }

        public void SetNote(string serverName, string characterName, string note)
        {
            var key = GetKey(serverName, characterName);
            if (_notes.ContainsKey(key))
            {
                _notes[key].Note = note;
            }
            else
            {
                var item = new NoteItem()
                {
                    ServerName = serverName,
                    CharacterName = FixCharacterCasing(characterName),
                    Note = note
                };

                _notes.Add(key, item);
            }

            var serializer = new XmlSerializer(typeof(NoteItem[]));

            using (var fs = new FileStream(_fileName, FileMode.Create))
            {
                serializer.Serialize(fs, _notes.Values.ToArray());
            }

            AsyncHelper.RunSynchronously(() => _eventService.Raise<NotesChangedEvent>());
        }

        public IReadOnlyCollection<NoteItem> GetAllNotes()
        {
            EnsureInitialized();
            return _notes.Values;
        }

        private string FixCharacterCasing(string characterName)
        {
            var fixedName = characterName.Substring(0, 1).ToUpper() + characterName.Substring(1).ToLower();
            return fixedName;
        }
    }
}
