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
            string version_str;
            string os_str;

            // insert default values if null
            if (String.IsNullOrEmpty(entry.Version))
            {
                version_str = "<Version>";
            }
            else
            {
                version_str = entry.Version;
            }

            if (String.IsNullOrEmpty(entry.OS))
            {
                os_str = "<OS>";
            }
            else
            {
                os_str = entry.OS;
            }

            return String.Format("{0}, {1}", version_str, os_str);
        }
    }
}
