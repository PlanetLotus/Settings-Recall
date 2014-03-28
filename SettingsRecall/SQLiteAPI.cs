using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SettingsRecall {
    public class SQLiteAPI {
        SQLiteDatabase db;

        public string dbLocation;

        public List<string> supportedOses;

        /// <summary>
        /// Default constructor for SQLiteAPI Class.
        /// Creates a new SQLiteDatabase object.
        /// </summary>
        public SQLiteAPI() {
            db = new SQLiteDatabase();

            // Default value
            dbLocation = "../../test.db";
        }

        /// <summary>
        /// Creates a new SQLiteDatabase object with specified database file.
        /// </summary>
        /// <param name="dbInputFile"></param>
        public SQLiteAPI(string dbInputFile) {
            db = new SQLiteDatabase(dbInputFile);
            dbLocation = dbInputFile;
        }

        private string ValidateProgramEntry(ProgramEntry entry) {
            // Validate entry object: Not null
            if (entry == null)
                return "Entry cannot be null.";

            // Validate name: Not null, not empty
            if (entry.Name == null || entry.Name.Trim() == "")
                return "Entry name cannot be empty.";

            // Validate paths: Cannot be null, must be at least length 1
            if (entry.Paths == null || entry.Paths.Count < 1)
                return "Entry paths must contain at least one path.";

            // Validate paths: Contents cannot be null
            foreach (string path in entry.Paths) {
                if (path == null || path.Trim() == "")
                    return "Entry path must not be empty.";
            }

            // No errors
            return "";
        }

        public bool AddProgram(ProgramEntry entry) {
            // Validate entry
            string validation = ValidateProgramEntry(entry);
            if (validation != "") {
                Console.WriteLine(validation);
                return false;
            }

            // Convert paths to a JSON string
            string json_paths = JsonConvert.SerializeObject(entry.Paths);

            // Prepare the data for db
            Dictionary<string, string> insert = new Dictionary<string, string>();
            insert.Add("Name", entry.Name.Trim());
            insert.Add("Paths", json_paths);
            insert.Add("Description", entry.Description.Trim());

            // Insert into db
            // If there's a problem, the exception is output to console. If we need to output to GUI, we need to pass that info back from SQLiteDatabase.cs
            try {
                db.Insert("Program", insert);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        public bool EditProgram(ProgramEntry entry) 
        {
            // Require name
            if (entry.Name == null || entry.Name.Trim() == "") {
                Console.WriteLine("Program name must not be null or empty.");
                return false;
            }

            // Make sure there's something to update
            if (entry.Paths == null && entry.Description == null) {
                Console.WriteLine("Nothing to update! Returning...");
                return false;
            }

            // Prepare the data for db
            Dictionary<string, string> update = new Dictionary<string, string>();

            // Optional parameter: Add description
            if (entry.Description != null) { update.Add("Description", entry.Description); }

            // Optional parameter: Convert paths to a JSON string
            if (entry.Paths != null) {
                foreach (string path in entry.Paths) {
                    if (path == null || path.Trim() == "") {
                        Console.WriteLine("Entry path must not be empty.");
                        return false;
                    }
                }

                string json_paths = JsonConvert.SerializeObject(entry.Paths);
                update.Add("Paths", json_paths);
                Console.WriteLine(json_paths);
            }

            // Make sure something is being updated
            if (update.Count < 1) {
                Console.WriteLine("Nothing valid to update! Returning...");
                return false;
            }

            // Insert into db
            try {
                db.Update("Program", update, string.Format("Name = '{0}'", entry.Name));
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        public bool DeleteProgram(string name) {
            try {
                db.Delete("Program", string.Format("Name = '{0}'", name));
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Retrieve a list of program entries from the database.
        /// </summary>
        /// <returns>A list of ProgramEntry retrieved from the db.</returns>
        public List<ProgramEntry> GetProgramList() {
            string query = "SELECT Name,Paths,Description FROM Program;";
            DataTable dt;
            List<ProgramEntry> entryList = new List<ProgramEntry>();

            try {
                dt = db.GetDataTable(query);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return null;
            }

            // Make sure rows were returned
            if (dt.Rows.Count < 1) {
                Console.WriteLine("No rows returned for GetProgramList.");
                return null;
            }
            
            // create a ProgramEntry object for each row in the DataTable
            foreach (DataRow row in dt.Rows)
            {
                ProgramEntry entry = new ProgramEntry(row);
                entryList.Add(entry); // add to list
            }

            return entryList;
        }

        public List<string> GetProgramNameList() {
            // Fetch the global list..not using this function's parameter at the moment
            string query = "SELECT Name FROM Program;";
            DataTable dt;
            try {
                dt = db.GetDataTable(query);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return null;
            }

            // Make sure rows were returned
            if (dt.Rows.Count < 1) {
                Console.WriteLine("No rows returned for GetProgramNameList.");
                return null;
            }

            // Convert datatable to list
            List<string> names = new List<string>();
            foreach (DataRow row in dt.Rows)
                names.Add(row["Name"].ToString());
            
            return names;
        }

        /// <summary>
        /// Get one entry from the ProgramEntry table in the db.
        /// </summary>
        /// <param name="program_ID">The unique program ID number.</param>
        /// <returns>A ProgramEntry
        /// Program_ID, Name, Version, OS, IsPermanent, Description, Paths, Foreign Key</returns>
        public ProgramEntry GetProgram(string name)
        {
            // query the database for row containing program_ID
            string query = string.Format("SELECT Name,Paths,Description FROM Program WHERE Name = '{0}';", name);
            DataTable dt;
            try
            {
                dt = db.GetDataTable(query);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

            // Make sure rows were returned
            if (dt.Rows.Count < 1) {
                Console.WriteLine("No rows returned for Name == {0}", name);
                return null;
            }

            // create a ProgramEntry object from the DataTable
            return new ProgramEntry(dt.Rows[0]);
        }

        public void ClearDB() {
            db.ClearDB();
        }
    }
}