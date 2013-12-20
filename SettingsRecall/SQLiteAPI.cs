﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <param name="paths">A list of paths to program settings files.</param>
        /// <param name="description">An optional description of the program.</param>
        /// <returns>Boolean success or failure.</returns>
        public bool AddProgram(string programName, List<string> paths, string description) {
            return true;
        }

        /// <summary>
        /// Edit a program already in the database.
        /// </summary>
        /// <param name="programName">The name of the program to be edited.</param>
        /// <param name="paths">A list of paths to program settings files.</param>
        /// <param name="description">An optional description of the program.</param>
        /// <returns>Boolean success or failure.</returns>
        public bool EditProgram(string programName, List<string> paths, string description) {
            return true;
        }

        /// <summary>
        /// Delete a program from the database.
        /// </summary>
        /// <param name="programName">The name of the program to be deleted.</param>
        /// <returns>Boolean success or failure.</returns>
        public bool DeleteProgram(string programName) {
            return true;
        }

        /// <summary>
        /// Retrieve a list of programs from the database.
        /// </summary>
        /// <param name="list">Which list to retrieve.</param>
        /// <returns></returns>
        public List<string> GetProgramList(string list) {
            // Not currently sure whether there will be more than one list in the db. Might just be the global list.
            return null;
        }
    }
}