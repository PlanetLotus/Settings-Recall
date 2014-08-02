using SettingsRecall.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using BackupDataModel = SettingsRecall.BackupService.BackupDataModel;

namespace SettingsRecall {
    public class RestoreService {
        /// <summary>
        /// Overwrites files, but does not create directories.
        /// </summary>
        public static void RestoreBackup(IEnumerable<BackupDataModel> selectedPrograms, CopyHandler copyHandler) {
            copyHandler.InitRestore(restoreLogFile);

            foreach (BackupDataModel program in selectedPrograms) {
                // This assumes that no backup paths were deleted. It's completely relying on the JSON data (this is preferred).
                foreach (string restoreDest in program.SourceToDestPaths.Keys) {
                    string backupPathLocation = program.SourceToDestPaths[restoreDest];
                    copyHandler.Copy(backupPathLocation, restoreDest, OverwriteEnum.Overwrite);
                }
            }

            copyHandler.CloseRestore();
        }

        private const string restoreLogFile = "restoreLog.txt";
    }
}
