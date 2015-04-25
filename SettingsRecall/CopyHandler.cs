using SettingsRecall.Utility;
using System;
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
            return true;
        }

        public virtual string Copy(string source, string dest, OverwriteEnum overwriteSetting = OverwriteEnum.Rename) {
            if (fs.File.Exists(source))
                return CopyFile(source, dest, overwriteSetting);
            else if (fs.Directory.Exists(source))
                return CopyFolder(source, dest, overwriteSetting);

            log.WriteLine("Source does not exist at " + source);
            return null;
        }

        private string CopyFile(string source, string dest, OverwriteEnum overwriteSetting = OverwriteEnum.Rename) {
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
                    fs.File.Copy(source, dest, overwriteSetting == OverwriteEnum.Overwrite);
                } catch (Exception ex) {
                    // TODO: Catch specific exceptions and handle accordingly
                    log.WriteLine("Could not copy file {0} to {1}: {2}", source, dest, ex.Message);
                    return null;
                }
            }

            log.WriteLine("Copied " + source + " to " + dest);

            return dest;
        }

        private string CopyFolder(string source, string dest, OverwriteEnum overwriteSetting = OverwriteEnum.Rename) {
            if (fs.Directory.Exists(dest)) {
                if (overwriteSetting == OverwriteEnum.Rename) {
                    dest = GetUniqueFolderDestination(dest);
                } else if (overwriteSetting == OverwriteEnum.Ask) {
                    MessageBoxResult result = MessageBox.Show(
                        string.Format("{0} already exists. Overwrite? (\"No\" will rename file)", dest),
                        "Overwrite file?",
                        MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.No)
                        dest = GetUniqueFolderDestination(dest);
                }
            }

            if (!isDryRun) {
                try {
                    DoCopyFolder(source, dest, overwriteSetting);
                } catch (Exception ex) {
                    // TODO: Catch specific exceptions and handle accordingly
                    log.WriteLine("Could not copy directory {0} to {1}: {2}", source, dest, ex.Message);
                    return null;
                }
            }

            log.WriteLine("Copied " + source + " to " + dest);
            return dest;
        }

        private void DoCopyFolder(string source, string dest, OverwriteEnum overwriteSetting = OverwriteEnum.Rename) {
            // Create all of the directories
            foreach (string dirPath in fs.Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
                fs.Directory.CreateDirectory(dirPath.Replace(source, dest));

            // Copy all files
            foreach (string newPath in fs.Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
                fs.File.Copy(newPath, newPath.Replace(source, dest), overwriteSetting == OverwriteEnum.Overwrite);
        }

        public bool CloseBackup(string jsonData, string backupDataFileName) {
            log.WriteLine("Backup finished at " + DateTime.Now);
            log.Close();

            fs.File.WriteAllText(backupDir + backupDataFileName, jsonData);

            return true;
        }

        public bool CloseRestore() {
            log.WriteLine("Restore finished at " + DateTime.Now);
            log.Close();
            return true;
        }

        public string BackupDir {
            get { return backupDir; }
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

        private string GetUniqueFolderDestination(string originalDest) {
            // Loop through renaming process until dest doesn't exist
            string dest = originalDest + "-1";
            int incrementer = 1;

            while (fs.Directory.Exists(dest)) {
                dest = dest.Substring(0, dest.Length - 1) + incrementer;
                incrementer++;
            }

            return dest;
        }

        private string backupDir;
        private IFileSystem fs;
        private StreamWriter log;
        private bool isDryRun;
    }
}
