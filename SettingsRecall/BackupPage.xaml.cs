﻿using SettingsRecall.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SettingsRecall {
    public partial class BackupPage : StackPanel {
        public BackupPage() {
            InitializeComponent();

            supportedPrograms = new List<ProgramEntry>();
            backupDir = null;
            leftListItems = new ObservableCollection<ListBoxItem>();
            rightListItems = new ObservableCollection<ListBoxItem>();
            errorMessage = new ErrorMessage(this);

            // Initialize the left list with the names of the supported programs whose paths exist on the machine
            GetUserPrograms();

            backupPageLeftList.ItemsSource = leftListItems;
            backupPageRightList.ItemsSource = rightListItems;
        }

        private void GetUserPrograms() {
            // Clear lists
            supportedPrograms.Clear();
            leftListItems.Clear();

            // Get supported programs
            List<ProgramEntry> programEntries = SQLiteAPI.GetProgramList();

            // Filter list down to programs where at least one path for that program exists on the machine
            // Filter program entries by:
            // - At least one path in the entry exists on user's computer
            foreach (ProgramEntry entry in programEntries) {
                Console.WriteLine(entry.Name);
                foreach (string path in entry.Paths) {
                    if (File.Exists(path) || Directory.Exists(path)) {
                        supportedPrograms.Add(entry);

                        ListBoxItem item = new ListBoxItem();
                        item.Content = entry.Name;
                        leftListItems.Add(item);
                        break;
                    }
                }
            }

            unsupportedPrograms = programEntries.Where(entry => !supportedPrograms.Contains(entry)).ToList();
        }

        private void addProgramButton_Click(object sender, RoutedEventArgs e) {
            // instantiate edit window
            EditProgramWindow editWindow = new EditProgramWindow(null) { Owner = App.mainWindow };
            bool? programEdited = editWindow.ShowDialog();

            if (programEdited.HasValue && programEdited.Value == true)
                GetUserPrograms();
        }

        private void listSelectionChanged(object sender, SelectionChangedEventArgs e) {
            activeList = (ListBox)sender;
        }

        private void editProgramButton_Click(object sender, RoutedEventArgs e) {
            string programName = "";

            if (activeList == backupPageLeftList && backupPageLeftList.SelectedValue != null)
                programName = ((ListBoxItem)backupPageLeftList.SelectedValue).Content.ToString();
            else if (activeList == backupPageRightList && backupPageRightList.SelectedValue != null)
                programName = ((ListBoxItem)backupPageRightList.SelectedValue).Content.ToString();

            if (programName == "") return;

            // instantiate edit window
            EditProgramWindow editWindow = new EditProgramWindow(programName);

            // open the edit window dialog
            editWindow.Owner = App.mainWindow;
            bool? programEdited = editWindow.ShowDialog();

            if (programEdited.HasValue && programEdited.Value == true)
                GetUserPrograms();
        }

        private void chooseFolderButton_Click(object sender, RoutedEventArgs e) {
            backupDir = Helpers.GetFolderPathFromDialog();

            if (backupDir != null)
                folder_label.Content = backupDir;
        }

        private void addToBackupButton_Click(object sender, RoutedEventArgs e) {
            // Remove from left list, add to right list
            ListBoxItem selected = (ListBoxItem)backupPageLeftList.SelectedItem;
            leftListItems.Remove(selected);
            rightListItems.Add(selected);
        }

        private void removeFromBackupButton_Click(object sender, RoutedEventArgs e) {
            // Remove from right list, add to left list
            ListBoxItem selected = (ListBoxItem)backupPageRightList.SelectedItem;
            leftListItems.Add(selected);
            rightListItems.Remove(selected);
        }

        private void showAllProgramsCheckbox_Click(object sender, RoutedEventArgs e) {
            if (showAllProgramsCheckbox.IsChecked.HasValue && showAllProgramsCheckbox.IsChecked.Value == true) {
                foreach (ProgramEntry entry in unsupportedPrograms) {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = entry.Name;
                    item.IsEnabled = false;
                    leftListItems.Add(item);
                }
            } else {
                for (int i = backupPageLeftList.Items.Count - 1; i >= 0; i--) {
                    ListBoxItem item = (ListBoxItem)backupPageLeftList.Items[i];
                    if (unsupportedPrograms.Any(p => p.Name == item.Content.ToString()))
                        leftListItems.RemoveAt(i);
                }
            }
        }

        private void createBackupButton_Click(object sender, RoutedEventArgs e) {
            errorMessage.ClearAllErrors();

            if (backupPageRightList.Items.Count == 0) {
                errorMessage.AddErrorLabel("No programs selected to back up!");
                return;
            }

            // Make sure save directory has been selected
            if (backupDir == null || backupDir == "") {
                errorMessage.AddErrorLabel("Must set save location before creating backup.");
                return;
            }

            // Make sure save directory has a trailing slash (so we can append to it)
            if (!backupDir.Last().Equals('\\'))
                backupDir += "\\";

            CopyHandler copyHandler = new CopyHandler(backupDir, "backup_log.txt", false);

            HashSet<string> selectedProgramNames = backupPageRightList.Items
                .Cast<ListBoxItem>()
                .Select(lbi => lbi.Content.ToString())
                .ToHashSet();

            IEnumerable<ProgramEntry> selectedPrograms = supportedPrograms.Where(p => selectedProgramNames.Contains(p.Name));

            BackupService.CreateBackup(selectedPrograms, copyHandler);
        }

        private ObservableCollection<ListBoxItem> leftListItems;
        private ObservableCollection<ListBoxItem> rightListItems;
        private List<ProgramEntry> supportedPrograms;
        private List<ProgramEntry> unsupportedPrograms;
        private ListBox activeList;
        private string backupDir;
        private ErrorMessage errorMessage;
    }
}