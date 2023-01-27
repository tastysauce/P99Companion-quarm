using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.Services
{
    public interface IInventoryService
    {
        IReadOnlyCollection<InventoryItem> GetInventoryItems();

        FileSystemWatcher CreateInventoryChangedWatcher();
    }
}
