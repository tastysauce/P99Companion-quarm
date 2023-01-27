using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Common
{
    public static class AppConfig
    {
        public static string DataFolderLocationOverride
        {
            get
            {
                return ConfigurationManager.AppSettings[nameof(DataFolderLocationOverride)];
            }
        }
    }
}
