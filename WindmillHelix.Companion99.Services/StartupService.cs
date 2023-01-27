using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Common;

namespace WindmillHelix.Companion99.Services
{
    public class StartupService : IStartupService
    {
        private readonly IConfigurationService _configurationService;

        public StartupService(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public void EnsureDataDirectoryExists()
        {
            var dataFolder = FileHelper.GetDataFolder();
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }
        }

        public bool IsEverQuestDirectoryValid()
        {
            string folderLocation = _configurationService.EverQuestFolder;
            var result = _configurationService.IsValidEverQuestFolder(folderLocation);

            return result;
        }
    }
}
