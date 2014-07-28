using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;

namespace SettingsRecall.Utility {
    public static class Extensions {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source) {
            return new HashSet<T>(source);
        }

        public static void Empty(this DirectoryInfoBase directory) {
            foreach (FileInfoBase file in directory.GetFiles()) file.Delete();
            foreach (DirectoryInfoBase subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
        }
    }
}
