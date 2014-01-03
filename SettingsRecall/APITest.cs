using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SettingsRecall {
    using NUnit.Framework;

    [TestFixture]
    public class APITest {
        string db_file = "../../unittest.db";
        SQLiteDatabase db;
        SQLiteAPI testAPI;

        [TestFixtureSetUp]
        public void Init() {
            // Create a testing database
            db = new SQLiteDatabase(db_file);
            db.ClearDB();

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
            db.Insert("ProgramEntry", insert);   // Should we wrap this in try/catch?

            // Link the API to the testing database
            testAPI = new SQLiteAPI(db_file);
        }

        [TestFixtureTearDown]
        public void Cleanup() {
            // Delete all data in the db
            db.ClearDB();
        }

        [Test]
        public void Test_AddProgramEntry() {
            // This test depends on assuming GetProgramEntryList works properly
            // This test depends on assuming GetProgramNameList works properly

            // Test: Add a duplicate entry -- SHOULD NOT ADD ANYTHING
            List<string> paths = new List<string>();
            paths.Add("");
            testAPI.AddProgramEntry("testprogram1", "1.0", "XP", false, paths);
            paths.Clear();

            // Test: Add another, legitimate entry
            paths.Add("vista/path/to/file4.txt");
            string json_paths = JsonConvert.SerializeObject(paths);
            Dictionary<string, string> insert = new Dictionary<string, string>() {
                {"Name", "testprogram1"},
                {"Version", "1.1"},
                {"OS", "Vista"},
                {"IsPermanent", "0"},
                {"Paths", json_paths},
                {"Description", ""}
            };
            db.Insert("ProgramEntry", insert);

            // Make sure it was added by calling GetProgramEntryList
            // There should be two entries now
            DataTable dt = testAPI.GetProgramEntryList();
            Assert.IsNotNull(dt);
            Assert.IsNotNull(dt.Rows);
            Assert.AreEqual(dt.Rows.Count, 2);

            // Make sure the name exists once in the Program table
            List<string> names = testAPI.GetProgramNameList();
            Assert.AreEqual(names.Count, 1);
            Assert.AreEqual(names[0], "testprogram1");
        }
    }
}
