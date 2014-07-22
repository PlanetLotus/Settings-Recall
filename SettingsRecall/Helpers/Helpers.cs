using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace SettingsRecall {
    class Helpers {
        public static string GetOSFriendlyName() {
            string result = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
            foreach (ManagementObject os in searcher.Get()) {
                result = os["Caption"].ToString();
                break;
            }

            // Convert result to either XP, Vista, 7, or 8 for our standards
            if (result.Contains("XP"))
                result = "Windows XP";
            else if (result.Contains("Vista"))
                result = "Windows Vista";
            else if (result.Contains("Windows 7"))
                result = "Windows 7";
            else if (result.Contains("Windows 8"))
                result = "Windows 8";
            else {
                Console.WriteLine("Unexpected OS name: " + result);
                return null;
            }

            // Append 32-bit or 64-bit
            if (Environment.Is64BitOperatingSystem == true)
                result += " 64-bit";
            else
                result += " 32-bit";

            return result;
        }

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
