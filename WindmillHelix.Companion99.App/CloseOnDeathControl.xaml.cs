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
    /// Interaction logic for CloseOnDeathControl.xaml
    /// </summary>
    public partial class CloseOnDeathControl : UserControl, ILogListener
    {
        private readonly IGameProcessService _gameProcessService;

        public CloseOnDeathControl()
        {
            InitializeComponent();

            _gameProcessService = DependencyInjector.Resolve<IGameProcessService>();

            var logReaderService = DependencyInjector.Resolve<ILogReaderService>();
            logReaderService.AddListener(this);
        }

        public void HandleLogLine(string serverName, string characterName, DateTime eventDate, string line)
        {
            if(line.StartsWith("You have been slain"))
            {
                bool isChecked = false;
                Dispatcher.Invoke(() => isChecked = CloseOnDeathCheckbox.IsChecked.GetValueOrDefault());
                _gameProcessService.KillGameProcess();
            }
        }
    }
}
