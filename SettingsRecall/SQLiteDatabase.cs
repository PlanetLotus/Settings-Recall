﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace SettingsRecall {
    // This class was borrowed from:
    // http://www.dreamincode.net/forums/topic/157830-using-sqlite-with-c%23/
    public class SQLiteDatabase {
        string dbConnection;

        /// <summary>
        ///     Default Constructor for SQLiteDatabase Class.
        /// </summary>
        public SQLiteDatabase() {
            // This path will need updated when in production...should simply remove the ../../
            dbConnection = string.Format("Data Source={0}", Globals.dbLocation);
        }

        /// <summary>
        ///     Allows the programmer to run a query against the Database.
        /// </summary>
        /// <param name="sql">The SQL to run</param>
        /// <returns>A DataTable containing the result set.</returns>
        public virtual DataTable GetDataTable(string sql) {
            DataTable dt = new DataTable();
            try {
                Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
                SQLiteConnection cnn = new SQLiteConnection(dbConnection);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = sql;
                SQLiteDataReader reader = mycommand.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                cnn.Close();
            } catch (Exception e) {
                throw new Exception(e.Message);
            }
            return dt;
        }

        /// <summary>
        ///     Allows the programmer to interact with the database for purposes other than a query.
        /// </summary>
        /// <param name="sql">The SQL to be run.</param>
        /// <returns>An Integer containing the number of rows updated.</returns>
        public int ExecuteNonQuery(string sql) {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            cnn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = sql;
            int rowsUpdated = mycommand.ExecuteNonQuery();
            cnn.Close();
            return rowsUpdated;
        }

        /// <summary>
        ///     Allows the programmer to retrieve single items from the DB.
        /// </summary>
        /// <param name="sql">The query to run.</param>
        /// <returns>A string.</returns>
        public string ExecuteScalar(string sql) {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            cnn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = sql;
            object value = mycommand.ExecuteScalar();
            cnn.Close();
            if (value != null) {
                return value.ToString();
            }
            return "";
        }

        /// <summary>
        ///     Allows the programmer to easily update rows in the DB.
        /// </summary>
        /// <param name="tableName">The table to update.</param>
        /// <param name="data">A dictionary containing Column names and their new values.</param>
        /// <param name="where">The where clause for the update statement.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public virtual bool Update(string tableName, Dictionary<string, string> data, string where) {
            string vals = "";
            Boolean returnCode = true;
            if (data.Count >= 1) {
                foreach (KeyValuePair<string, string> val in data) {
                    vals += string.Format(" {0} = '{1}',", val.Key.ToString(), val.Value.ToString());
                }
                vals = vals.Substring(0, vals.Length - 1);
            }
            try {
                ExecuteNonQuery(string.Format("update {0} set {1} where {2};", tableName, vals, where));
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                returnCode = false;
            }
            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily delete rows from the DB.
        /// </summary>
        /// <param name="tableName">The table from which to delete.</param>
        /// <param name="where">The where clause for the delete.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public virtual bool Delete(string tableName, string where) {
            Boolean returnCode = true;
            try {
                ExecuteNonQuery(string.Format("delete from {0} where {1};", tableName, where));
            } catch (Exception fail) {
                Console.WriteLine(fail.Message);
                returnCode = false;
            }
            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily insert into the DB
        /// </summary>
        /// <param name="tableName">The table into which we insert the data.</param>
        /// <param name="data">A dictionary containing the column names and data for the insert.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public virtual bool Insert(string tableName, Dictionary<string, string> data) {
            string columns = "";
            string values = "";
            Boolean returnCode = true;
            foreach (KeyValuePair<string, string> val in data) {
                columns += string.Format(" {0},", val.Key.ToString());
                values += string.Format(" '{0}',", val.Value);
            }
            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);
            try {
                ExecuteNonQuery(string.Format("insert into {0}({1}) values({2});", tableName, columns, values));
            } catch (Exception fail) {
                Console.WriteLine(fail.Message);
                returnCode = false;
            }
            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily delete all data from the DB.
        /// </summary>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool ClearDB() {
            DataTable tables;
            try {
                tables = GetDataTable("select NAME from SQLITE_MASTER where type='table' order by NAME;");
                foreach (DataRow table in tables.Rows) {
                    ClearTable(table["NAME"].ToString());
                }
                return true;
            } catch {
                return false;
            }
        }

        /// <summary>
        ///     Allows the user to easily clear all data from a specific table.
        /// </summary>
        /// <param name="table">The name of the table to clear.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool ClearTable(string table) {
            try {
                ExecuteNonQuery(string.Format("delete from {0};", table));
                return true;
            } catch {
                return false;
            }
        }
    }
}