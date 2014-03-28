using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SettingsRecall.Tests {
    [TestFixture, RequiresSTA]
    class TestBackup {
        BackupPage page;
        string db_file = "../../unittest.db";

        [TestFixtureSetUp]
        public void Init() {
            // Create test directories and files
            Globals.sqlite_api = new SQLiteAPI(db_file);
            Globals.load_save_location = @"C:\Users\Matt\Documents\Visual Studio 2012\Projects\SettingsRecall\SettingsRecall\obj\Debug\BackupTests";
            string testDataDir = @"C:\Users\Matt\Documents\Visual Studio 2012\Projects\SettingsRecall\SettingsRecall\obj\Debug\TestData\";

            Cleanup();

            List<string> programPaths1 = new List<string>() { testDataDir + @"program1\a.txt", testDataDir + @"program1\b.txt", testDataDir + @"program1\c.txt" };
            List<string> programPaths2 = new List<string>() { testDataDir + @"program2\d.txt" };

            ProgramEntry program1 = new ProgramEntry("backupTestProgram1", programPaths1);
            ProgramEntry program2 = new ProgramEntry("backupTestProgram2", programPaths2);

            Globals.sqlite_api.AddProgram(program1);
            Globals.sqlite_api.AddProgram(program2);

            page = new BackupPage();
            page.supportedPrograms.Clear();
            page.supportedPrograms.Add(program1);
            page.supportedPrograms.Add(program2);
            page.backupPageRightList.Items.Add("backupTestProgram1");
            page.backupPageRightList.Items.Add("backupTestProgram2");
        }

        [TestFixtureTearDown]
        public void Cleanup() {
            // Delete test data
            Globals.sqlite_api.ClearDB();
            Globals.load_save_location = @"C:\Users\Matt\Documents\Visual Studio 2012\Projects\SettingsRecall\SettingsRecall\obj\Debug\BackupTests";
            if (Directory.Exists(Globals.load_save_location)) {
                DirectoryInfo di = new DirectoryInfo(Globals.load_save_location);
                di.Delete(true);
            }
        }

        [Test]
        public void Test_Backup() {
            page.createBackupButton_Click(null, null);

            DirectoryInfo di = new DirectoryInfo(Globals.load_save_location);
            Assert.AreEqual(2, di.GetDirectories().Length);
            Assert.AreEqual(6, di.GetFiles("*.*", SearchOption.AllDirectories).Length);
        }
    }
}