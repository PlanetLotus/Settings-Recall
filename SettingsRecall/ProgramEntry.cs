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
        public int Program_ID {get; set;}
        public string Name {get; set;}
        public string Version {get; set;}
        public string OS {get; set;}
        public bool IsPermanent {get; set;}
        public string Description { get; set; }
        public List<string> Paths { get; set; }
        
        /// <summary>
        /// Default constructor for ProgramEntry
        /// </summary>
        public ProgramEntry() {
            // Initialize isPermanent to false
            this.IsPermanent = false;
        }

        /// <summary>
        /// Constructor which converts a DataRow extracted from the DB into a ProgramEntry.
        /// </summary>
        /// <param name="row">DataRow extracted from programEntry table.</param>
        public ProgramEntry(DataRow row)
        {
            // Initialize isPermanent to false
            this.IsPermanent = false;

            // Assign each ProgramEntry field
            this.Program_ID = Globals.StrToInt(row["Program_ID"].ToString());
            this.Name = row["Name"].ToString();
            this.Version = row["Version"].ToString();
            this.OS = row["OS"].ToString();
            if (Globals.StrToInt(row["IsPermanent"].ToString()) == 1) this.IsPermanent = true;
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

            str = string.Format("*****\nProgram_ID: {0}\nName: {1}\nVersion: {2}\nOS: {3}\nIsPermanent: {4}\nDescription: {5}\nPaths: {6}\n*****",
                Program_ID.ToString(),
                Name,
                Version,
                OS,
                IsPermanent.ToString(),
                Description,
                paths);

            return str;
        }
    }
}
