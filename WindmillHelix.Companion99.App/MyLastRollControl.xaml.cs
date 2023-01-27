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
    /// Interaction logic for MyLastRollControl.xaml
    /// </summary>
    public partial class MyLastRollControl : UserControl, ILogListener
    {
        private bool _isNextLineMyRoll = false;

        public MyLastRollControl()
        {
            InitializeComponent();

            var logReaderService = DependencyInjector.Resolve<ILogReaderService>();
            logReaderService.AddListener(this);
        }

        public void HandleLogLine(string serverName, string characterName, DateTime eventDate, string line)
        {
            if(_isNextLineMyRoll)
            {
                var parts = line.Split(' ');
                var roll = parts.Last().Replace(".", string.Empty);
                _isNextLineMyRoll = false;

                Dispatcher.Invoke(() =>
                {
                    LastRollLabel.Content = roll;
                });

                return;
            }

            if(line.Equals($"**A Magic Die is rolled by {characterName}.", StringComparison.CurrentCultureIgnoreCase))
            {
                _isNextLineMyRoll = true;
            }
        }
    }
}
