using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsRecall
{
    public class VersionListEntry
    {
        public ProgramEntry entry { get; set; }

        /// <summary>
        /// Default Constructor for VersionListEntry
        /// </summary>
        public VersionListEntry()
        {
            this.entry = new ProgramEntry();
        }

        /// <summary>
        /// Constructor for VersionListEntry
        /// </summary>
        /// <param name="p_entry">ProgramEntry parameter</param>
        public VersionListEntry(ProgramEntry p_entry)
        {
            this.entry = p_entry;
        }

        /// <summary>
        /// Create a formated string for use in the Version List
        /// </summary>
        /// <returns>Formatted String</returns>
        public override string ToString()
        {
            return String.Format("VERSION LIST ENTRY NEEDS DELETED");
        }
    }
}
