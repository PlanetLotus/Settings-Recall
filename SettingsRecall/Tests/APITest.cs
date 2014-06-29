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
        /*
        [TestFixtureSetUp]
        public void Init() {
            paths = new List<string> { "testpath" };
            jsonPaths = JsonConvert.SerializeObject(paths);
            dt.Columns.Add("Name");
            dt.Columns.Add("IsPermanent");
            dt.Columns.Add("Paths");
            dt.Columns.Add("Description");
            dt.Rows.Add("testprogram1", false, jsonPaths, "");

            stubbedDb = MockRepository.GenerateStub<SQLiteDatabase>();
            Globals.db = stubbedDb;
        }

        [TestCase("a")]
        [TestCase("asdfasdfasdf")]
        public void Test_GetNonExistentProgramEntry(string name) {
            stubbedDb
                .Stub(x => x.GetDataTable(string.Format(getProgramSelect, name)))
                .Return(new DataTable());

            Assert.IsNull(SQLiteAPI.GetProgram(name));
        }

        [TestCase("testprogram1")]
        public void Test_GetProgramEntry(string name) {

            stubbedDb
                .Stub(x => x.GetDataTable(string.Format(getProgramSelect, name)))
                .Return(dt);

            Assert.IsNotNull(SQLiteAPI.GetProgram(name));
        }

        [Test]
        public void Test_GetProgramList() {
            string getProgramListSelect = "SELECT Name,IsPermanent,Paths,Description FROM Program ORDER BY Name;";
            stubbedDb
                .Stub(x => x.GetDataTable(getProgramListSelect))
                .Return(dt);

            List<ProgramEntry> entryList = SQLiteAPI.GetProgramList(); 

            Assert.IsNotNull(entryList, "Test failed: GetProgramEntryList with no parameters returned null");
            Assert.GreaterOrEqual(entryList.Count, 1, "Test failed: GetProgramEntryList returned less than 1 entry.");
        }

        [Test]
        public void Test_GetProgramNameList() {
            string getProgramNameListSelect = "SELECT Name FROM Program ORDER BY Name;";
            stubbedDb
                .Stub(x => x.GetDataTable(getProgramNameListSelect))
                .Return(dt);

            List<string> names = SQLiteAPI.GetProgramNameList();

            Assert.IsNotNull(names, "Test failed: GetProgramNameList returned null");
            Assert.GreaterOrEqual(names.Count, 1, "Test failed: GetProgramNameList returned less than 1 entry.");
        }

        [Test]
        public void Test_AddProgramBadPaths() {
            // This doesn't require a stub because it fails before it touches the db
            ProgramEntry programEntry1 = new ProgramEntry("badtestprogram", false, new List<string>(), "Really useful test description");
            Assert.IsFalse(SQLiteAPI.AddProgram(programEntry1));
        }

        [Test]
        public void Test_AddProgram() {
            List<string> paths = new List<string>() { "vista/path/to/file4.txt" };
            ProgramEntry programEntry = new ProgramEntry("apiTestEntry", false, paths, "Really useful test description");

            Dictionary<string, string> insert = new Dictionary<string, string>();
            insert.Add("Name", programEntry.Name);
            insert.Add("IsPermanent", "0");
            insert.Add("Paths", JsonConvert.SerializeObject(programEntry.Paths));
            insert.Add("Description", programEntry.Description);

            stubbedDb
                .Stub(x => x.Insert("Program", insert))
                .Return(true);

            Assert.IsTrue(SQLiteAPI.AddProgram(programEntry));
        }

        [Test]
        public void Test_EditProgram() {
            List<string> paths = new List<string>() { "vista/path/to/file9.txt" };
            ProgramEntry programEntry = new ProgramEntry("testprogram1", false, paths, "Really useful edited test description");

            Dictionary<string, string> update = new Dictionary<string, string>();
            update.Add("Name", programEntry.Name);
            update.Add("IsPermanent", "0");
            update.Add("Paths", JsonConvert.SerializeObject(programEntry.Paths));
            update.Add("Description", programEntry.Description);

            stubbedDb
                .Stub(x => x.Update("Program", update, string.Format("Name = {0}", programEntry.Name)))
                .Return(true);

            Assert.IsTrue(SQLiteAPI.EditProgram(programEntry));
        }

        [Test]
        public void Test_DeleteProgram() {
            string programName = "testprogram1";
            stubbedDb
                .Stub(x => x.Delete("Program", string.Format("Name = {0}", programName)))
                .Return(true);

            Assert.IsTrue(SQLiteAPI.DeleteProgram(programName));
        }

        private SQLiteDatabase stubbedDb;
        private const string getProgramSelect = "SELECT Name,IsPermanent,Paths,Description FROM Program WHERE Name = '{0}';";

        private List<string> paths;
        private string jsonPaths;
        private DataTable dt = new DataTable();
        */
    }
}
