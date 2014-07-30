using Newtonsoft.Json;
using SettingsRecall.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Windows;

namespace SettingsRecall {
    public class CopyHandler {
        public CopyHandler(string backupDir, bool isDryRun = false, IFileSystem fileSystem = null) {
            if (fileSystem == null)
                fs = new FileSystem();  // Use System.IO
            else
                fs = fileSystem;

            this.isDryRun = isDryRun;
            this.backupDir = backupDir;
        }

        public bool InitBackup(string logFileName) {
            backupData = new List<BackupDataModel>();

            fs.Directory.CreateDirectory(backupDir);

            // If backupDir already exists, make sure it's empty
            // This prevents duplicates (due to renaming) of the same program's files
            if (fs.Directory.GetFileSystemEntries(backupDir).Any()) {
                DirectoryInfoBase dir = fs.DirectoryInfo.FromDirectoryName(backupDir);
                dir.Empty();
            }

            if (isDryRun) {
                log = new StreamWriter(Console.OpenStandardOutput());
            } else {
                FileInfoBase fib = fs.FileInfo.FromFileName(backupDir + logFileName);
                log = new StreamWriter(fib.Create());
            }

            log.WriteLine("Backup started at " + DateTime.Now);
            return true;
        }

        public bool InitRestore(string logFileName) {
            if (isDryRun) {
                log = new StreamWriter(Console.OpenStandardOutput());
            } else {
                FileInfoBase fib = fs.FileInfo.FromFileName(backupDir + logFileName);
                log = new StreamWriter(fib.Create());
            }

            log.WriteLine("Restore started at " + DateTime.Now);
            return true;
        }

        public virtual bool CreateProgramFolder(string programName) {
            fs.Directory.CreateDirectory(backupDir + programName);
            backupData.Add(
                new BackupDataModel { ProgramName = programName, SourceToDestPaths = new Dictionary<string, string>() });

            return true;
        }

        public virtual string Copy(string source, string relativeDest, OverwriteEnum overwriteSetting = OverwriteEnum.Rename) {
            if (!fs.File.Exists(source)) {
                log.WriteLine("Source does not exist at " + source);
                return null;
            }

            string dest = backupDir + relativeDest;

            if (fs.File.Exists(dest)) {
                if (overwriteSetting == OverwriteEnum.Rename) {
                    dest = GetUniqueFileDestination(dest);
                } else if (overwriteSetting == OverwriteEnum.Ask) {
                    MessageBoxResult result = MessageBox.Show(
                        string.Format("{0} already exists. Overwrite? (\"No\" will rename file)", dest),
                        "Overwrite file?",
                        MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.No)
                        dest = GetUniqueFileDestination(dest);
                }
            }

            if (!isDryRun) {
                try {
                    fs.File.Copy(source, dest);
                } catch {
                    // TODO: Catch specific exceptions and handle accordingly
                    log.WriteLine("Could not copy file " + source + " to " + dest);
                    return null;
                }
            }

            log.WriteLine("Copied " + source + " to " + dest);

            return dest;
        }

        public virtual void AddJsonPath(string programName, string source, string actualDestination) {
            backupData
                .Single(entry => entry.ProgramName == programName)
                .SourceToDestPaths.Add(source, actualDestination);
        }

        public bool CloseBackup() {
            log.WriteLine("Backup finished at " + DateTime.Now);
            log.Close();

            string jsonData = JsonConvert.SerializeObject(backupData);
            fs.File.WriteAllText(backupDir + "backupData.json", jsonData);

            return true;
        }

        public bool CloseRestore() {
            log.WriteLine("Restore finished at " + DateTime.Now);
            log.Close();
            return true;
        }

        private string GetUniqueFileDestination(string originalDest) {
            // Loop through renaming process until dest doesn't exist
            string dest = originalDest + "-1";
            int fileIncrementer = 1;

            while (fs.File.Exists(dest)) {
                dest = dest.Substring(0, dest.Length - 1) + fileIncrementer;
                fileIncrementer++;
            }

            return dest;
        }

        private sealed class BackupDataModel {
            public string ProgramName { get; set; }
            public Dictionary<string, string> SourceToDestPaths { get; set; }
        }

        private string backupDir;
        private IFileSystem fs;
        private StreamWriter log;
        private List<BackupDataModel> backupData;
        private bool isDryRun;
    }
}
