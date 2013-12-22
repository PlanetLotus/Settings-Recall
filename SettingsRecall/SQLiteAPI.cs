using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SettingsRecall {
    class SQLiteAPI {
        SQLiteDatabase db;

        /// <summary>
        /// Default constructor for SQLiteAPI Class.
        /// Creates a new SQLiteDatabase object.
        /// </summary>
        public SQLiteAPI() {
            db = new SQLiteDatabase();
        }

        /// <summary>
        /// Add a new program to the database.
        /// </summary>
        /// <param name="programName">The name of the program to be added.</param>
        /// <param name="paths">A series of paths to program settings files.</param>
        /// <param name="description">An optional description of the program.</param>
        /// <returns>Boolean success or failure.</returns>
        public bool AddProgram(string programName, Dictionary<string, Dictionary<string, string>> paths, string description) {
            // Convert paths to a JSON string
            string json_paths = JsonConvert.SerializeObject(paths);
            Console.WriteLine(json_paths);

            // Prepare the data for db
            Dictionary<string, string> insert = new Dictionary<string, string>();
            insert.Add("Name", programName);
            insert.Add("Paths", json_paths);
            insert.Add("Description", description);

            // Insert into db
            try {
                db.Insert("Program", insert);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Edit a program already in the database.
        /// </summary>
        /// <param name="programName">The name of the program to be edited.</param>
        /// <param name="paths">A list of paths to program settings files.</param>
        /// <param name="description">An optional description of the program.</param>
        /// <returns>Boolean success or failure.</returns>
        public bool EditProgram(string programName, Dictionary<string, Dictionary<string, string>> paths=null, string description=null) {
            // Make sure there's something to update
            if (paths == null && description == null) {
                Console.WriteLine("Nothing to update! Returning...");
                return false;
            }

            // Prepare the data for db
            Dictionary<string, string> update = new Dictionary<string, string>();

            // Optional parameter: Convert paths to a JSON string
            if (paths != null) {
                string json_paths = JsonConvert.SerializeObject(paths);
                update.Add("Paths", json_paths);
                Console.WriteLine(json_paths);
            }

            // Optional parameter: Add description
            if (description != null) {
                update.Add("Description", description);
            }

            // Insert into db
            try {
                db.Update("Program", update, String.Format("Name = '{0}'", programName));
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
