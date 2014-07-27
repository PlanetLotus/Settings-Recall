using System.Collections.Generic;
using System.IO;

namespace SettingsRecall.Utility {
    public static class Extensions {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source) {
            return new HashSet<T>(source);
        }

        public static void Empty(this DirectoryInfo directory) {
            foreach (FileInfo file in directory.GetFiles()) file.Delete();
            foreach (DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
        }
    }
}
