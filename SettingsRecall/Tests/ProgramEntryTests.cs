using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;

namespace SettingsRecall {
    [TestFixture]
    public class ProgramEntryTests {
        [TestCase("")]
        [TestCase("    ")]
        public void Test_BadName(string name) {
            List<string> paths = new List<string>() { "xp/path/to/file1.txt" };

            // Verify failure
            Assert.Throws(typeof(ArgumentException), () => new ProgramEntry(name, false, paths, "Really useful test description"));
        }

        [Test]
        public void Test_BadPaths() {
            List<string> paths1 = new List<string>() { null, "path2" };
            List<string> paths2 = new List<string>() { "path1", "" };
            List<string> paths3 = new List<string>() { "path1", "   " };

            // Verify failure
            Assert.Throws(typeof(ArgumentException), () => new ProgramEntry("badtestprogram", false, paths1, "Really useful test description"));
            Assert.Throws(typeof(ArgumentException), () => new ProgramEntry("badtestprogram", false, paths2, "Really useful test description"));
            Assert.Throws(typeof(ArgumentException), () => new ProgramEntry("badtestprogram", false, paths3, "Really useful test description"));
        }
    }
}
