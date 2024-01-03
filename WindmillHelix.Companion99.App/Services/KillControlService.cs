using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Services;

namespace WindmillHelix.Companion99.App.Services
{
    public class KillControlService : IKillControlService, ILogListener
    {
        public KillControlService(ILogReaderService logReaderService)
        {
            logReaderService.AddListener(this);
        }

        public void HandleLogLine(string serverName, string characterName, DateTime eventDate, string line)
        {
            if (line.StartsWith("You have been slain", StringComparison.OrdinalIgnoreCase) && false)
            {
                HandleDeath();
            }
        }

        private void HandleDeath()
        {
            // todo: raise a notification or something
        }
    }
}
