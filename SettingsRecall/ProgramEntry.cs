using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json;

namespace SettingsRecall
{
    public class ProgramEntry
    {
        public string Name {get; set;}
        public List<string> Paths { get; set; }
        public string Description { get; set; }
        
        public ProgramEntry() {
            this.Name = "";
            this.Description = "";
            this.Paths = null;
        }

        public ProgramEntry(string name, List<string> paths, string description="") {
            this.Name = name;
            this.Paths = paths;
            this.Description = description;
        }

        /// <summary>
        /// Constructor which converts a DataRow extracted from the DB into a ProgramEntry.
        /// </summary>
        /// <param name="row">DataRow extracted from programEntry table.</param>
        public ProgramEntry(DataRow row)
        {
            // Assign each ProgramEntry field
            this.Name = row["Name"].ToString();
            this.Paths = JsonConvert.DeserializeObject<List<string>>(row["Paths"].ToString());
            this.Description = row["Description"].ToString();
        }

        public override string ToString()
        {
            string str;
            string paths = "";
            StringBuilder pathBuilder = new StringBuilder();

            foreach (string path in Paths)
            {
                pathBuilder.Append(path + ", ");
            }
            // Cut off extra comma
            if (pathBuilder.Length > 1) paths = pathBuilder.ToString().Substring(0, pathBuilder.Length-2);

            str = string.Format("*****\r\nName: {0}\r\nPaths: {1}\r\nDescription: {2}\r\n*****",
                Name,
                Description,
                paths);

            return str;
        }
    }
}
