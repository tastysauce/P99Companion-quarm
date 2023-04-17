using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Services.Interop;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.Services
{
    public class SocialService : ISocialService
    {
        private readonly IConfigurationService _configurationService;

        public SocialService(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public IReadOnlyCollection<string> GetCharacterIniFiles()
        {
            var files = Directory.GetFiles(_configurationService.EverQuestFolder, "*_*.ini")
                .Select(x => new FileInfo(x))
                .Where(x => !x.Name.StartsWith("UI_"))
                .Select(x => x.Name.Substring(0, x.Name.Length - 4))
                .ToList();

            return files;
        }

        public IReadOnlyCollection<Social> GetSocials(string characterIniFile)
        {
            if(!characterIniFile.EndsWith(".ini"))
            {
                characterIniFile += ".ini";
            }

            var fullFilePath = Path.Combine(_configurationService.EverQuestFolder, characterIniFile);
            var socials = new List<Social>();
            
            for(int pageNumber = 1; pageNumber <= 10; pageNumber++)
            {
                for(int itemNumber = 1; itemNumber <= 12; itemNumber++)
                {
                    var prefix = $"Page{pageNumber}Button{itemNumber}";

                    var name = GetSocialValue(fullFilePath, prefix, "Name");
                    var colorString = GetSocialValue(fullFilePath, prefix, "Color");
                    var lines = new List<string>();
                    for(int i = 1; i<= 5; i++)
                    {
                        var line = GetSocialValue(fullFilePath, prefix, $"Line{i}");
                        lines.Add(line);
                    }

                    var toCheck = lines.Concat(new string[] { name, colorString });
                    if(toCheck.All(x => string.IsNullOrWhiteSpace(x)))
                    {
                        continue;
                    }

                    var social = new Social
                    {
                        Name = name,
                        Lines = lines.ToArray(),
                        PageNumber = pageNumber,
                        ItemNumber = itemNumber
                    };

                    if(!string.IsNullOrWhiteSpace(colorString))
                    {
                        social.Color = int.Parse(colorString);
                    }

                    socials.Add(social);
                }
            }

            return socials;
        }

        public void SaveSocial(string characterIniFile, Social social)
        {
            if (!characterIniFile.EndsWith(".ini"))
            {
                characterIniFile += ".ini";
            }

            var fullFilePath = Path.Combine(_configurationService.EverQuestFolder, characterIniFile);

            var prefix = $"Page{social.PageNumber}Button{social.ItemNumber}";

            SetSocialValue(fullFilePath, prefix, "Name", social.Name);
            SetSocialValue(fullFilePath, prefix, "Color", social.Color.ToString());
            for (int i = 1; i <= 5; i++)
            {
                var line = social.Lines != null && social.Lines.Length >= i ? social.Lines[i - 1] : string.Empty;
                SetSocialValue(fullFilePath, prefix, $"Line{i}", line);
            }
        }

        private string GetSocialValue(string fullFilePath, string prefix, string suffix)
        {
            var valueBuilder = new StringBuilder(255);
            IniFile.GetPrivateProfileString("Socials", prefix + suffix, string.Empty, valueBuilder, 255, fullFilePath);
            return valueBuilder.ToString();
        }

        private void SetSocialValue(string fullFilePath, string prefix, string suffix, string value)
        {
            IniFile.WritePrivateProfileString("Socials", prefix + suffix, value, fullFilePath);
        }
    }
}
