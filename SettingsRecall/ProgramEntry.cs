using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json;

namespace SettingsRecall {
    public class ProgramEntry {
        public ProgramEntry() {
            Name = "";
            IsPermanent = false;
            Description = "";
            Paths = new List<string>();
        }

        public ProgramEntry(string name, bool isPermanent, List<string> paths, string description = "") {
            Name = validateName(name);
            IsPermanent = isPermanent;
            Paths = validatePaths(paths);
            Description = validateDescription(description);
        }

        /// <summary>
        /// Constructor which converts a DataRow extracted from the DB into a ProgramEntry.
        /// </summary>
        /// <param name="row">DataRow extracted from programEntry table.</param>
        public ProgramEntry(DataRow row) {
            // Assign each ProgramEntry field
            Name = row["Name"].ToString();
            IsPermanent = row["IsPermanent"].ToString() == "1" ? true : false;
            Paths = JsonConvert.DeserializeObject<List<string>>(row["Paths"].ToString());
            Description = row["Description"].ToString();
        }

        public override string ToString() {
            string str;
            string paths = "";
            StringBuilder pathBuilder = new StringBuilder();

            foreach (string path in Paths) {
                pathBuilder.Append(path + ", ");
            }
            // Cut off extra comma
            if (pathBuilder.Length > 1) paths = pathBuilder.ToString().Substring(0, pathBuilder.Length - 2);

            str = string.Format("*****\r\nName: {0}\r\nIsPermanent: {1}\r\nPaths: {2}\r\nDescription: {3}\r\n*****",
                Name,
                IsPermanent,
                Description,
                paths);

            return str;
        }

        private string validateName(string name) {
            if (name == null)
                throw new ArgumentNullException("Name cannot be null.");

            if (name.Trim() == "")
                throw new ArgumentException("Name cannot be empty.");

            return name.Trim();
        }

        private List<string> validatePaths(List<string> paths) {
            if (paths == null)
                throw new ArgumentNullException("Paths cannot be null.");

            if (paths.Any(path => path == null || path.Trim() == ""))
                throw new ArgumentException("No path can be null or empty.");

            // TODO: Add a unit test for this case
            HashSet<string> hashset = new HashSet<string>();
            if (paths.Any(path => !hashset.Add(path)))
                throw new ArgumentException("Duplicates in paths aren't allowed.");

            return paths.Select(path => path.Trim()).ToList();
        }

        private string validateDescription(string description) {
            if (description == null)
                throw new ArgumentNullException("Description cannot be null.");

            return description.Trim();
        }

        private string name;
        private List<string> paths;
        private string description;
        public bool IsPermanent { get; set; }

        public string Name {
            get { return name; }
            set { name = validateName(value); }
        }

        public List<string> Paths {
            get { return paths; }
            set { paths = validatePaths(value); }
        }

        public string Description {
            get { return description; }
            set { description = validateDescription(value); }
        }
    }
}
