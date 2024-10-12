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

        public void SwapSocials(string characterIniFile, Social social1, Social social2)
        {
            var key1 = CalculateHotbuttonEntryKey(social1.PageNumber, social1.ItemNumber);
            var key2 = CalculateHotbuttonEntryKey(social2.PageNumber, social2.ItemNumber);

            var temp1 = new Social
            {
                Name = social1.Name,
                Color = social1.Color,
                Lines = social1.Lines,
                ItemNumber = social2.ItemNumber,
                PageNumber = social2.PageNumber,
            };

            var temp2 = new Social
            {
                Name = social2.Name,
                Color = social2.Color,
                Lines = social2.Lines,
                ItemNumber = social1.ItemNumber,
                PageNumber = social1.PageNumber,
            };

            SaveSocial(characterIniFile, temp1);
            SaveSocial(characterIniFile, temp2);

            //[HotButtons]
            //Page1Button8 = F15
            //Page10Button1 = E87
            //Page10Button6 = E37

            if (!characterIniFile.EndsWith(".ini"))
            {
                characterIniFile += ".ini";
            }

            var fullFilePath = Path.Combine(_configurationService.EverQuestFolder, characterIniFile);
            var buttonsSectionName = "HotButtons";
            for(int pageNumber = 1; pageNumber < 10; pageNumber++)
            {
                for(int buttonNumber = 1; buttonNumber < 10; buttonNumber++)
                {
                    var buttonKey = $"Page{pageNumber}Button{buttonNumber}";
                    var valueBuilder = new StringBuilder(255);
                    IniFile.GetPrivateProfileString(buttonsSectionName, buttonKey, string.Empty, valueBuilder, 255, fullFilePath);
                    var currentValue = valueBuilder.ToString();
                    string newValue = null;
                    if(currentValue == key1)
                    {
                        newValue = key2;
                    }
                    else if(currentValue == key2)
                    {
                        newValue = key1;
                    }

                    if (newValue != null)
                    {
                        IniFile.WritePrivateProfileString(buttonsSectionName, buttonKey, newValue, fullFilePath);
                    }
                }
            }

            
        }

        private static string CalculateHotbuttonEntryKey(int pageNumber, int itemNumber)
        {
            var numeral = (pageNumber - 1) * 12 + itemNumber - 1;
            var result = "E" + numeral.ToString();
            return result;
        }
    }
}
