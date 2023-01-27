using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Common;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.Services
{
    public class SpellbookService : ISpellbookService
    {
        private const string SpellbookFileNameFilter = "*-Spellbook.txt";
        private readonly IConfigurationService _configurationService;

        public SpellbookService(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public FileSystemWatcher CreateSpellbookChangedWatcher()
        {
            var watcher = new FileSystemWatcher(_configurationService.EverQuestFolder);

            watcher.Filter = SpellbookFileNameFilter;
            watcher.IncludeSubdirectories = false;
            watcher.NotifyFilter = NotifyFilters.LastWrite;

            return watcher;
        }

        public IReadOnlyCollection<SpellbookItem> GetSpellbookItems()
        {
            var files = Directory.GetFiles(_configurationService.EverQuestFolder, SpellbookFileNameFilter);

            var results = new List<SpellbookItem>();
            foreach (var file in files)
            {
                var items = GetSpellbookItems(file);
                results.AddRange(items);
            }

            return results;
        }

        private string[] GetFileLines(string fileName, int maxRetries)
        {
            int tryCount = 0;
            while (tryCount < maxRetries)
            {
                try
                {
                    var lines = File.ReadAllLines(fileName);
                    return lines;
                }
                catch (Exception thrown)
                {
                    Thread.Sleep(200);
                    tryCount++;
                }
            }

            return new string[0];
        }

        private IReadOnlyCollection<SpellbookItem> GetSpellbookItems(string fileName)
        {
            var spells = new List<SpellbookItem>();

            var lines = GetFileLines(fileName, 5);

            if (lines.Length == 0)
            {
                return spells;
            }

            var fileInfo = new FileInfo(fileName);
            var characterName = fileInfo.Name.Replace("-Spellbook.txt", string.Empty, StringComparison.InvariantCultureIgnoreCase);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var columns = line.Split('\t');
                var level = int.Parse(columns[0]);
                {
                    var item = new SpellbookItem
                    {
                        CharacterName = characterName,
                        Level = level,
                        SpellName = columns[1]
                    };

                    spells.Add(item);
                }
            }

            return spells;
        }
    }
}
