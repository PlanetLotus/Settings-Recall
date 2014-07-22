using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsRecall {
    public static class Globals {
        public static SQLiteDatabase db;
        public static string load_save_location; // Where backup files are saved
        public static string dbLocation = "../../test.db";
    }
}
