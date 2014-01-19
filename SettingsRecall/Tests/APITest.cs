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
            insert.Add("Version", "1.0");
            insert.Add("OS", "Windows XP 32-bit");
            insert.Add("IsPermanent", "0");
            insert.Add("Paths", json_paths);
            insert.Add("Description", "testdescription1");
            db.Insert("ProgramEntry", insert);

            Dictionary<string, string> name_insert = new Dictionary<string, string>();
            name_insert.Add("ProgramName", "testprogram1");
            db.Insert("Program", name_insert);

            // Link the API to the testing database
            testAPI = new SQLiteAPI(db_file);
        }

        [TestFixtureTearDown]
        public void Cleanup() {
            // Delete all data in the db
            Console.WriteLine("Cleaning up tests...");
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

        [TestCase("", "", "")]
        [TestCase("Aptitude", "9001", "Debian")]
        public void Test_GetNonExistentProgramId(string name, string version, string os) {
            Assert.AreEqual(-1, testAPI.GetProgram_ID(name, version, os));
        }

        [Test]
        public void Test_GetProgramId() {
            Assert.AreEqual(1, testAPI.GetProgram_ID("testprogram1", "1.0", "Windows XP 32-bit"));
        }

        [TestCase(null)]
        [TestCase("testprogram1")]
        public void Test_GetProgramEntryList(string name) {
            List<ProgramEntry> entryList;

            if (name == null) {
                entryList = testAPI.GetProgramEntryList(); 
                Assert.IsNotNull(entryList, "Test failed: GetProgramEntryList with no parameters returned null");
            } else {
                entryList = testAPI.GetProgramEntryList(name); 
                Assert.IsNotNull(entryList, "Test failed: GetProgramEntryList with name parameter returned null");
            }

            Assert.GreaterOrEqual(entryList.Count, 1, "Test failed: GetProgramEntryList returned less than 1 entry.");
        }

        [Test]
        public void Test_GetProgramNameList() {
            List<string> names = testAPI.GetProgramNameList();
            Assert.IsNotNull(names, "Test failed: GetProgramNameList returned null");
            Assert.GreaterOrEqual(names.Count, 1, "Test failed: GetProgramNameList returned less than 1 entry.");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void Test_AddProgramEntryBadName(string name) {
            List<string> paths = new List<string>() { "xp/path/to/file1.txt" };
            ProgramEntry programEntry = new ProgramEntry(name, "1.0", "Windows XP 32-bit", paths, "Really useful test description", false);

            // Verify failure
            Assert.IsFalse(testAPI.AddProgramEntry(programEntry));
        }

        [TestCase(null)]
        [TestCase("    ")]
        [TestCase("Mac OSX 8-bit")]
        public void Test_AddProgramEntryBadOs(string os) {
            List<string> paths = new List<string>() { "xp/path/to/file1.txt" };
            ProgramEntry programEntry = new ProgramEntry("badtestprogram", "1.0", os, paths, "Really useful test description", false);

            // Verify failure
            Assert.IsFalse(testAPI.AddProgramEntry(programEntry));
        }

        [Test]
        public void Test_AddProgramEntryBadPaths() {
            List<string> paths1 = new List<string>() { null, "path2" };
            List<string> paths2 = new List<string>() { "path1", "" };
            List<string> paths3 = new List<string>() { "path1", "   "};

            // Verify failure
            ProgramEntry programEntry1 = new ProgramEntry("badtestprogram", "1.0", "Windows XP 32-bit", null, "Really useful test description", false);
            Assert.IsFalse(testAPI.AddProgramEntry(programEntry1));

            ProgramEntry programEntry2 = new ProgramEntry("badtestprogram", "1.0", "Windows XP 32-bit", paths1, "Really useful test description", false);
            Assert.IsFalse(testAPI.AddProgramEntry(programEntry2));

            ProgramEntry programEntry3 = new ProgramEntry("badtestprogram", "1.0", "Windows XP 32-bit", paths2, "Really useful test description", false);
            Assert.IsFalse(testAPI.AddProgramEntry(programEntry3));

            ProgramEntry programEntry4 = new ProgramEntry("badtestprogram", "1.0", "Windows XP 32-bit", paths3, "Really useful test description", false);
            Assert.IsFalse(testAPI.AddProgramEntry(programEntry4));
        }

        [Test]
        // This test depends on assuming GetProgramEntryList works properly
        public void Test_AddDuplicateProgramEntry() {
            // Get initial count
            int programEntryCount = testAPI.GetProgramEntryList().Count;

            // Build new object
            List<string> paths = new List<string>() { "xp/path/to/file1.txt" };
            ProgramEntry programEntry = new ProgramEntry("testprogram1", "1.0", "Windows XP 32-bit", paths, "Really useful test description", false);
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
            ProgramEntry programEntry = new ProgramEntry("apiTestEntry", "1.0", "Windows Vista 64-bit", paths, "Really useful test description", false);

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

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void Test_EditProgramEntryBadName(string name) {
            ProgramEntry programEntry = new ProgramEntry(name, null, null, null, null, false, 1);

            // Verify failure
            Assert.IsFalse(testAPI.EditProgramEntry(programEntry));
        }

        [TestCase(null)]
        [TestCase("    ")]
        [TestCase("Mac OSX 8-bit")]
        public void Test_EditProgramEntryBadOs(string os) {
            ProgramEntry programEntry = new ProgramEntry(null, null, os, null, null, false, 1);

            // Verify failure
            Assert.IsFalse(testAPI.AddProgramEntry(programEntry));
        }

        [Test]
        public void Test_EditProgramEntryBadPaths() {
            List<string> paths1 = new List<string>() { null, "path2" };
            List<string> paths2 = new List<string>() { "path1", "" };
            List<string> paths3 = new List<string>() { "path1", "   "};

            // Verify failure
            ProgramEntry programEntry1 = new ProgramEntry(null, null, null, paths1, "", false, 1);
            Assert.IsFalse(testAPI.AddProgramEntry(programEntry1));

            ProgramEntry programEntry2 = new ProgramEntry(null, null, null, paths2, "", false, 1);
            Assert.IsFalse(testAPI.AddProgramEntry(programEntry2));

            ProgramEntry programEntry3 = new ProgramEntry(null, null, null, paths3, "", false, 1);
            Assert.IsFalse(testAPI.AddProgramEntry(programEntry3));
        }

        [Test]
        public void Test_EditProgramEntry() {
            // Create object
            List<string> paths = new List<string>() { "vista/path/to/file9.txt" };
            ProgramEntry programEntry = new ProgramEntry("apiTestEntry", "1.1", "Windows Vista 64-bit", paths, "Really useful edited test description", false, 1);

            // Edit entry
            testAPI.EditProgramEntry(programEntry);

            // Find entry in list
            ProgramEntry editedEntry = testAPI.GetProgramEntry(1);

            // Check if edited object is equal to test object
            Assert.AreEqual(programEntry.ToString(), editedEntry.ToString());

            // Re-initialize so we don't lose the data
            this.Init();

            // TODO: Test EditProgramEntry with individual parameters too
        }

        [Test]
        public void Test_DeleteProgramEntry() {
            Assert.IsTrue(testAPI.DeleteProgramEntry(1));

            // Re-initialize so that we don't lose the data
            this.Init();
        }
    }
}
