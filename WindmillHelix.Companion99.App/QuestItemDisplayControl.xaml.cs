using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using WindmillHelix.Companion99.Services.Events;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.App
{
    /// <summary>
    /// Interaction logic for QuestItemDisplayControl.xaml
    /// </summary>
    public partial class QuestItemDisplayControl : UserControl, IEventSubscriber<InventoryFilesChangedEvent>, IDisposable
    {
        private readonly IQuestService _questService;
        private readonly IInventoryService _inventoryService;
        private readonly IEventService _eventService;

        private bool _isInitialized = false;
        private IReadOnlyCollection<InventoryItem> _items;

        public QuestItemDisplayControl()
        {
            InitializeComponent();

            _questService = DependencyInjector.Resolve<IQuestService>();
            _inventoryService = DependencyInjector.Resolve<IInventoryService>();
            _eventService = DependencyInjector.Resolve<IEventService>();

            _eventService.AddSubscriber(this);
        }

        public int ItemId { get; set; }

        public string CharacterName { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!_isInitialized)
            {
                BuildTree();
            }
        }

        private void BuildTree()
        {
            var items = _inventoryService.GetInventoryItems();
            _items = items.Where(x => x.CharacterName.EqualsIngoreCase(CharacterName)).ToList();


            var questItem = _questService.GetQuestItem(ItemId);
            var element = MakeTreeViewItem(questItem, false, null);
            var treeView = new TreeView();
            treeView.Items.Add(element);

            ContainerGrid.Children.Clear();
            ContainerGrid.Children.Add(treeView);
            _isInitialized = true;
        }

        private UIElement MakeTreeViewItem(QuestItem item, bool isParentChecked, string url)
        {
            var inventoryItem = _items.FirstOrDefault(x => x.ItemId == item.ItemId);
            bool hasItem = inventoryItem != null;
            var itemUrl = item.Url ?? url;
            if(item.SubQuestItems != null && item.SubQuestItems.Length > 0)
            {
                var isChecked = hasItem || isParentChecked;
                var treeViewItem = new TreeViewItem();
                treeViewItem.Tag = item;
                treeViewItem.IsExpanded = !isChecked;
                treeViewItem.Header = MakeCheckableLabel(item, isChecked, itemUrl, inventoryItem);

                foreach(var subQuestItem in item.SubQuestItems)
                {
                    var subQuestElement = MakeTreeViewItem(subQuestItem, hasItem || isParentChecked, itemUrl);
                    treeViewItem.Items.Add(subQuestElement);
                }

                return treeViewItem;
            }

            return MakeCheckableLabel(item, hasItem || isParentChecked, itemUrl, inventoryItem);
        }

        private UIElement MakeCheckableLabel(QuestItem item, bool isChecked, string url, InventoryItem inventoryItem)
        {
            var panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;

            var box = new CheckBox();
            box.Tag = item;
            box.IsChecked = isChecked;
            box.IsEnabled = false;
            panel.Children.Add(box);

            var text = item.ItemName;
            if(inventoryItem != null)
            {
                text += $" [{inventoryItem.Location}]";
            }

            var label = new Label();
            label.Margin = new Thickness(0, 0, 0, 0);
            label.Padding = new Thickness(0, 0, 0, 0);
            label.Content = text;
            label.Tag = url;
            label.MouseDoubleClick += Label_MouseDoubleClick;

            panel.Children.Add(label);

            return panel;
        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var label = (Label)sender;
            var url = label.Tag as string;
            if(!string.IsNullOrWhiteSpace(url))
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
        }

        public Task Handle(InventoryFilesChangedEvent value)
        {
            this.Dispatcher.Invoke(() => BuildTree());
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _eventService.RemoveSubscriber(this);
        }
    }
}
