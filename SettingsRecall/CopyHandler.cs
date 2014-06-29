using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;

namespace SettingsRecall {
    public class CopyHandler {
        string backupDir;
        IFileSystem fs;
        StreamWriter log;
        bool isDryRun;

        public CopyHandler(string backupDir, string logFileName, bool isDryRun = false, IFileSystem fileSystem = null) {
            if (fileSystem == null)
                fs = new FileSystem();  // Use System.IO
            else
                fs = fileSystem;

            fs.Directory.CreateDirectory(backupDir);

            this.isDryRun = isDryRun;
            this.backupDir = backupDir;

            if (isDryRun) {
                FileInfoBase fib = fs.FileInfo.FromFileName(backupDir + logFileName);
                log = new StreamWriter(fib.Create());
            } else {
                log = new StreamWriter(Console.OpenStandardOutput());
            }
        }

        public bool InitBackup() {
            log.WriteLine("DO NOT MODIFY THIS FILE. IT MAY BE USED TO RESTORE YOUR SETTINGS TO THE CORRECT LOCATION.");
            log.WriteLine("");
            log.WriteLine("Backup started at " + DateTime.Now);

            // Backup database file
            string dbFileName = Globals.dbLocation.Split(new char[] { '\\', '/' }).Last();
            return Copy(Globals.dbLocation, backupDir + dbFileName);
        }

        public bool InitRestore() {
            log.WriteLine("Restore started at " + DateTime.Now);
            return true;
        }

        public virtual bool CreateProgramFolder(string programName) {
            fs.Directory.CreateDirectory(backupDir + programName);
            return true;
        }

        public virtual bool Copy(string source, string relativeDest, bool overwrite = false) {
            if (!fs.File.Exists(source)) {
                log.WriteLine("Source does not exist at " + source);
                return false;
            }

            string dest = backupDir + relativeDest;

            // Loop through renaming process until dest doesn't exist
            if (!overwrite && fs.File.Exists(dest)) {
                dest += "-1";
                int fileIncrementer = 1;

                while (fs.File.Exists(dest)) {
                    dest = dest.Substring(0, dest.Length-1) + fileIncrementer;
                    fileIncrementer++;
                }
            }

            if (!isDryRun) {
                try {
                    fs.File.Copy(source, dest, overwrite);
                } catch {
                    // TODO: Catch specific exceptions and handle accordingly
                    log.WriteLine("Could not copy file " + source + " to " + dest);
                    return false;
                }
            }

            log.WriteLine("Copied " + source + " to " + dest);

            return true;
        }

        public bool CloseBackup() {
            log.WriteLine("Backup finished at " + DateTime.Now);
            log.Close();
            return true;
        }

        public bool CloseRestore() {
            log.WriteLine("Restore finished at " + DateTime.Now);
            log.Close();
            return true;
        }
    }
}
