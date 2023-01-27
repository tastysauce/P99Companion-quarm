using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindmillHelix.Companion99.App.Models;
using WindmillHelix.Companion99.Services;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.App
{
    /// <summary>
    /// Interaction logic for SpellbookControl.xaml
    /// </summary>
    public partial class SpellbookControl : UserControl
    {
        private IReadOnlyCollection<SpellbookItem> _items;
        private readonly ISpellbookService _spellbookService;
        private readonly ISpellsService _spellsService;
        private readonly FileSystemWatcher _watcher;

        public SpellbookControl()
        {
            InitializeComponent();

            _spellbookService = DependencyInjector.Resolve<ISpellbookService>();
            _spellsService = DependencyInjector.Resolve<ISpellsService>();

            var items = _spellbookService.GetSpellbookItems();

            _items = items;

            var characterNames = _items.Select(x => x.CharacterName).Distinct().OrderBy(x => x).Select(x => new ComboItem<string, string> { Value = x, Display = x });

            var characterItems = new List<ComboItem<string, string>>();
            characterItems.Add(new ComboItem<string, string> { Value = string.Empty, Display = "-- Select From List --" });
            characterItems.AddRange(characterNames);

            CharacterComboBox.ItemsSource = characterItems;
            CharacterComboBox.SelectedIndex = 0;

            HaveStatusComboBox.SelectedIndex = 0;

            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            CharacterComboBox.SelectionChanged += CharacterComboBox_SelectionChanged;
            HaveStatusComboBox.SelectionChanged += HaveStatusComboBox_SelectionChanged;

            _watcher = _spellbookService.CreateSpellbookChangedWatcher();
            _watcher.Changed += HandleSpellbookFilesChanged;
            _watcher.Created += HandleSpellbookFilesChanged;
            _watcher.EnableRaisingEvents = true;

            SetItemSource();
        }


        private void HandleSpellbookFilesChanged(object sender, FileSystemEventArgs e)
        {
            var items = _spellbookService.GetSpellbookItems();

            _items = items;

            var characterNames = _items.Select(x => x.CharacterName).Distinct().OrderBy(x => x).Select(x => new ComboItem<string, string> { Value = x, Display = x });

            var characterItems = new List<ComboItem<string, string>>();
            characterItems.Add(new ComboItem<string, string> { Value = string.Empty, Display = "-- All --" });
            characterItems.AddRange(characterNames);

            Dispatcher.Invoke(() =>
            {
                var currentCharacterValue = (string)CharacterComboBox.SelectedValue;

                CharacterComboBox.ItemsSource = characterItems;
                if (currentCharacterValue != null)
                {
                    CharacterComboBox.SelectedItem = characterItems.Single(x => x.Value == currentCharacterValue);
                }

                SetItemSource();
            });
        }

        private void CharacterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetItemSource();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetItemSource();
        }

        private void HaveStatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetItemSource();
        }

        private void SetItemSource()
        {
            var searchText = SearchTextBox.Text;

            var filtered = _items;

            if (string.IsNullOrWhiteSpace((string)CharacterComboBox.SelectedValue))
            {
                filtered = filtered.Where(x => false).ToList();
            }

            var toDisplay = new List<SpellDisplayModel>();
            var characterClass = _spellsService.DetermineClassFromSpellbook(filtered);
            if (characterClass.HasValue)
            {
                var possibleSpells = _spellsService.GetSpells(characterClass.Value);
                toDisplay = possibleSpells.Select(x => new SpellDisplayModel
                {
                    Level = x.Level,
                    SpellName = x.Name,
                    HasSpell = filtered.Any(f => f.SpellName.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase))
                }).ToList();
            }
            else
            {
                toDisplay = filtered.Select(x => new SpellDisplayModel
                {
                    Level = x.Level,
                    SpellName = x.SpellName
                }).ToList();
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                toDisplay = toDisplay.Where(x => x.SpellName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }

            var haveSelection = HaveStatusComboBox.SelectedIndex;
            if(haveSelection == 1)
            {
                toDisplay = toDisplay.Where(x => x.HasSpell).ToList();
            }
            else if(haveSelection == 2)
            {
                toDisplay = toDisplay.Where(x => !x.HasSpell).ToList();
            }

            toDisplay = toDisplay.OrderBy(x => x.Level).ThenBy(x => x.SpellName).ToList();
            ItemsListView.ItemsSource = toDisplay;
        }

        protected void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var spell = ((ListViewItem)sender).Content as SpellDisplayModel;
            if (spell == null)
            {
                return;
            }

            var url = "https://wiki.project1999.com/" + HttpUtility.UrlEncode(spell.SpellName.Replace(" ", "_"));

            var start = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };

            Process.Start(start);
        }
    }
}
