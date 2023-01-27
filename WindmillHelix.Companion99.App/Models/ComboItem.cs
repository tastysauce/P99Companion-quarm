using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.App.Models
{
    public class ComboItem<TDisplay, TValue>
    {
        public TDisplay Display { get; set; }

        public TValue Value { get; set; }
    }
}
