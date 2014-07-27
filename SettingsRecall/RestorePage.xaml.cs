using SettingsRecall.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SettingsRecall {
    public partial class RestorePage : StackPanel {
        public RestorePage() {
            InitializeComponent();
            errorMessage = new ErrorMessage(this);
            restoreDir = null;
            restorablePrograms = new ObservableCollection<string>();
            addedPrograms = new ObservableCollection<string>();

            restorePageLeftList.ItemsSource = restorablePrograms;
            restorePageRightList.ItemsSource = addedPrograms;
            restoreButton.IsEnabled = false;
        }

        private void chooseFolderButton_Click(object sender, RoutedEventArgs e) {
            string restoreDir = Helpers.GetFolderPathFromDialog();

            if (restoreDir == null) return;

            // Find database file
            string[] dbFileMatches = Directory.GetFiles(restoreDir, "*.db");

            errorMessage.ClearAllErrors();

            if (dbFileMatches.Length == 0) {
                errorMessage.AddErrorLabel("Restore info not found.");
                return;
            } else if (dbFileMatches.Length > 1) {
                errorMessage.AddErrorLabel("Multiple possible databases found.");
                return;
            }

            Globals.dbLocation = dbFileMatches.Single();

            allDbPrograms = SQLiteAPI.GetProgramList();
            HashSet<string> allDbProgramNames = allDbPrograms.Select(p => p.Name).ToHashSet();

            // Display directory in label
            folderLabel.Content = restoreDir;

            // Generate list of restorable programs, based on db, filtered by directories backed up
            // TODO: Write unit test for directory backed up that's not in db
            restorablePrograms.Clear();
            string[] dirs = Directory.GetDirectories(restoreDir);

            foreach (string dir in dirs) {
                string programName = Helpers.TrimFilename(dir);

                if (allDbProgramNames.Contains(programName))
                    restorablePrograms.Add(programName);
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e) {
            // Make sure something is selected
            if (restorePageLeftList.SelectedIndex == -1)
                return;

            // Remove from left list, add to right list
            string selected = restorePageLeftList.SelectedItem.ToString();
            addedPrograms.Add(selected);
            restorablePrograms.Remove(selected);

            // Enable the 'Restore' button if necessary
            if (addedPrograms.Count > 0 && restoreDir != null)
                restoreButton.IsEnabled = true;
        }

        private void removeButton_Click(object sender, RoutedEventArgs e) {
            // Make sure something is selected
            if (restorePageRightList.SelectedIndex == -1)
                return;

            // Remove from right list, add to left list
            string selected = restorePageRightList.SelectedItem.ToString();
            restorablePrograms.Add(selected);
            addedPrograms.Remove(selected);

            // Disable the 'Restore button if necessary
            if (addedPrograms.Count == 0 || restoreDir == null) {
                restoreButton.IsEnabled = false;
            }
        }

        private void restoreButton_Click(object sender, RoutedEventArgs e) {
            IEnumerable<ProgramEntry> selectedPrograms = allDbPrograms.Where(p => addedPrograms.Contains(p.Name));

            foreach (ProgramEntry program in selectedPrograms) {
                string backupProgramDir = restoreDir + program.Name;

                if (!Directory.Exists(backupProgramDir)) {
                    Console.WriteLine(backupProgramDir + " does not exist in the backup folder.");
                    continue;
                }

                // Copy each file in backup folder
                foreach (string filePath in Directory.GetFiles(backupProgramDir)) {
                    // Copy file to each path in ProgramEntry with a matching filename that exists on this machine
                    // Overwrites files, but does not create directories
                    string fileName = Helpers.TrimFilename(filePath);

                    foreach (string matchedPath in program.Paths.Where(path => path.EndsWith(fileName))) {
                        if (!Directory.Exists(Helpers.GetParentFromFile(matchedPath)))
                            continue;

                        Console.WriteLine("Copying " + fileName + " to " + matchedPath);

                        if (File.Exists(matchedPath))
                            Console.WriteLine("Overwriting " + matchedPath);
                    }
                }
            }
        }

        private ObservableCollection<string> restorablePrograms;
        private ObservableCollection<string> addedPrograms;
        private IEnumerable<ProgramEntry> allDbPrograms;
        private ErrorMessage errorMessage;
        private string restoreDir;
    }
}
