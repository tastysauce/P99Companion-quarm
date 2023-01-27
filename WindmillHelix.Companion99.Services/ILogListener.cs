using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Services
{
    public interface ILogListener
    {
        void HandleLogLine(string serverName, string characterName, DateTime eventDate, string line);
    }
}
