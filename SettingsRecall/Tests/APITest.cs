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
        string db_file = "../../integrationtest.db";
        SQLiteDatabase db;
        SQLiteAPI testAPI;

        [TestFixtureSetUp]
        public void Init() {
            // Create a testing database
            Console.WriteLine("Initializing tests...");
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
            insert.Add("IsPermanent", "0");
            insert.Add("Paths", json_paths);
            insert.Add("Description", "testdescription1");
            db.Insert("Program", insert);

            // Link the API to the testing database
            testAPI = new SQLiteAPI(db_file);
        }

        [TestFixtureTearDown]
        public void Cleanup() {
            // Delete all data in the db
            Console.WriteLine("Cleaning up tests...");
            db.ClearDB();
        }

        [TestCase("asdfasdfasdf")]
        [TestCase("")]
        public void Test_GetNonExistentProgramEntry(string name) {
            Assert.IsNull(testAPI.GetProgram(name));
        }

        [TestCase("testprogram1")]
        public void Test_GetProgramEntry(string name) {
            Assert.IsNotNull(testAPI.GetProgram(name));
        }

        public void Test_GetProgramList() {
            List<ProgramEntry> entryList = testAPI.GetProgramList(); 

            Assert.IsNotNull(entryList, "Test failed: GetProgramEntryList with no parameters returned null");
            Assert.GreaterOrEqual(entryList.Count, 1, "Test failed: GetProgramEntryList returned less than 1 entry.");
        }

        [Test]
        public void Test_GetProgramNameList() {
            List<string> names = testAPI.GetProgramNameList();

            Assert.IsNotNull(names, "Test failed: GetProgramNameList returned null");
            Assert.GreaterOrEqual(names.Count, 1, "Test failed: GetProgramNameList returned less than 1 entry.");
        }

        [TestCase("")]
        [TestCase("    ")]
        public void Test_AddProgramBadName(string name) {
            List<string> paths = new List<string>() { "xp/path/to/file1.txt" };
            ProgramEntry programEntry = new ProgramEntry(name, false, paths, "Really useful test description");

            // Verify failure
            Assert.IsFalse(testAPI.AddProgram(programEntry));
        }

        [Test]
        public void Test_AddProgramBadPaths() {
            List<string> paths1 = new List<string>() { null, "path2" };
            List<string> paths2 = new List<string>() { "path1", "" };
            List<string> paths3 = new List<string>() { "path1", "   "};

            // Verify failure
            ProgramEntry programEntry1 = new ProgramEntry("badtestprogram", false, new List<string>(), "Really useful test description");
            Assert.IsFalse(testAPI.AddProgram(programEntry1));

            ProgramEntry programEntry2 = new ProgramEntry("badtestprogram", false, paths1, "Really useful test description");
            Assert.IsFalse(testAPI.AddProgram(programEntry2));

            ProgramEntry programEntry3 = new ProgramEntry("badtestprogram", false, paths2, "Really useful test description");
            Assert.IsFalse(testAPI.AddProgram(programEntry3));

            ProgramEntry programEntry4 = new ProgramEntry("badtestprogram", false, paths3, "Really useful test description");
            Assert.IsFalse(testAPI.AddProgram(programEntry4));
        }

        [Test]
        // This test depends on assuming GetProgramEntryList works properly
        public void Test_AddDuplicateProgram() {
            // Get initial count
            int programEntryCount = testAPI.GetProgramList().Count;

            // Build new object
            List<string> paths = new List<string>() { "xp/path/to/file1.txt" };
            ProgramEntry programEntry = new ProgramEntry("testprogram1", false, paths, "Really useful test description");
            testAPI.AddProgram(programEntry);

            // Verify the number of program entries didn't change
            Assert.AreEqual(programEntryCount, testAPI.GetProgramList().Count);
        }

        [Test]
        // This test depends on assuming GetProgramEntryList works properly
        // This test depends on assuming GetProgramNameList works properly
        public void Test_AddProgram() {
            int programEntryCount = testAPI.GetProgramList().Count;

            // Create object
            List<string> paths = new List<string>() { "vista/path/to/file4.txt" };
            ProgramEntry programEntry = new ProgramEntry("apiTestEntry", false, paths, "Really useful test description");

            // Add program
            testAPI.AddProgram(programEntry);

            // Make sure it was added by calling GetProgramEntryList
            // There should be two entries now
            List<ProgramEntry> list = testAPI.GetProgramList();
            Assert.IsNotNull(list);
            Assert.AreEqual(programEntryCount + 1, list.Count);

            // Make sure the name exists once in the Program table
            List<string> names = testAPI.GetProgramNameList();
            Assert.IsNotNull(names);
            Assert.Contains("apiTestEntry", names);
        }

        [TestCase("")]
        [TestCase("    ")]
        public void Test_EditProgramBadName(string name) {
            ProgramEntry programEntry = new ProgramEntry(name, false, new List<string>());

            // Verify failure
            Assert.IsFalse(testAPI.EditProgram(programEntry));
        }

        [Test]
        public void Test_EditProgramBadPaths() {
            string name = "testprogram1";
            List<string> paths1 = new List<string>() { null, "path2" };
            List<string> paths2 = new List<string>() { "path1", "" };
            List<string> paths3 = new List<string>() { "path1", "   "};

            // Verify failure
            ProgramEntry programEntry1 = new ProgramEntry(name, false, paths1, "");
            Assert.IsFalse(testAPI.AddProgram(programEntry1));

            ProgramEntry programEntry2 = new ProgramEntry(name, false, paths2, "");
            Assert.IsFalse(testAPI.AddProgram(programEntry2));

            ProgramEntry programEntry3 = new ProgramEntry(name, false, paths3, "");
            Assert.IsFalse(testAPI.AddProgram(programEntry3));
        }

        [Test]
        public void Test_EditProgram() {
            // Create object
            List<string> paths = new List<string>() { "vista/path/to/file9.txt" };
            ProgramEntry programEntry = new ProgramEntry("testprogram1", false, paths, "Really useful edited test description");

            // Edit entry
            testAPI.EditProgram(programEntry);

            // Find entry in list
            ProgramEntry editedEntry = testAPI.GetProgram("testprogram1");

            // Check if edited object is equal to test object
            Assert.AreEqual(programEntry.ToString(), editedEntry.ToString());

            // Re-initialize so we don't lose the data
            this.Init();
        }

        [Test]
        public void Test_DeleteProgram() {
            Assert.IsTrue(testAPI.DeleteProgram("testprogram1"));

            // Re-initialize so that we don't lose the data
            this.Init();
        }
    }
}
