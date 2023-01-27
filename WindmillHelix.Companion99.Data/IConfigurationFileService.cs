using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Data
{
    public interface IConfigurationFileService
    {
        void SetValues(IDictionary<string, string> items);

        IDictionary<string, string> GetAllValues();
    }
}
