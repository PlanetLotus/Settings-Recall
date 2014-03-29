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
        public bool IsPermanent { get; set; }
        public List<string> Paths { get; set; }
        public string Description { get; set; }
        
        public ProgramEntry() {
            Name = "";
            IsPermanent = false;
            Description = "";
            Paths = new List<string>();
        }

        public ProgramEntry(string name, bool isPermanent, List<string> paths, string description="") {
            if (name == null || paths == null || description == null)
                throw new ArgumentNullException("Null values are not acceptable for ProgramEntry members.");

            if (name.Trim() == "")
                throw new ArgumentException("Name cannot be empty.");

            if (paths.Any(path => path == null || path.Trim() == ""))
                throw new ArgumentException("No path can be null or empty.");

            Name = name;
            IsPermanent = isPermanent;
            Paths = paths;
            Description = description;
        }

        /// <summary>
        /// Constructor which converts a DataRow extracted from the DB into a ProgramEntry.
        /// </summary>
        /// <param name="row">DataRow extracted from programEntry table.</param>
        public ProgramEntry(DataRow row)
        {
            // Assign each ProgramEntry field
            Name = row["Name"].ToString();
            IsPermanent = row["IsPermanent"].ToString() == "1" ? true : false;
            Paths = JsonConvert.DeserializeObject<List<string>>(row["Paths"].ToString());
            Description = row["Description"].ToString();
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

            str = string.Format("*****\r\nName: {0}\r\nIsPermanent: {1}\r\nPaths: {2}\r\nDescription: {3}\r\n*****",
                Name,
                IsPermanent,
                Description,
                paths);

            return str;
        }
    }
}
