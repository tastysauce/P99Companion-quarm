using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for InventoryControl.xaml
    /// </summary>
    public partial class InventoryControl : UserControl
    {
        private IReadOnlyCollection<InventoryItem> _items;
        private readonly IInventoryService _inventoryService;
        private readonly FileSystemWatcher _watcher;

        public InventoryControl()
        {
            InitializeComponent();

            _inventoryService = DependencyInjector.Resolve<IInventoryService>();
            _watcher = _inventoryService.CreateInventoryChangedWatcher();

            var items = _inventoryService.GetInventoryItems();

            _items = items;

            var characterNames = _items.Select(x => x.CharacterName).Distinct().OrderBy(x => x).Select(x => new ComboItem<string, string> { Value = x, Display = x });

            var characterItems = new List<ComboItem<string, string>>();
            characterItems.Add(new ComboItem<string, string> { Value = string.Empty, Display = "-- All --" });
            characterItems.AddRange(characterNames);

            CharacterComboBox.ItemsSource = characterItems;
            CharacterComboBox.SelectedIndex = 0;

            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            CharacterComboBox.SelectionChanged += CharacterComboBox_SelectionChanged;

            _watcher = _inventoryService.CreateInventoryChangedWatcher();
            _watcher.Changed += HandleInventoryFilesChanged;
            _watcher.Created += HandleInventoryFilesChanged;
            _watcher.EnableRaisingEvents = true;

            SetItemSource();
        }

        private void HandleInventoryFilesChanged(object sender, FileSystemEventArgs e)
        {
            var items = _inventoryService.GetInventoryItems();

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

        private void SetItemSource()
        {
            var searchText = SearchTextBox.Text;

            var filtered = _items;

            if(!string.IsNullOrWhiteSpace((string)CharacterComboBox.SelectedValue))
            {
                filtered = filtered.Where(x => x.CharacterName == (string)CharacterComboBox.SelectedValue).ToList();
            }

            if(string.IsNullOrWhiteSpace(searchText))
            {
                ItemsListView.ItemsSource = filtered;
                return;
            }

            filtered = filtered.Where(x => x.ItemName.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)).ToList();

            ItemsListView.ItemsSource = filtered;
        }
    }
}
