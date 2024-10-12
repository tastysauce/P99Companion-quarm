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
using WindmillHelix.Companion99.App.Models;
using WindmillHelix.Companion99.Services;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.App
{
    /// <summary>
    /// Interaction logic for SocialsControl.xaml
    /// </summary>
    public partial class SocialsControl : UserControl
    {
        private readonly ISocialService _socialService;

        private Button _selectedButton;

        public SocialsControl()
        {
            InitializeComponent();

            _socialService = DependencyInjector.Resolve<ISocialService>();

            var items = _socialService.GetCharacterIniFiles();

            var characterNames = items.OrderBy(x => x).Select(x => new ComboItem<string, string> { Value = x, Display = x });

            var characterItems = new List<ComboItem<string, string>>();
            characterItems.Add(new ComboItem<string, string> { Value = string.Empty, Display = "-- All --" });
            characterItems.AddRange(characterNames);

            CharacterComboBox.ItemsSource = characterItems;
            CharacterComboBox.SelectedIndex = 0;
           
            CharacterComboBox.SelectionChanged += CharacterComboBox_SelectionChanged;
        }

        private void CharacterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetItemSource();
        }

        private void SetItemSource()
        {
            _selectedButton = null;
            MainContainer.Children.Clear();

            string characterName = (string)CharacterComboBox.SelectedValue;
            var socials = _socialService.GetSocials(characterName);

            for (int pageNumber = 1; pageNumber <= 10; pageNumber++)
            {
                var pageContainer = new StackPanel();
                pageContainer.Orientation = Orientation.Horizontal;
                var label = new Label();
                label.Width = 60;
                label.Content = pageNumber.ToString() + ":";

                pageContainer.Children.Add(label);

                for (int itemNumber = 1; itemNumber <= 12; itemNumber++)
                {
                    var social = socials.SingleOrDefault(x => x.PageNumber == pageNumber && x.ItemNumber == itemNumber);

                    if(social == null)
                    {
                        social = new Social
                        {
                            PageNumber = pageNumber,
                            ItemNumber = itemNumber
                        };
                    }

                    var button = new Button();
                    button.Content = social.Name;
                    button.Tag = social;
                    button.Width = 80;
                    button.Height = 20;

                    button.MouseRightButtonUp += Button_MouseRightButtonUp;

                    pageContainer.Children.Add(button);

                    if(itemNumber == 4 || itemNumber == 8)
                    {
                        var spacer = new Label();
                        label.Width = 20;
                        pageContainer.Children.Add(spacer);
                    }
                }

                MainContainer.Children.Add(pageContainer);
            }
        }

        private void Button_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var button = (Button)sender;
            if(_selectedButton == null)
            {
                _selectedButton = button;
                button.BorderBrush = Brushes.Yellow;
            }
            else if(_selectedButton == button)
            {
                button.BorderBrush = Brushes.Black;
                _selectedButton = null;
            }
            else
            {
                var social1 = (Social)button.Tag;
                var social2 = (Social)_selectedButton.Tag;
                string characterName = (string)CharacterComboBox.SelectedValue;

                /*
                var temp1 = new Social
                {
                    Name = social1.Name,
                    Color = social1.Color,
                    Lines = social1.Lines,
                    ItemNumber = social2.ItemNumber,
                    PageNumber = social2.PageNumber,
                };

                var temp2 = new Social
                {
                    Name = social2.Name,
                    Color = social2.Color,
                    Lines = social2.Lines,
                    ItemNumber = social1.ItemNumber,
                    PageNumber = social1.PageNumber,
                };

                _socialService.SaveSocial(characterName, temp1);
                _socialService.SaveSocial(characterName, temp2);
                */

                _socialService.SwapSocials(characterName, social1, social2);

                _selectedButton.BorderBrush = Brushes.Black;
                _selectedButton = null;

                SetItemSource();
            }
        }
    }
}
