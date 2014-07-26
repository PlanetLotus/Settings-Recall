using System.Linq;
using System.Windows.Forms;

namespace SettingsRecall {
    class Helpers {
        /// <summary>
        /// Trim everything but the filename from a full path
        /// </summary>
        /// <param name="path">A full path</param>
        /// <returns>Filename</returns>
        public static string TrimFilename(string path) {
            return path.Trim().Split('\\').Last();
        }

        public static string GetParentFromFile(string path) {
            string[] pathSections = path.Trim().Split('\\');
            string parentDirPath = "";

            for (int i = 0; i < pathSections.Length - 1; i++) {
                parentDirPath += pathSections[i] + @"\";
            }

            return parentDirPath;
        }

        public static string GetFolderPathFromDialog() {
            FolderBrowserDialog open_dialog = new FolderBrowserDialog { ShowNewFolderButton = true };
            DialogResult result = open_dialog.ShowDialog();

            string folderPath = null;

            if (result == System.Windows.Forms.DialogResult.OK)
                folderPath = open_dialog.SelectedPath;
            else
                return folderPath;

            // Make sure save directory has a trailing slash (so we can append to it)
            if (!folderPath.Last().Equals('\\'))
                folderPath = folderPath.Trim() + "\\";

            return folderPath;
        }
    }
}
