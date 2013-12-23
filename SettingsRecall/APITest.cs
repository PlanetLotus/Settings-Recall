using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SettingsRecall {
    using NUnit.Framework;

    [TestFixture]
    public class APITest {
        SQLiteDatabase db;
        SQLiteAPI testAPI;

        [TestFixtureSetUp]
        public void Init() {
            // Create a testing database
            db = new SQLiteDatabase("unittest.db");

            // Create some test data directly in the db, without the API
            Dictionary<string, Dictionary<string, string>> dict = new Dictionary<string, Dictionary<string, string>>();
            dict.Add("xp", new Dictionary<string, string>() { { "1", "xp/path/to/file1.txt" }, { "2", "xp/path/to/file2.txt" } });
            dict.Add("vista", new Dictionary<string,string>() { {"1", "vista/path/to/file1.txt"}});
            string json_paths = JsonConvert.SerializeObject(dict);

            Dictionary<string, string> insert = new Dictionary<string, string>();
            insert.Add("Name", "testprogram1");
            insert.Add("Paths", json_paths);
            insert.Add("Description", "testdescription1");
            db.Insert("Program", insert);   // Should we wrap this in try/catch?

            // Link the API to the testing database
            testAPI = new SQLiteAPI("unittest.db");
        }

        [TestFixtureTearDown]
        public void Cleanup() {
            // Delete all data in the db
            db.ClearDB();
        }
    }
}
