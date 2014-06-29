using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SettingsRecall {
    public static class BackupService {
        public static void CreateBackup(IEnumerable<ProgramEntry> selectedPrograms, CopyHandler copyHandler) {
            copyHandler.InitBackup();

            // Loop through selectedPrograms, copying files to save location
            foreach (ProgramEntry program in selectedPrograms) {
                // Create folder for program in save location
                copyHandler.CreateProgramFolder(program.Name);

                // Check edge case: Multiple files of same name
                // Implement later...
                // Get number of versions in each program, x
                // Get strings that have multiple versions, strList
                // Create x subdirs in program dir
                // When copying, if filename in strList, copy to a subdir that doesn't contain filename

                foreach (string path in program.Paths) {
                    // Copy files at path to programDir
                    // It's okay (and expected) for not all paths to exist
                    string filename = path.Split('\\').Last();
                    copyHandler.Copy(path, program.Name + "\\" + filename);
                }
            }

            copyHandler.CloseBackup();
            
        }
    }
}
