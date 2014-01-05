using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsRecall
{
    /// <summary>
    /// Provides required information to perform a change to a program entry.
    /// </summary>
    class EntryChange
    {
        public string entry_type { get; set; } // The type of modification. Add, Edit, or Delete
        public ProgramEntry entry { get; set; } // the ProgramEntry data to be used in the modification.
    }
}
