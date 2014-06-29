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
        [Test]
        public void Test_Backup() {
            // Arrange
            List<string> programPaths1 = new List<string>() { @"program1\a.txt", @"program1\b.txt" };
            IFileSystem stubbedFileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { programPaths1[0], new MockFileData("") },
                { programPaths1[1], new MockFileData("") }
            });

            CopyHandler copyHandler = MockRepository.GenerateMock<CopyHandler>(@"C:\Unittest\", "unitTestLog.txt", false, stubbedFileSystem);
            int copyCalls = 0;
            copyHandler.Expect(t => t.Copy(null, null)).IgnoreArguments().WhenCalled(a => copyCalls++).Return(true);

            ProgramEntry program1 = new ProgramEntry("backupTestProgram1", false, programPaths1);
            List<ProgramEntry> selectedPrograms = new List<ProgramEntry> { program1 };

            // Act
            BackupService.CreateBackup(selectedPrograms, copyHandler);

            // Assert
            Assert.AreEqual(programPaths1.Count + 1, copyCalls);    // + 1 for InitBackup
        }
    }
}