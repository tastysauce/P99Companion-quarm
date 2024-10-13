using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Common;
using WindmillHelix.Companion99.Services;

namespace WindmillHelix.Companion99.App.Services
{
    public class KillControlService : IKillControlService, ILogListener
    {
        private readonly ILastZoneService _lastZoneService;

        public KillControlService(ILogReaderService logReaderService, ILastZoneService lastZoneService)
        {
            logReaderService.AddListener(this);
            _lastZoneService = lastZoneService;
        }

        public void HandleLogLine(string serverName, string characterName, DateTime eventDate, string line)
        {
            if (line.StartsWith("You have been slain", StringComparison.OrdinalIgnoreCase) || line.Equals("You died."))
            {
                HandleDeath(serverName, characterName, eventDate);
            }
        }

        private void HandleDeath(string serverName, string characterName, DateTime eventDate)
        {
            var lastZones = _lastZoneService.GetLastZones();
            var currentZone = lastZones.SingleOrDefault(x => x.ServerName.EqualsIngoreCase(serverName) && x.CharacterName.EqualsIngoreCase(characterName));
            if(currentZone?.ZoneName == "Plane of Air")
            {
                _lastZoneService.SetSkyCorpseDate(serverName, characterName, eventDate.ToUniversalTime());
            }
        }
    }
}
