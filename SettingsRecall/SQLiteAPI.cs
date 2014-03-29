﻿using System;
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

            // Validate paths: Must be at least length 1
            if (entry.Paths.Count == 0)
                return "Entry paths must contain at least one path.";

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

            // Convert IsPermanent to string (int representation)
            string isPermanent = entry.IsPermanent ? "1" : "0";

            // Convert paths to a JSON string
            string json_paths = JsonConvert.SerializeObject(entry.Paths);

            // Prepare the data for db
            Dictionary<string, string> insert = new Dictionary<string, string>();
            insert.Add("Name", entry.Name.Trim());
            insert.Add("IsPermanent", isPermanent);
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
            // TODO: Use a changeset class so that the properties can be nullable
            // That will make this a lot less confusing

            // Make sure there's something to update
            if (entry.Paths.Count == 0 && entry.Description == "") {
                Console.WriteLine("Nothing to update! Returning...");
                return false;
            }

            // Prepare the data for db
            Dictionary<string, string> update = new Dictionary<string, string>();

            // Unnullable parameter: Add isPermanent
            string isPermanent = entry.IsPermanent ? "1" : "0";
            update.Add("IsPermanent", isPermanent);

            // Optional parameter: Add description
            if (entry.Description != "") update.Add("Description", entry.Description);

            // Optional parameter: Convert paths to a JSON string
            if (entry.Paths.Count != 0) {
                string json_paths = JsonConvert.SerializeObject(entry.Paths);
                update.Add("Paths", json_paths);
                Console.WriteLine(json_paths);
            }

            // Make sure something is being updated
            if (update.Count == 0) {
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
            string query = "SELECT Name,IsPermanent,Paths,Description FROM Program;";
            DataTable dt;
            List<ProgramEntry> entryList = new List<ProgramEntry>();

            try {
                dt = db.GetDataTable(query);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return entryList;
            }

            // Make sure rows were returned
            if (dt.Rows.Count == 0) {
                Console.WriteLine("No rows returned for GetProgramList.");
                return entryList;
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
            List<string> names = new List<string>();

            string query = "SELECT Name FROM Program;";
            DataTable dt;
            try {
                dt = db.GetDataTable(query);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return names;
            }

            // Make sure rows were returned
            if (dt.Rows.Count < 1) {
                Console.WriteLine("No rows returned for GetProgramNameList.");
                return names;
            }

            // Convert datatable to list
            foreach (DataRow row in dt.Rows)
                names.Add(row["Name"].ToString());
            
            return names;
        }

        /// <summary>
        /// Get one entry from the ProgramEntry table in the db.
        /// </summary>
        public ProgramEntry GetProgram(string name)
        {
            // query the database for row containing program_ID
            string query = string.Format("SELECT Name,IsPermanent,Paths,Description FROM Program WHERE Name = '{0}';", name);
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