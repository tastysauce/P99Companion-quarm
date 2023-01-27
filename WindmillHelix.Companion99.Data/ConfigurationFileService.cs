using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Common;

namespace WindmillHelix.Companion99.Data
{
    public class ConfigurationFileService : IConfigurationFileService
    {
        private readonly string _configFile;
        public ConfigurationFileService()
        {
            _configFile = Path.Combine(FileHelper.GetDataFolder(), "config.json");
        }

        public void SetValues(IDictionary<string, string> items)
        {
            var currentValues = GetAllValues();
            foreach (var item in items)
            {
                currentValues[item.Key] = item.Value;
            }

            var json = JsonConvert.SerializeObject(currentValues);
            File.WriteAllText(_configFile, json);
        }

        public IDictionary<string, string> GetAllValues()
        {
            if(!File.Exists(_configFile))
            {
                return new Dictionary<string, string>();
            }

            var json = File.ReadAllText(_configFile);
            var values = JsonConvert.DeserializeObject<IDictionary<string, string>>(json);
            return values;
        }
    }
}
