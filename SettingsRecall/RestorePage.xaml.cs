﻿using SettingsRecall.Utility;
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
            alertMessage = new AlertMessage(this);
            restoreDir = null;
            addedPrograms = new ObservableCollection<string>();

            restorePageLeftList.ItemsSource = restorablePrograms;
            restorePageRightList.ItemsSource = addedPrograms;
            restoreButton.IsEnabled = false;
        }

        private void chooseFolderButton_Click(object sender, RoutedEventArgs e) {
            restoreDir = Helpers.GetFolderPathFromDialog();

            if (restoreDir == null) return;

            // Find database file
            string[] dbFileMatches = Directory.GetFiles(restoreDir, "*.db");

            alertMessage.ClearAllAlerts();

            if (dbFileMatches.Length == 0) {
                alertMessage.AddAlertLabel("Restore info not found.");
                return;
            } else if (dbFileMatches.Length > 1) {
                alertMessage.AddAlertLabel("Multiple possible databases found.");
                return;
            }

            Globals.dbLocation = dbFileMatches.Single();

            allDbPrograms = SQLiteAPI.GetProgramList();
            HashSet<string> allDbProgramNames = allDbPrograms.Select(p => p.Name).ToHashSet();

            // Display directory in label
            folderLabel.Content = restoreDir;

            // Generate list of restorable programs, based on db, filtered by directories backed up
            // TODO: Write unit test for directory backed up that's not in db
            IEnumerable<string> restorableProgramsEnumerable = Directory.GetDirectories(restoreDir)
                .Select(dir => Helpers.TrimFilename(dir))
                .Where(programName => allDbProgramNames.Contains(programName));

            restorablePrograms = new ObservableCollection<string>(restorableProgramsEnumerable);
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
            alertMessage.ClearAllAlerts();

            if (restorePageRightList.Items.Count == 0) {
                alertMessage.AddAlertLabel("No programs selected to restore!");
                return;
            }

            // Make sure save directory has been selected
            if (restoreDir == null || restoreDir == "") {
                alertMessage.AddAlertLabel("Must set load location before restoring backup.");
                return;
            }

            IEnumerable<ProgramEntry> selectedPrograms = allDbPrograms.Where(p => addedPrograms.Contains(p.Name));
            CopyHandler copyHandler = new CopyHandler(restoreDir);

            RestoreService.RestoreBackup(selectedPrograms, copyHandler);
        }

        private ObservableCollection<string> restorablePrograms;
        private ObservableCollection<string> addedPrograms;
        private IEnumerable<ProgramEntry> allDbPrograms;
        private AlertMessage alertMessage;
        private string restoreDir;
    }
}
