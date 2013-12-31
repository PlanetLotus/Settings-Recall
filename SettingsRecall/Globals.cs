using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsRecall
{
    public static class Globals
    {
        // Global Variables and Constants

        public static SQLiteDatabase sqlite_db;
        public static SQLiteAPI sqlite_api;
        public static string load_save_location; // Where backup files are saved

        // Global Helpers

        /// <summary>
        /// Convert a number represented by a string to an int.
        /// </summary>
        /// <param name="numStr">String to be converted</param>
        /// <returns>Int representation of string.</returns>
        public static int StrToInt(string numStr) {
           // This function is simply to keep the error checking out of normal code to reduce clutter
           int numInt;
           try { numInt = Convert.ToInt32(numStr); }
           catch(FormatException) { return -1; }
           catch(OverflowException) { return -1; }

           return numInt;
       }

    }
}
