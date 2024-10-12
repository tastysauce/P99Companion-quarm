using System;
using System.Collections.Generic;
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
using WindmillHelix.Companion99.Common;
using WindmillHelix.Companion99.Services;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.App
{
    /// <summary>
    /// Interaction logic for LastZoneControl.xaml
    /// </summary>
    public partial class LastZoneControl : UserControl, ILogListener
    {
        private readonly ILastZoneService _lastZoneService;
        private readonly ILastLoginService _lastLoginService;

        private bool _isResetting = false;

        private IReadOnlyCollection<CharacterZone> _items;

        public LastZoneControl()
        {
            InitializeComponent();

            var logReaderService = DependencyInjector.Resolve<ILogReaderService>();
            _lastZoneService = DependencyInjector.Resolve<ILastZoneService>();
            _lastLoginService = DependencyInjector.Resolve<ILastLoginService>();
            logReaderService.AddListener(this);

            LoadZones();
            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        public void HandleLogLine(string serverName, string characterName, DateTime eventDate, string line)
        {
            const string zonedPrefix = "You have entered ";
            if (!line.StartsWith(zonedPrefix))
            {
                return;
            }

            var zone = line.Substring(zonedPrefix.Length).TrimEnd('.');
            var account = _lastLoginService.GetLastLoginName();
            _lastZoneService.SetLastZone(serverName, characterName, zone, account);
            LoadZones();
        }

        private void LoadZones()
        {
            var items = _lastZoneService.GetLastZones()
                .OrderBy(x => x.ServerName).ThenBy(x => x.CharacterName).ToList();

            Dispatcher.Invoke(() =>
            {
                _items = items;
            });
            
            ApplyFilters();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _isResetting = true;
            try
            {
                SearchTextBox.Clear();
                _isResetting = false;
                ApplyFilters();

            }
            finally
            {
                _isResetting = false;
            }
        }

        private void ApplyFilters()
        {
            Dispatcher.Invoke(() =>
            {
                var items = _items;
                var filter = SearchTextBox.Text?.Trim();
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    items = items.Where(x => x.Account.ContainsIngoreCase(filter) || x.CharacterName.ContainsIngoreCase(filter) || x.ZoneName.ContainsIngoreCase(filter)).ToList();
                }

                ResultsListView.ItemsSource = items;
            });
        }
    }
}
