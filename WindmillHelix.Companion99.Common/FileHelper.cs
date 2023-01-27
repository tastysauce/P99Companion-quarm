using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Common
{
    public static class FileHelper
    {
        public static string GetDataFolder()
        {
            if(!string.IsNullOrWhiteSpace(AppConfig.DataFolderLocationOverride))
            {
                return AppConfig.DataFolderLocationOverride;
            }

            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dataFolder = Path.Combine(appData, "P99Companion");
            return dataFolder;
        }

        public static string GetAppFolder()
        {
            var entryFile = new FileInfo(Assembly.GetEntryAssembly().Location);
            var appFolder = entryFile.DirectoryName;
            return appFolder;
        }
    }
}
