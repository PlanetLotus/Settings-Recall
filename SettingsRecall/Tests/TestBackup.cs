using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;

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
            copyHandler1 = MockRepository.GenerateMock<CopyHandler>(@"C:\Unittest\", "unitTestLog.txt", false, stubbedFileSystem1);
            copyHandler1
                .Expect(t => t.Copy(null, null))
                .IgnoreArguments()
                .WhenCalled(action => copyCalls++)
                .Return(true);

            // Act
            BackupService.CreateBackup(selectedPrograms1, copyHandler1);

            // Assert
            Assert.AreEqual(programPaths1.Count + 1, copyCalls);    // + 1 for InitBackup
        }

        /*
        [Test]
        public void Test_BackupProgramDirectoriesAreCreated() {
            // Arrange
            int createProgramFolderCalls = 0;
            CopyHandler copyHandler2 = MockRepository.GenerateMock<CopyHandler>(@"C:\Unittest\", "unitTestLog.txt", false, stubbedFileSystem2);
            copyHandler2
                .Expect(t => t.CreateProgramFolder(null))
                .IgnoreArguments()
                .WhenCalled(action => createProgramFolderCalls++)
                .Return(true);

            // Act
            BackupService.CreateBackup(selectedPrograms1, copyHandler2);

            // Assert
            Assert.AreEqual(selectedPrograms1.Count, createProgramFolderCalls);
        }
        */

        /*
        [Test]
        public void Test_BackupWithPathsThatDoNotExist() {

        }
        */

        IFileSystem stubbedFileSystem1;
        IFileSystem stubbedFileSystem2;
        List<ProgramEntry> selectedPrograms1;
        CopyHandler copyHandler1;
        List<string> programPaths1;
    }
}