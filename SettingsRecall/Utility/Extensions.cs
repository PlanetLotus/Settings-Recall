using System.Collections.Generic;

namespace SettingsRecall.Utility {
    public static class Extensions {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source) {
            return new HashSet<T>(source);
        }
    }
}
