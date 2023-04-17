using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for TellListControl.xaml
    /// </summary>
    public partial class TellListControl : UserControl, ILogListener
    {
        private Dictionary<string, TellControl> _tellControls = new Dictionary<string, TellControl>();

        public TellListControl()
        {
            InitializeComponent();

            var logReaderService = DependencyInjector.Resolve<ILogReaderService>();
            logReaderService.AddListener(this);
        }

        public void HandleLogLine(string serverName, string characterName, DateTime eventDate, string line)
        {
            if(!line.Contains(" tells you, '") && !line.StartsWith("You told "))
            {
                return;
            }

            string direction;
            string partnerName;
            string text;

            Regex regex;
            if (line.StartsWith("You told "))
            {
                regex = new Regex(@"You told (\w*), '(.*)'");
                direction = "out";
            }
            else
            {
                regex = new Regex(@"(\w*) tells you, '(.*)'");
                direction = "in";
            }

            var match = regex.Match(line);
            if (match.Success)
            {
                partnerName = match.Captures[0].Value;
                text = match.Captures[1].Value;
                DispatchTell(partnerName, text, direction);
            }
        }

        private void DispatchTell(string partnerName, string text, string direction)
        {
            if(!_tellControls.ContainsKey(partnerName))
            {
                _tellControls.Add(partnerName, new TellControl());
            }

            
            TellsStackPanel.Children.Insert(0, _tellControls[partnerName]);
        }
    }
}
