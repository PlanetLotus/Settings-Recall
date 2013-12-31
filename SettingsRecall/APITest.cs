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
        string db_file = "unittest.db";

        [TestFixtureSetUp]
        public void Init() {
            // Create a testing database
            db = new SQLiteDatabase(db_file);

            // Create some test data directly in the db, without the API
            List<string> paths = new List<string>();
            paths.Add("path/to/\"quoted filename.txt\"");
            paths.Add("path/to/file2.txt");
            paths.Add("path/to/file3.txt");
            string json_paths = JsonConvert.SerializeObject(paths);

            Dictionary<string, string> insert = new Dictionary<string, string>();
            insert.Add("Name", "testprogram1");
            insert.Add("Version", "1.0");
            insert.Add("OS", "XP");
            insert.Add("IsPermanent", "0");
            insert.Add("Paths", json_paths);
            insert.Add("Description", "testdescription1");
            db.Insert("Program", insert);   // Should we wrap this in try/catch?

            // Link the API to the testing database
            testAPI = new SQLiteAPI(db_file);
        }

        [TestFixtureTearDown]
        public void Cleanup() {
            // Delete all data in the db
            db.ClearDB();
        }

        [Test]
        public void TestAddProgram() {
            // This test depends on assuming GetProgramNameList works properly

            // Add a program name to the db
            testAPI.AddProgram("testAddProgram1");

            // Make sure it was added by calling GetProgramNameList
        }
    }
}
