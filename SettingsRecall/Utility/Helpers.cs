using System.Linq;

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
    }
}
