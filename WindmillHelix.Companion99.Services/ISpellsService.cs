using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Common;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.Services
{
    public interface ISpellsService
    {
        EverQuestClass? DetermineClassFromSpellbook(IReadOnlyCollection<SpellbookItem> spellbookItems);

        IReadOnlyCollection<SpellItem> GetSpells(EverQuestClass everQuestClass);
    }
}
