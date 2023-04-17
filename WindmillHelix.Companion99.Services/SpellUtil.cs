using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Services
{
    public static class SpellUtil
    {
        public static bool SpellNameEquals(string item1, string item2)
        {
            item1 = NormalizeSpellName(item1);
            item2 = NormalizeSpellName(item2);
            
            return item2.Equals(item1, StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeSpellName(string spellName)
        {
            return spellName
                .Replace("`", string.Empty)
                .Replace("'", string.Empty)
                .Replace(":", string.Empty)
                .Replace(" ", string.Empty);
        }
    }
}
