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
            db.Insert("ProgramEntry", insert);

            // Link the API to the testing database
            testAPI = new SQLiteAPI(db_file);
        }

        [TestFixtureTearDown]
        public void Cleanup() {
            // Delete all data in the db
            db.ClearDB();
        }

        [TestCase(1000)]
        [TestCase(-1)]
        public void Test_GetNonExistentProgramEntry(int program_ID) {
            Assert.IsNull(testAPI.GetProgramEntry(program_ID));
        }

        [TestCase(1)]
        public void Test_GetProgramEntry(int program_ID) {
            Assert.IsNotNull(testAPI.GetProgramEntry(program_ID));
        }

        [Test]
        // This test depends on assuming GetProgramEntryList works properly
        public void Test_AddDuplicateProgramEntry() {
            // Get initial count
            int programEntryCount = testAPI.GetProgramEntryList().Count;

            // Build new object
            List<string> paths = new List<string>() { "xp/path/to/file1.txt" };
            ProgramEntry programEntry = new ProgramEntry("testprogram1", "1.0", "XP", paths, "Really useful test description", false);
            testAPI.AddProgramEntry(programEntry);

            // Verify the number of program entries didn't change
            Assert.AreEqual(programEntryCount, testAPI.GetProgramEntryList().Count);
        }

        [Test]
        // This test depends on assuming GetProgramEntryList works properly
        // This test depends on assuming GetProgramNameList works properly
        public void Test_AddProgramEntry() {
            int programEntryCount = testAPI.GetProgramEntryList().Count;

            // Create object
            List<string> paths = new List<string>() { "vista/path/to/file4.txt" };
            ProgramEntry programEntry = new ProgramEntry("apiTestEntry", "1.0", "Vista", paths, "Really useful test description", false);

            // Add program
            testAPI.AddProgramEntry(programEntry);

            // Make sure it was added by calling GetProgramEntryList
            // There should be two entries now
            List<ProgramEntry> list = testAPI.GetProgramEntryList();
            Assert.IsNotNull(list);
            Assert.AreEqual(programEntryCount + 1, list.Count);

            // Make sure the name exists once in the Program table
            List<string> names = testAPI.GetProgramNameList();
            Assert.IsNotNull(names);
            Assert.Contains("apiTestEntry", names);
        }
    }
}
