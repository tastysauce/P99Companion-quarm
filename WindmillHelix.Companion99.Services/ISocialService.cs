using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.Services
{
    public interface ISocialService
    {
        IReadOnlyCollection<string> GetCharacterIniFiles();

        IReadOnlyCollection<Social> GetSocials(string characterIniFile);

        void SaveSocial(string characterIniFile, Social social);

        void SwapSocials(string characterIniFile, Social social1, Social social2);
    }
}
