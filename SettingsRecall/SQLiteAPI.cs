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
        /// Add a new Program to the database.
        /// </summary>
        /// <param name="programName">Name of the program being added</param>
        /// <returns>Boolean success of failure</returns>
        public bool AddProgram(string programName)
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

        // NEEDS TO CHECK FOR DUPES in name/version/os
        /// <summary>
        /// Add a program entry to the database.
        /// The program entry list contains information about every version
        /// of a program whereas the program list is simply a list of supported programs.
        /// </summary>
        /// <param name="programName">Name of the program being added.</param>
        /// <param name="programVersion">Program version name.</param>
        /// <param name="OS">Entry's intended OS.</param>
        /// <param name="isPermanent">Will this entry be editable?</param>
        /// <param name="description">Optional description of entry.</param>
        /// <param name="paths">A list of all paths to preference files.</param>
        /// <returns>Bool success or failure</returns>
        public bool AddProgramEntry(
            string programName, 
            string programVersion, 
            string OS, 
            bool isPermanent, 
            string description, 
            Dictionary<string, string> paths) {

            // Convert paths to a JSON string
            string json_paths = JsonConvert.SerializeObject(paths);
            Console.WriteLine(json_paths);

            // convert isPermanent to an int
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
            try {
                db.Insert("ProgramEntry", insert);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        // STILL WORK IN PROGRESS....
        /// <summary>
        /// Edit a program already in the database.
        /// IsPermanent is not an editable field
        /// </summary>
        /// <param name="programName">The name of the program to be edited.</param>
        /// <param name="paths">A list of paths to program settings files.</param>
        /// <param name="description">An optional description of the program.</param>
        /// <returns>Boolean success or failure.</returns>
        public bool EditProgramEntry(
            string programName, 
            string programVersion = null, 
            string OS = null, 
            string description=null,
            Dictionary<string, string> paths=null) {

            // Make sure there's something to update
            if (programVersion == null &&
                OS == null &&
                paths == null &&
                description == null) {
                Console.WriteLine("Nothing to update! Returning...");
                return false;
            }

            // Prepare the data for db
            Dictionary<string, string> update = new Dictionary<string, string>();

            // Optional parameter: Add Version
            if (programVersion != null)
            {
                update.Add("Version", programVersion);
            }

            // Optional parameter: Add OS
            if (OS != null)
            {
                update.Add("OS", OS);
            }

            // Optional parameter: Add description
            if (description != null) {
                update.Add("Description", description);
            }

            // Optional parameter: Convert paths to a JSON string
            if (paths != null) {
                string json_paths = JsonConvert.SerializeObject(paths);
                update.Add("Paths", json_paths);
                Console.WriteLine(json_paths);
            }

            // Insert into db
            try {
                db.Update("ProgramEntry", update, String.Format("Name = '{0}'", programName));
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Delete a program from the database.
        /// </summary>
        /// <param name="programName">The name of the program to be deleted.</param>
        /// <returns>Boolean success or failure.</returns>
        public bool DeleteProgram(string programName) {
            // This functionality could be extended to delete a program given a path or description, too...
            try {
                db.Delete("Program", String.Format("Name = '{0}'", programName));
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Retrieve a list of programs from the database.
        /// </summary>
        /// <param name="list">Which list to retrieve.</param>
        /// <returns>The DataTable retrieved from the db.</returns>
        public DataTable GetProgramList(string list) {
            // Not currently sure whether there will be more than one list in the db. Might just be the global list.

            // Fetch the global list..not using this function's parameter at the moment
            String query = "SELECT Name, Paths, Description FROM Program;";
            DataTable dt;
            try {
                dt = db.GetDataTable(query);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return null;
            }

            return dt;
        }

        /// <summary>
        /// Retrieve a list of program names from the database.
        /// </summary>
        /// <param name="list">Which list to retrieve.</param>
        /// <returns>A list of names. One entry per row in the table.</returns>
        public List<string> GetProgramNameList(string list) {
            // Not currently sure whether there will be more than one list in the db. Might just be the global list.

            // Fetch the global list..not using this function's parameter at the moment
            String query = "SELECT Name FROM Program;";
            DataTable dt;
            try {
                dt = db.GetDataTable(query);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return null;
            }

            // Convert datatable to list
            List<string> names = new List<string>();
            foreach (DataRow row in dt.Rows) {
                names.Add(row["Name"].ToString());
            }

            return names;
        }
    }
}
