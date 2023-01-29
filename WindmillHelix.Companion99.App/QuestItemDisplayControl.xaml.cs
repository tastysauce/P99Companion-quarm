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
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.App
{
    /// <summary>
    /// Interaction logic for QuestItemDisplayControl.xaml
    /// </summary>
    public partial class QuestItemDisplayControl : UserControl
    {
        private readonly IQuestService _questService;
        private bool _isInitialized = false;

        public QuestItemDisplayControl()
        {
            InitializeComponent();

            _questService = DependencyInjector.Resolve<IQuestService>();
        }

        public int ItemId { get; set; }

        public IReadOnlyCollection<InventoryItem> Items { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!_isInitialized)
            {
                var questItem = _questService.GetQuestItem(ItemId);
                var element = MakeTreeViewItem(questItem, false);
                var treeView = new TreeView();
                treeView.Items.Add(element);
                ContainerGrid.Children.Add(treeView);
                _isInitialized = true;
            }
        }

        private UIElement MakeTreeViewItem(QuestItem item, bool isParentChecked)
        {
            bool hasItem = Items.Any(x => x.ItemId == item.ItemId);
            if(item.SubQuestItems != null && item.SubQuestItems.Length > 0)
            {
                var isChecked = hasItem || isParentChecked;
                var treeViewItem = new TreeViewItem();
                treeViewItem.Tag = item;
                treeViewItem.IsExpanded = !isChecked;
                treeViewItem.Header = MakeCheckableLabel(item, isChecked);

                foreach(var subQuestItem in item.SubQuestItems)
                {
                    var subQuestElement = MakeTreeViewItem(subQuestItem, hasItem || isParentChecked);
                    treeViewItem.Items.Add(subQuestElement);
                }

                return treeViewItem;
            }

            return MakeCheckableLabel(item, hasItem || isParentChecked);
        }

        private UIElement MakeCheckableLabel(QuestItem item, bool isChecked)
        {
            var panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;

            var box = new CheckBox();
            box.Tag = item;
            box.IsChecked = isChecked;
            box.IsEnabled = false;
            panel.Children.Add(box);

            var label = new Label();
            label.Margin = new Thickness(0, 0, 0, 0);
            label.Padding = new Thickness(0, 0, 0, 0);
            label.Content = item.ItemName;
            panel.Children.Add(label);

            return panel;
        }
    }
}
