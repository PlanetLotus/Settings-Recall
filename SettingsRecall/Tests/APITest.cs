using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

// After making SQLiteDatabase.cs a static class, this shouldn't/doesn't work anymore! 
// Fat TODO: Make these tests mock tests so that they become unit tests.
namespace SettingsRecall {
    using NUnit.Framework;

    [TestFixture]
    public class APITest {
        string db_file = "../../integrationtest.db";

        [TestFixtureSetUp]
        public void Init() {
            // Create a testing database
            Console.WriteLine("Initializing tests...");
            SQLiteDatabase.ClearDB();

            // Create some test data directly in the db, without the API
            List<string> paths = new List<string>();
            paths.Add("path/to/\"quoted filename.txt\"");
            paths.Add("path/to/file2.txt");
            paths.Add("path/to/file3.txt");
            string json_paths = JsonConvert.SerializeObject(paths);

            Dictionary<string, string> insert = new Dictionary<string, string>();
            insert.Add("Name", "testprogram1");
            insert.Add("IsPermanent", "0");
            insert.Add("Paths", json_paths);
            insert.Add("Description", "testdescription1");
            SQLiteDatabase.Insert("Program", insert);
        }

        [TestFixtureTearDown]
        public void Cleanup() {
            // Delete all data in the db
            Console.WriteLine("Cleaning up tests...");
            SQLiteDatabase.ClearDB();
        }

        [TestCase("asdfasdfasdf")]
        [TestCase("")]
        public void Test_GetNonExistentProgramEntry(string name) {
            Assert.IsNull(SQLiteAPI.GetProgram(name));
        }

        [TestCase("testprogram1")]
        public void Test_GetProgramEntry(string name) {
            Assert.IsNotNull(SQLiteAPI.GetProgram(name));
        }

        public void Test_GetProgramList() {
            List<ProgramEntry> entryList = SQLiteAPI.GetProgramList(); 

            Assert.IsNotNull(entryList, "Test failed: GetProgramEntryList with no parameters returned null");
            Assert.GreaterOrEqual(entryList.Count, 1, "Test failed: GetProgramEntryList returned less than 1 entry.");
        }

        [Test]
        public void Test_GetProgramNameList() {
            List<string> names = SQLiteAPI.GetProgramNameList();

            Assert.IsNotNull(names, "Test failed: GetProgramNameList returned null");
            Assert.GreaterOrEqual(names.Count, 1, "Test failed: GetProgramNameList returned less than 1 entry.");
        }

        [Test]
        public void Test_AddProgramBadPaths() {
            // Verify failure
            ProgramEntry programEntry1 = new ProgramEntry("badtestprogram", false, new List<string>(), "Really useful test description");
            Assert.IsFalse(SQLiteAPI.AddProgram(programEntry1));
        }

        [Test]
        // This test depends on assuming GetProgramEntryList works properly
        public void Test_AddDuplicateProgram() {
            // Get initial count
            int programEntryCount = SQLiteAPI.GetProgramList().Count;

            // Build new object
            List<string> paths = new List<string>() { "xp/path/to/file1.txt" };
            ProgramEntry programEntry = new ProgramEntry("testprogram1", false, paths, "Really useful test description");
            SQLiteAPI.AddProgram(programEntry);

            // Verify the number of program entries didn't change
            Assert.AreEqual(programEntryCount, SQLiteAPI.GetProgramList().Count);
        }

        [Test]
        // This test depends on assuming GetProgramEntryList works properly
        // This test depends on assuming GetProgramNameList works properly
        public void Test_AddProgram() {
            int programEntryCount = SQLiteAPI.GetProgramList().Count;

            // Create object
            List<string> paths = new List<string>() { "vista/path/to/file4.txt" };
            ProgramEntry programEntry = new ProgramEntry("apiTestEntry", false, paths, "Really useful test description");

            // Add program
            SQLiteAPI.AddProgram(programEntry);

            // Make sure it was added by calling GetProgramEntryList
            // There should be two entries now
            List<ProgramEntry> list = SQLiteAPI.GetProgramList();
            Assert.IsNotNull(list);
            Assert.AreEqual(programEntryCount + 1, list.Count);

            // Make sure the name exists once in the Program table
            List<string> names = SQLiteAPI.GetProgramNameList();
            Assert.IsNotNull(names);
            Assert.Contains("apiTestEntry", names);
        }

        [Test]
        public void Test_EditProgram() {
            // Create object
            List<string> paths = new List<string>() { "vista/path/to/file9.txt" };
            ProgramEntry programEntry = new ProgramEntry("testprogram1", false, paths, "Really useful edited test description");

            // Edit entry
            SQLiteAPI.EditProgram(programEntry);

            // Find entry in list
            ProgramEntry editedEntry = SQLiteAPI.GetProgram("testprogram1");

            // Check if edited object is equal to test object
            Assert.AreEqual(programEntry.ToString(), editedEntry.ToString());

            // Re-initialize so we don't lose the data
            this.Init();
        }

        [Test]
        public void Test_DeleteProgram() {
            Assert.IsTrue(SQLiteAPI.DeleteProgram("testprogram1"));

            // Re-initialize so that we don't lose the data
            this.Init();
        }
    }
}
