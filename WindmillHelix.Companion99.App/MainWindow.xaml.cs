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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ILogReaderService _logReaderService;

        public MainWindow()
        {
            InitializeComponent();

            _logReaderService = DependencyInjector.Resolve<ILogReaderService>();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            _logReaderService.Start();
        }
    }
}
