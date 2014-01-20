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

        public List<string> supportedOses;

        /// <summary>
        /// Default constructor for SQLiteAPI Class.
        /// Creates a new SQLiteDatabase object.
        /// </summary>
        public SQLiteAPI() {
            InitSupportedOses();
            db = new SQLiteDatabase();
        }

        /// <summary>
        /// Creates a new SQLiteDatabase object with specified database file.
        /// </summary>
        /// <param name="dbInputFile"></param>
        public SQLiteAPI(string dbInputFile) {
            InitSupportedOses();
            db = new SQLiteDatabase(dbInputFile);
        }

        private void InitSupportedOses() {
            supportedOses = new List<string>() {
                "",                     // No specified OS is also acceptable
                "Windows XP 32-bit",
                "Windows XP 64-bit",
                "Windows Vista 32-bit",
                "Windows Vista 64-bit",
                "Windows 7 32-bit",
                "Windows 7 64-bit",
                "Windows 8 32-bit",
                "Windows 8 64-bit"
            };
        }

        private string ValidateProgramEntry(ProgramEntry entry) {
            // Validate entry object: Not null
            if (entry == null)
                return "Entry cannot be null.";

            // Validate name: Not null, not empty
            if (entry.Name == null || entry.Name.Trim() == "")
                return "Entry name cannot be empty.";

            // Validate OS: In supported OSes list
            if (entry.OS == null || !supportedOses.Contains(entry.OS))
                return "Entry OS must be in the list of supported OSes.";

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

        /// <summary>
        /// Helper function that checks if a name is present in ProgramEntry.
        /// </summary>
        /// <param name="programName">The name to be checked.</param>
        /// <returns>True if name is in use, false if not.</returns>
        private bool IsNameInUse(string programName) {
            List<ProgramEntry> entries = GetProgramEntryList();
            if (entries != null) {
                foreach (ProgramEntry entry in entries) {
                    if (entry.Name == programName) {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Delete a program from the Program table.
        /// </summary>
        /// <param name="programName">The name of the program to be deleted.</param>
        /// <returns>Boolean success or failure.</returns>
        private bool DeleteProgram(string programName) {
            try {
                db.Delete("Program", String.Format("ProgramName = '{0}'", programName));
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Add a new Program to the database.
        /// </summary>
        /// <param name="programName">Name of the program being added</param>
        /// <returns>Boolean success or failure</returns>
        private bool AddProgram(string programName)
        {
            // Prepare data for db
            Dictionary<string, string> insert = new Dictionary<string, string>();
            insert.Add("ProgramName", programName);

            // Insert into db
            try
            {
                db.Insert("Program", insert);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Add a program entry to the database.
        /// The program entry list contains information about every version
        /// of a program whereas the program list is simply a list of supported programs.
        /// </summary>
        /// <param name="entry">ProgramEntry object.</param>
        /// <returns>Bool, success or failure</returns>
        public bool AddProgramEntry(ProgramEntry entry) {
            // Validate entry
            string validation = ValidateProgramEntry(entry);
            if (validation != "") {
                Console.WriteLine(validation);
                return false;
            }

            // Convert paths to a JSON string
            string json_paths = JsonConvert.SerializeObject(entry.Paths);
            Console.WriteLine(json_paths);

            // convert isPermanent to an int for the db
            int isPermInt = 0;
            if (entry.IsPermanent) isPermInt = 1;

            // Prepare the data for db
            Dictionary<string, string> insert = new Dictionary<string, string>();
            insert.Add("Name", entry.Name.Trim());
            insert.Add("Version", entry.Version.Trim());
            insert.Add("OS", entry.OS.Trim());
            insert.Add("IsPermanent", isPermInt.ToString());
            insert.Add("Description", entry.Description.Trim());
            insert.Add("Paths", json_paths);

            // Insert into db
            // If there's a problem, the exception is output to console. If we need to output to GUI, we need to pass that info back from SQLiteDatabase.cs
            try {
                db.Insert("ProgramEntry", insert);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }

            // If this program name doesn't exist in Program table, add to that too
            List<string> names = GetProgramNameList();
            if (names == null || !names.Contains(entry.Name)) {
                AddProgram(entry.Name);
            }

            return true;
        }

        /// <summary>
        /// Edit a program already in the database.
        /// IsPermanent is not an editable field
        /// </summary>
        /// <param name="entry">The ProgramEntry object.</param>
        /// <returns>Boolean success or failure.</returns>
        public bool EditProgramEntry(ProgramEntry entry) 
        {
            // Make sure there's something to update
            if ((entry.Name == null || entry.Name.Trim() == "") &&
                entry.Version == null &&
                entry.OS == null &&
                entry.Paths == null &&
                entry.Description == null) {
                Console.WriteLine("Nothing to update! Returning...");
                return false;
            }

            // Make sure not IsPermanent
            ProgramEntry old_entry = GetProgramEntry(entry.Program_ID);
            if (old_entry == null || old_entry.IsPermanent)
            {
                return false;
            }

            // Prepare the data for db
            Dictionary<string, string> update = new Dictionary<string, string>();

            // Optional parameter: Add Name
            if (entry.Name != null && entry.Name.Trim() != "") 
                update.Add("Name", entry.Name);

            // Optional parameter: Add Version
            if (entry.Version != null) { update.Add("Version", entry.Version); }

            // Optional parameter: Add OS
            if (entry.OS != null) { update.Add("OS", entry.OS); }

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
                db.Update("ProgramEntry", update, String.Format("Program_ID = '{0}'", entry.Program_ID));
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }

            // If this program name doesn't exist in Program table, add to that too
            if (entry.Name != null) {
                List<string> names = GetProgramNameList();
                if (names == null || !names.Contains(entry.Name)) {
                    AddProgram(entry.Name);
                }

                // If old name doesn't exist anymore, delete it!
                if (!IsNameInUse(old_entry.Name)) {
                    DeleteProgram(old_entry.Name);
                }
            }

            return true;
        }

        /// <summary>
        /// Delete a ProgramEntry from the ProgramEntry table.
        /// </summary>
        /// <param name="program_ID">The unique ID referencing a ProgramEntry.</param>
        /// <returns>Boolean success or failure.</returns>
        public bool DeleteProgramEntry(int program_ID) {
            // Retrieve the entry
            ProgramEntry old_entry = GetProgramEntry(program_ID);

            try {
                db.Delete("ProgramEntry", String.Format("Program_ID = '{0}'", program_ID));
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }

            // If old name doesn't exist anymore, delete it!
            if (!IsNameInUse(old_entry.Name)) {
                DeleteProgram(old_entry.Name);
            }

            return true;
        }

        /// <summary>
        /// Retrieve a list of program entries from the database.
        /// </summary>
        /// <returns>A list of ProgramEntry retrieved from the db.</returns>
        public List<ProgramEntry> GetProgramEntryList() {
            String query = "SELECT Program_ID,Name,Version,OS,IsPermanent,Description,Paths FROM ProgramEntry;";
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
                Console.WriteLine("No rows returned for GetProgramEntryList.");
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


        /// <summary>
        /// Retrieve a list of program entries from the database, filtered by program name.
        /// </summary>
        /// <param name="name">A specific name to search for.</param>
        /// <returns>A list of ProgramEntry retrieved from the db.</returns>
        public List<ProgramEntry> GetProgramEntryList(string name) {
            String query = string.Format("SELECT Program_ID,Name,Version,OS,IsPermanent,Description,Paths FROM ProgramEntry WHERE Name='{0}';", name);
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
                Console.WriteLine("No rows returned for GetProgramEntryList.");
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

        /// <summary>
        /// Retrieve a list of program names from the database.
        /// </summary>
        /// <returns>A list of names. One entry per row in the table.</returns>
        public List<string> GetProgramNameList() {
            // Fetch the global list..not using this function's parameter at the moment
            String query = "SELECT ProgramName FROM Program;";
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
            foreach (DataRow row in dt.Rows) {
                names.Add(row["ProgramName"].ToString());
            }
            
            return names;
        }

        /// <summary>
        /// Get one entry from the ProgramEntry table in the db.
        /// </summary>
        /// <param name="program_ID">The unique program ID number.</param>
        /// <returns>A ProgramEntry
        /// Program_ID, Name, Version, OS, IsPermanent, Description, Paths, Foreign Key</returns>
        public ProgramEntry GetProgramEntry(int program_ID)
        {
            // query the database for row containing program_ID
            String query = string.Format("SELECT * FROM ProgramEntry WHERE Program_ID='{0}';", program_ID);
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
                Console.WriteLine("No rows returned for Program_ID == {0}", program_ID);
                return null;
            }

            // create a ProgramEntry object from the DataTable
            ProgramEntry entry = new ProgramEntry(dt.Rows[0]);
            
            return entry;
        }

        /// <summary>
        /// Search the db for a specific Name,Version,OS and return its Program_ID
        /// </summary>
        /// <param name="Name">Program name in search</param>
        /// <param name="Version">Program version</param>
        /// <param name="OS">Program OS</param>
        /// <returns>The unique Program ID, -1 on error.</returns>
        public int GetProgram_ID(string Name, string Version, string OS)
        {
            // Query the database for Name, Version, OS
            String query = string.Format("SELECT Program_ID FROM ProgramEntry WHERE Name='{0}' AND Version='{1}' AND OS='{2}'", Name, Version, OS);
            DataTable dt;
            try
            {
                dt = db.GetDataTable(query);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }

            // Make sure rows were returned
            if (dt.Rows.Count < 1) {
                Console.WriteLine("No rows returned for {0}, {1}, {2}", Name, Version, OS);
                return -1;
            }

            // get the program ID and return it.
            DataRow row = dt.NewRow();
            row = dt.Rows[0];

            string id_str = row["Program_ID"].ToString();
            return Globals.StrToInt(id_str);
        }
    }
}