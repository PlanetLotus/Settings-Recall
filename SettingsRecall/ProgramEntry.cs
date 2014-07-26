using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SettingsRecall {
    public class ProgramEntry {
        public ProgramEntry() {
            name = "";
            isPermanent = false;
            description = "";
            paths = new List<string>();
        }

        public ProgramEntry(string name, bool isPermanent, List<string> paths, string description = "") {
            Name = name;
            this.isPermanent = isPermanent;
            Paths = paths;
            Description = description;
        }

        /// <summary>
        /// Constructor which converts a DataRow extracted from the DB into a ProgramEntry.
        /// </summary>
        /// <param name="row">DataRow extracted from programEntry table.</param>
        public ProgramEntry(DataRow row) {
            // Assign each ProgramEntry field
            Name = row["Name"].ToString();
            isPermanent = row["IsPermanent"].ToString() == "1" ? true : false;
            Paths = JsonConvert.DeserializeObject<List<string>>(row["Paths"].ToString());
            Description = row["Description"].ToString();
        }

        public override string ToString() {
            string paths = string.Join(", ", Paths);

            return string.Format("*****\r\nName: {0}\r\nIsPermanent: {1}\r\nPaths: {2}\r\nDescription: {3}\r\n*****",
                Name,
                IsPermanent,
                Description,
                paths);
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

        public bool IsPermanent {
            get { return isPermanent; }
        }

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

        private string name;
        private List<string> paths;
        private string description;
        private bool isPermanent;
    }
}
