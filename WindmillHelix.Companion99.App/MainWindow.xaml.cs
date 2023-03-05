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
using WindmillHelix.Companion99.Services;
using WindmillHelix.Companion99.Services.Events;

namespace WindmillHelix.Companion99.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ILogReaderService _logReaderService;
        private readonly IConfigurationService _configurationService;
        private readonly IInventoryService _inventoryService;
        private readonly FileSystemWatcher _watcher;
        private readonly IEventService _eventService;

        public MainWindow()
        {
            InitializeComponent();

            _logReaderService = DependencyInjector.Resolve<ILogReaderService>();
            _configurationService = DependencyInjector.Resolve<IConfigurationService>();
            _inventoryService = DependencyInjector.Resolve<IInventoryService>();
            _eventService = DependencyInjector.Resolve<IEventService>();

            _watcher = _inventoryService.CreateInventoryChangedWatcher();
            _watcher.Changed += HandleInventoryFilesChanged;
            _watcher.Created += HandleInventoryFilesChanged;
            _watcher.EnableRaisingEvents = true;

            AncientCyclopsTimerControl.Visibility = _configurationService.IsAncientCyclopsTimerEnabled ? Visibility.Visible : Visibility.Hidden;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            _logReaderService.Start();
        }

        private void HandleInventoryFilesChanged(object sender, FileSystemEventArgs e)
        {
            _eventService.Raise<InventoryFilesChangedEvent>();
        }
    }
}
