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
using WindmillHelix.Companion99.Services;

namespace WindmillHelix.Companion99.App
{
    /// <summary>
    /// Interaction logic for LastZoneControl.xaml
    /// </summary>
    public partial class LastZoneControl : UserControl, ILogListener
    {
        private readonly ILastZoneService _lastZoneService;

        public LastZoneControl()
        {
            InitializeComponent();

            var logReaderService = DependencyInjector.Resolve<ILogReaderService>();
            _lastZoneService = DependencyInjector.Resolve<ILastZoneService>();
            logReaderService.AddListener(this);

            LoadZones();
        }

        public void HandleLogLine(string serverName, string characterName, DateTime eventDate, string line)
        {
            const string zonedPrefix = "You have entered ";
            if (!line.StartsWith(zonedPrefix))
            {
                return;
            }

            var zone = line.Substring(zonedPrefix.Length).TrimEnd('.');
            _lastZoneService.SetLastZone(serverName, characterName, zone);
            LoadZones();
        }

        private void LoadZones()
        {
            var items = _lastZoneService.GetLastZones()
                .OrderBy(x => x.ServerName).ThenBy(x => x.CharacterName).ToList();

            Dispatcher.Invoke(() =>
            {
                ResultsListView.ItemsSource = items;
            });
        }
    }
}
