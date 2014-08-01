using Newtonsoft.Json;
using SettingsRecall.Utility;
using System.Collections.Generic;
using System.Linq;

namespace SettingsRecall {
    public static class BackupService {
        public static void CreateBackup(IEnumerable<ProgramEntry> selectedPrograms, CopyHandler copyHandler) {
            copyHandler.InitBackup(logFileName);
            List<BackupDataModel> backupData = new List<BackupDataModel>();

            // Loop through selectedPrograms, copying files to save location
            foreach (ProgramEntry program in selectedPrograms) {
                // Create folder for program in save location
                copyHandler.CreateProgramFolder(program.Name);
                BackupDataModel backupModel = new BackupDataModel { ProgramName = program.Name, SourceToDestPaths = new Dictionary<string, string>() };

                foreach (string path in program.Paths) {
                    // Copy files at path to programDir
                    // It's okay (and expected) for not all paths to exist
                    string filename = path.Split('\\').Last();
                    string actualDest = copyHandler.Copy(path, program.Name + "\\" + filename);
                    backupModel.SourceToDestPaths.Add(path, actualDest);
                }

                backupData.Add(backupModel);
            }

            string jsonData = JsonConvert.SerializeObject(backupData);
            copyHandler.CloseBackup(jsonData, backupDataFileName);
        }

        private sealed class BackupDataModel {
            public string ProgramName { get; set; }
            public Dictionary<string, string> SourceToDestPaths { get; set; }
        }

        private const string logFileName = "backupLog.txt";
        private const string backupDataFileName = "backupData.json";
    }
}
