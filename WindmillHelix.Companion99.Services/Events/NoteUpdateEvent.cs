using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.Services.Events
{
    public class NoteUpdateEvent
    {
        public NoteUpdateEvent(NoteItem note)
        {
            Note = note;
        }

        public NoteItem Note { get; private set; }
    }
}
