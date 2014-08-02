using System;
using System.Collections.Generic;
using System.Linq;
using BackupDataModel = SettingsRecall.BackupService.BackupDataModel;

namespace SettingsRecall {
    public class RestoreService {
        public static void RestoreBackup(IEnumerable<BackupDataModel> selectedPrograms, CopyHandler copyHandler) {
            /*
             * This can either rely on what's in the .db file or what's in the log file. The .db file is easier to
             * read from but the log file is necessary to rely on in the case of multiple versions...it needs to know
             * which files to grab because there will be renamed duplicates. Currently, this is only set up to read the .db
             * file.
             * 
             * Reading from the log file is difficult because we want to support choosing which (not necessarily all)
             * programs you want to restore, and this requires getting the program names via the log, which requires looking at the
             * direct parent of the file backed up. Messy. Alternatively, the backup log could store more information...
             * It would be even easier to maintain another log that was purely for the machine. It could be a JSON file that would
             * have a list of program names backed up and then a list of source paths to backup paths.
             * 
             * It would make sense to only support reading from the log file as the other would be redundant.
             */

            /*
            foreach (ProgramEntry program in selectedPrograms) {
                string backupProgramDir = restoreDir + program.Name;

                // Copy each file in backup folder
                foreach (string filePath in Directory.GetFiles(backupProgramDir)) {
                    // Copy file to each path in ProgramEntry with a matching filename that exists on this machine
                    // Overwrites files, but does not create directories
                    string fileName = Helpers.TrimFilename(filePath);

                    foreach (string matchedPath in program.Paths.Where(path => path.EndsWith(fileName))) {
                        if (!Directory.Exists(Helpers.GetParentFromFile(matchedPath)))
                            continue;

                        Console.WriteLine("Copying " + fileName + " to " + matchedPath);

                        if (File.Exists(matchedPath))
                            Console.WriteLine("Overwriting " + matchedPath);
                    }
                }
            }
            */
        }
    }
}
