using Moq;
using NUnit.Framework;
using SettingsRecall.Utility;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace SettingsRecall.Tests {
    [TestFixture]
    class TestBackup {
        [TestFixtureSetUp]
        public void SetUp() {
            programPaths1 = new List<string>() { @"program1\a.txt", @"program1\b.txt" };
            stubbedFileSystem1 = new MockFileSystem(new Dictionary<string, MockFileData> {
                { programPaths1[0], new MockFileData("") },
                { programPaths1[1], new MockFileData("") }
            });
            stubbedFileSystem2 = new MockFileSystem(new Dictionary<string, MockFileData> {
                { programPaths1[0], new MockFileData("") },
                { programPaths1[1], new MockFileData("") }
            });

            ProgramEntry program1 = new ProgramEntry("backupTestProgram1", false, programPaths1);
            selectedPrograms1 = new List<ProgramEntry> { program1 };
        }

        [Test]
        public void Test_BackupCopiesFiles() {
            // Arrange
            int copyCalls = 0;
            Mock<CopyHandler> mockCopyHandler = new Mock<CopyHandler>(@"C:\Unittest\", "unitTestLog.txt", false, stubbedFileSystem1);
            mockCopyHandler
                .Setup(a => a.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<OverwriteEnum>()))
                .Callback(() => copyCalls++);
            CopyHandler copyHandler = mockCopyHandler.Object;

            // Act
            BackupService.CreateBackup(selectedPrograms1, copyHandler);

            // Assert
            Assert.AreEqual(programPaths1.Count + 1, copyCalls);    // + 1 for InitBackup
        }

        [Test]
        public void Test_BackupProgramDirectoriesAreCreated() {
            // Arrange
            int createProgramFolderCalls = 0;
            Mock<CopyHandler> mockCopyHandler = new Mock<CopyHandler>(@"C:\Unittest\", "unitTestLog.txt", false, stubbedFileSystem2);
            mockCopyHandler
                .Setup(a => a.CreateProgramFolder(It.IsAny<string>()))
                .Callback(() => createProgramFolderCalls++);
            CopyHandler copyHandler = mockCopyHandler.Object;

            // Act
            BackupService.CreateBackup(selectedPrograms1, copyHandler);

            // Assert
            Assert.AreEqual(selectedPrograms1.Count, createProgramFolderCalls);
        }

        IFileSystem stubbedFileSystem1;
        IFileSystem stubbedFileSystem2;
        List<ProgramEntry> selectedPrograms1;
        List<string> programPaths1;
    }
}