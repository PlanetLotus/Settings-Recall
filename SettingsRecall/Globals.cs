using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsRecall
{
    // Global Variables and Constants
    public static class Globals
    {
        public static SQLiteDatabase sqlite_db;
        public static SQLiteAPI sqlite_api;
        public static string load_save_location; // Where backup files are saved
    }
}
