using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.Services
{
    public interface INoteService
    {
        public IReadOnlyCollection<NoteItem> GetAllNotes();

        public string GetNote(string serverName, string characterName);

        public void SetNote(string serverName, string characterName, string note);
    }
}
