using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.App.Models
{
    public class SpellDisplayModel
    {
        public int Level { get; set; }

        public string SpellName { get; set; }

        public bool HasSpell { get; set; }

        public string HasSpellDisplay
        {
            get
            {
                return HasSpell ? "X" : string.Empty;
            }
        }
    }
}
