using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsRecall
{
    public class ProgramEntry
    {
        public int Program_ID {get; set;}
        public string Name {get; set;}
        public string Version {get; set;}
        public string OS {get; set;}
        public bool IsPermanent {get; set;}
        public string Description { get; set; }
        public List<string> Paths { get; set; }

        public override string ToString()
        {
            string str;
            StringBuilder pathString = new StringBuilder();

            foreach (string path in Paths)
            {
                pathString.Append(path);
            }

            str = string.Format("{0},{1},{2},{3},{4},{5},{6}",
                Program_ID.ToString(),
                Name,
                Version,
                OS,
                IsPermanent.ToString(),
                Description,
                pathString);

            return str;
        }
    }
}
