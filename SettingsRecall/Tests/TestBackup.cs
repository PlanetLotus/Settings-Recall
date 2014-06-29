using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;

namespace SettingsRecall.Tests {
    [TestFixture, RequiresSTA]
    class TestBackup {
        [TestFixtureSetUp]
        public void Init() {
            /*
            // Create test directories and files
            Globals.load_save_location = @"C:\Users\Matt\Documents\Visual Studio 2012\Projects\SettingsRecall\SettingsRecall\obj\Debug\BackupTests";
            string testDataDir = @"C:\Users\Matt\Documents\Visual Studio 2012\Projects\SettingsRecall\SettingsRecall\obj\Debug\TestData\";

            Cleanup();

            List<string> programPaths1 = new List<string>() { testDataDir + @"program1\a.txt", testDataDir + @"program1\b.txt", testDataDir + @"program1\c.txt" };
            List<string> programPaths2 = new List<string>() { testDataDir + @"program2\d.txt" };

            ProgramEntry program1 = new ProgramEntry("backupTestProgram1", false, programPaths1);
            ProgramEntry program2 = new ProgramEntry("backupTestProgram2", false, programPaths2);

            SQLiteAPI.AddProgram(program1);
            SQLiteAPI.AddProgram(program2);

            page = new BackupPage();
            page.supportedPrograms.Clear();
            page.supportedPrograms.Add(program1);
            page.supportedPrograms.Add(program2);
            page.backupPageRightList.Items.Add("backupTestProgram1");
            page.backupPageRightList.Items.Add("backupTestProgram2");
            */


            //page = MockRepository.GenerateStub<BackupPage>(stubbedFileSystem);
            //page.supportedPrograms.Add(program1);
        }

        [Test]
        public void Test_Backup() {
            // Arrange
            List<string> programPaths1 = new List<string>() { @"program1\a.txt", @"program1\b.txt" };
            IFileSystem stubbedFileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { programPaths1[0], new MockFileData("") },
                { programPaths1[1], new MockFileData("") }
            });
            CopyHandler copyHandler = new CopyHandler(@"C:\Unittest\", "unitTestLog.txt", false, stubbedFileSystem);

            ProgramEntry program1 = new ProgramEntry("backupTestProgram1", false, programPaths1);
            List<ProgramEntry> selectedPrograms = new List<ProgramEntry> { program1 };

            // Act
            BackupService.CreateBackup(selectedPrograms, copyHandler);

            // Assert
            //DirectoryInfo di = new DirectoryInfo(Globals.load_save_location);
            //Assert.AreEqual(2, di.GetDirectories().Length);
            //Assert.AreEqual(6, di.GetFiles("*.*", SearchOption.AllDirectories).Length);
        }
    }
}