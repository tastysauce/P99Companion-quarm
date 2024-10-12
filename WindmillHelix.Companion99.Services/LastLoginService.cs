using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Services.Interop;

namespace WindmillHelix.Companion99.Services
{
    public class LastLoginService : ILastLoginService
    {
        private readonly IConfigurationService _configurationService;

        public LastLoginService(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public string GetLastLoginName()
        {
            var fullFilePath = Path.Combine(_configurationService.EverQuestFolder, "eqlsPlayerData.ini");
            var valueBuilder = new StringBuilder(255);
            IniFile.GetPrivateProfileString("PLAYER", "Username", string.Empty, valueBuilder, 255, fullFilePath);
            return valueBuilder.ToString();
        }
    }
}
