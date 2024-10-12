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
    /// Interaction logic for GroupLeaderControl.xaml
    /// </summary>
    public partial class GroupLeaderControl : UserControl, ILogListener
    {
        private readonly Regex _leadershipTransfer = new Regex(@"^([A-Za-z]+) is now the leader of your group\.$");
        private readonly Regex _joinGroup = new Regex(@"^You notify ([A-Za-z]+) that you agree to join the group\.$");
        private const string NoLeader = "-NONE-";
        private string _lastLeader = NoLeader;

        public GroupLeaderControl()
        {
            InitializeComponent();

            LeaderLabel.MouseDoubleClick += LeaderLabel_MouseDoubleClick;
            var logReaderService = DependencyInjector.Resolve<ILogReaderService>();
            logReaderService.AddListener(this);
        }

        private void LeaderLabel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(_lastLeader == NoLeader)
            {
                return;
            }

            var text = $"My group leader is {_lastLeader}";
            Clipboard.SetText(text);
        }

        public void HandleLogLine(string serverName, string characterName, DateTime eventDate, string line)
        {
            //Your group has been disbanded.
            //You have been removed from the group.
            //You have formed the group.
            //You are now the leader of your group.

            //You notify Blah that you agree to join the group.
            //Blah is now the leader of your group.

            bool isGroupUpdate = true;
            string leader = null;

            if (line == "Your group has been disbanded."
                || line == "You have been removed from the group."
                || line == "Welcome to EverQuest!")
            {
                // no group
                leader = NoLeader;
            }
            else if (line == "You have formed the group." || line == "You are now the leader of your group.")
            {
                // you're the leader
                leader = "You";
            }
            else if (_leadershipTransfer.IsMatch(line))
            {
                var matches = _leadershipTransfer.Matches(line);
                leader = matches[0].Groups[1].Value;
            }
            else if (_joinGroup.IsMatch(line))
            {
                var matches = _joinGroup.Matches(line);
                leader = matches[0].Groups[1].Value;
            }
            else
            {
                isGroupUpdate = false;
            }

            if (isGroupUpdate)
            {
                if (leader == "You")
                {
                    _lastLeader = characterName;
                }
                else
                {
                    _lastLeader = leader;
                }

                Dispatcher.Invoke(() =>
                {
                    LeaderLabel.Content = "Leader: " + leader;
                });
            }
        }
    }
}
