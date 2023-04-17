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
    /// Interaction logic for LastLocationControl.xaml
    /// </summary>
    public partial class LastLocationControl : UserControl, ILogListener
    {
        private class LastLocation
        {
            public string ZoneName { get; set; }

            public string Location { get; set; }

            public override string ToString()
            {
                return $"{ZoneName}: {Location}";
            }
        }


        private List<LastLocation> _locs = new List<LastLocation>();

        public LastLocationControl()
        {
            InitializeComponent();
            var logReaderService = DependencyInjector.Resolve<ILogReaderService>();
            logReaderService.AddListener(this);
        }

        public void HandleLogLine(string serverName, string characterName, DateTime eventDate, string line)
        {
            const string locationPrefix = "Your Location is ";
            const string zonedPrefix = "You have entered ";
            if(line.StartsWith(zonedPrefix))
            {
                var zone = line.Substring(zonedPrefix.Length).TrimEnd('.');
                if(_locs.Count > 0 && string.IsNullOrWhiteSpace(_locs[0].Location))
                {
                    _locs.RemoveAt(0);
                }

                _locs.Insert(0, new LastLocation { ZoneName = zone });

                if(_locs.Count > 5)
                {
                    _locs.RemoveAt(5);
                }
            }

            if (line.StartsWith(locationPrefix))
            {
                var location = line.Substring(locationPrefix.Length);
                if (_locs.Count == 0)
                {
                    _locs.Insert(0, new LastLocation { ZoneName = "Unknown Zone", Location = location });
                }
                else
                {
                    _locs[0].Location = location;
                }

                Dispatcher.Invoke(() =>
                {
                    LocationLabel.Content = location;
                });
            }
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(LocationLabel.Content.ToString());
        }

        private void LocationLabel_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContextMenu menu = new ContextMenu();

            foreach(var loc in _locs)
            {
                MenuItem menuItem = new MenuItem();
                menuItem.Tag = loc;
                menuItem.Header = loc;
                menuItem.Click += MenuItem_Click;
                menu.Items.Add(menuItem);
            }

            menu.IsOpen = true;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            var loc = (LastLocation)item.Tag;
            if (!string.IsNullOrWhiteSpace(loc.Location))
            {
                Clipboard.SetText(loc.Location);
            }
        }
    }
}
