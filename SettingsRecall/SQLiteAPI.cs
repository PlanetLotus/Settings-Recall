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

        /// <summary>
        /// Default constructor for SQLiteAPI Class.
        /// Creates a new SQLiteDatabase object.
        /// </summary>
        public SQLiteAPI() {
            db = new SQLiteDatabase();
        }

        /// <summary>
        /// Creates a new SQLiteDatabase object with specified database file.
        /// </summary>
        /// <param name="dbInputFile"></param>
        public SQLiteAPI(string dbInputFile) {
            db = new SQLiteDatabase(dbInputFile);
        }

        /// <summary>
        /// Helper function that checks if a name is present in ProgramEntry.
        /// </summary>
        /// <param name="programName">The name to be checked.</param>
        /// <returns>True if name is in use, false if not.</returns>
        private bool IsNameInUse(string programName) {
            DataTable entries = GetProgramEntryList();
            if (entries != null) {
                foreach (DataRow entry in entries.Rows) {
                    if (entry["Name"].ToString() == programName) {
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
            // Convert paths to a JSON string
            string json_paths = JsonConvert.SerializeObject(entry.Paths);
            Console.WriteLine(json_paths);

            // convert isPermanent to an int for the db
            int isPermInt = 0;
            if (entry.IsPermanent) isPermInt = 1;

            // Prepare the data for db
            Dictionary<string, string> insert = new Dictionary<string, string>();
            insert.Add("Name", entry.Name);
            insert.Add("Version", entry.Version);
            insert.Add("OS", entry.OS);
            insert.Add("IsPermanent", isPermInt.ToString());
            insert.Add("Description", entry.Description);
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
        /// Add a program entry to the database.
        /// The program entry list contains information about every version
        /// of a program whereas the program list is simply a list of supported programs.
        /// </summary>
        /// <param name="programName">Name of the program being added.</param>
        /// <param name="programVersion">Program version name.</param>
        /// <param name="OS">Entry's intended OS.</param>
        /// <param name="isPermanent">Will this entry be editable?</param>
        /// <param name="paths">A list of all paths to preference files.</param>
        /// <param name="description">Optional description of entry.</param>
        /// <returns>Bool success or failure</returns>
        public bool AddProgramEntry(
            string programName, 
            string programVersion, 
            string OS, 
            bool isPermanent, 
            List<string> paths,
            string description="") { 

            // Convert paths to a JSON string
            string json_paths = JsonConvert.SerializeObject(paths);
            Console.WriteLine(json_paths);

            // convert isPermanent to an int for the db
            int isPermInt = 0;
            if (isPermanent) isPermInt = 1;

            // Prepare the data for db
            Dictionary<string, string> insert = new Dictionary<string, string>();
            insert.Add("Name", programName);
            insert.Add("Version", programVersion);
            insert.Add("OS", OS);
            insert.Add("IsPermanent", isPermInt.ToString());
            insert.Add("Description", description);
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
            if (names == null || !names.Contains(programName)) {
                AddProgram(programName);
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
            if (entry.Name == null &&
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
            if (entry.Name != null) { update.Add("Name", entry.Name); }

            // Optional parameter: Add Version
            if (entry.Version != null) { update.Add("Version", entry.Version); }

            // Optional parameter: Add OS
            if (entry.OS != null) { update.Add("OS", entry.OS); }

            // Optional parameter: Add description
            if (entry.Description != null) { update.Add("Description", entry.Description); }

            // Optional parameter: Convert paths to a JSON string
            if (entry.Paths != null) {
                string json_paths = JsonConvert.SerializeObject(entry.Paths);
                update.Add("Paths", json_paths);
                Console.WriteLine(json_paths);
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
        /// Edit a program already in the database.
        /// IsPermanent is not an editable field
        /// </summary>
        /// <param name="program_ID">The ID # of the program - not editable.</param>
        /// <param name="programName">The new name of the program to be edited.</param>
        /// <param name="paths">A new list of paths to program settings files.</param>
        /// <param name="description">A new optional description of the program.</param>
        /// <returns>Boolean success or failure.</returns>
        public bool EditProgramEntry(
            int program_ID,
            string programName = null, 
            string programVersion = null, 
            string OS = null, 
            string description=null,
            List<string> paths=null) {

            // Make sure there's something to update
            if (programName == null &&
                programVersion == null &&
                OS == null &&
                paths == null &&
                description == null) {
                Console.WriteLine("Nothing to update! Returning...");
                return false;
            }

            // Make sure not IsPermanent
            ProgramEntry old_entry = GetProgramEntry(program_ID);
            if (old_entry == null || old_entry.IsPermanent)
            {
                return false;
            }

            // Prepare the data for db
            Dictionary<string, string> update = new Dictionary<string, string>();

            // Optional parameter: Add Name
            if (programName != null) { update.Add("Name", programName); }

            // Optional parameter: Add Version
            if (programVersion != null) { update.Add("Version", programVersion); }

            // Optional parameter: Add OS
            if (OS != null) { update.Add("OS", OS); }

            // Optional parameter: Add description
            if (description != null) { update.Add("Description", description); }

            // Optional parameter: Convert paths to a JSON string
            if (paths != null) {
                string json_paths = JsonConvert.SerializeObject(paths);
                update.Add("Paths", json_paths);
                Console.WriteLine(json_paths);
            }

            // Insert into db
            try {
                db.Update("ProgramEntry", update, String.Format("Program_ID = '{0}'", program_ID));
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }

            // If this program name doesn't exist in Program table, add to that too
            if (programName != null) {
                List<string> names = GetProgramNameList();
                if (names == null || !names.Contains(programName)) {
                    AddProgram(programName);
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
        /// <returns>The DataTable retrieved from the db.</returns>
        public DataTable GetProgramEntryList() {
            String query = "SELECT Program_ID,Name,Version,OS,IsPermanent,Description,Paths FROM ProgramEntry;";
            DataTable dt;
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

            return dt;
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