using SettingsRecall.Utility;
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
            programListBoxItems = new ObservableCollection<ProgramListBoxItem>();
            alertMessage = new AlertMessage(this);

            // Initialize the left list with the names of the supported programs whose paths exist on the machine
            GetUserPrograms();

            backupPageProgramList.ItemsSource = programListBoxItems;
        }

        public ObservableCollection<ListBoxItem> ProgramListItems { get; set; }

        private void GetUserPrograms() {
            // Clear lists
            supportedPrograms.Clear();
            programListBoxItems.Clear();

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

                        ProgramListBoxItem item = new ProgramListBoxItem { IsChecked = false, Name = entry.Name, IsSupported = true };
                        programListBoxItems.Add(item);
                        break;
                    }
                }
            }

            unsupportedPrograms = programEntries.Where(entry => !supportedPrograms.Contains(entry)).ToList();

            selectAllButton.IsEnabled = supportedPrograms.Count != 0;
            selectNoneButton.IsEnabled = supportedPrograms.Count != 0;
        }

        private void addProgramButton_Click(object sender, RoutedEventArgs e) {
            // instantiate edit window
            EditProgramWindow editWindow = new EditProgramWindow(null) { Owner = App.mainWindow };
            bool? programEdited = editWindow.ShowDialog();

            if (programEdited.HasValue && programEdited.Value == true)
                GetUserPrograms();
        }

        private void editProgramButton_Click(object sender, RoutedEventArgs e) {
            string programName = ((ProgramListBoxItem)backupPageProgramList.SelectedValue).Name;

            if (programName == null || programName == "") return;

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

            if (Directory.EnumerateFileSystemEntries(backupDir).Any())
                alertMessage.AddAlertLabel(
                    "Warning! This backup folder is not empty and its contents will be erased as soon as Create Backup is clicked.",
                    AlertMessage.AlertLevel.Warn);
        }

        private void showAllProgramsCheckbox_Click(object sender, RoutedEventArgs e) {
            if (showAllProgramsCheckbox.IsChecked.HasValue && showAllProgramsCheckbox.IsChecked.Value == true) {
                foreach (ProgramEntry entry in unsupportedPrograms) {
                    ProgramListBoxItem item = new ProgramListBoxItem { Name = entry.Name, IsChecked = false, IsSupported = false };
                    programListBoxItems.Add(item);
                }
            } else {
                for (int i = backupPageProgramList.Items.Count - 1; i >= 0; i--) {
                    ProgramListBoxItem item = (ProgramListBoxItem)backupPageProgramList.Items[i];
                    if (unsupportedPrograms.Any(p => p.Name == item.Name))
                        programListBoxItems.RemoveAt(i);
                }
            }
        }

        private void selectAllButton_Click(object sender, RoutedEventArgs e) {
            foreach (ProgramListBoxItem item in programListBoxItems.Where(item => item.IsSupported && !item.IsChecked))
                item.IsChecked = true;
        }

        private void selectNoneButton_Click(object sender, RoutedEventArgs e) {
            foreach (ProgramListBoxItem item in programListBoxItems.Where(item => item.IsSupported && item.IsChecked))
                item.IsChecked = false;
        }

        private void createBackupButton_Click(object sender, RoutedEventArgs e) {
            alertMessage.ClearAllAlerts();

            List<ProgramListBoxItem> checkedItems = programListBoxItems.Where(item => item.IsChecked).ToList();

            if (checkedItems.Count == 0) {
                alertMessage.AddAlertLabel("No programs selected to back up!");
                return;
            }

            // Make sure save directory has been selected
            if (backupDir == null || backupDir == "") {
                alertMessage.AddAlertLabel("Must set save location before creating backup.");
                return;
            }

            CopyHandler copyHandler = new CopyHandler(backupDir);

            HashSet<string> selectedProgramNames = checkedItems
                .Select(item => item.Name)
                .ToHashSet();

            IEnumerable<ProgramEntry> selectedPrograms = supportedPrograms.Where(p => selectedProgramNames.Contains(p.Name));

            BackupService.CreateBackup(selectedPrograms, copyHandler);
        }

        private ObservableCollection<ProgramListBoxItem> programListBoxItems;
        private List<ProgramEntry> supportedPrograms;
        private List<ProgramEntry> unsupportedPrograms;
        private ListBox activeList;
        private string backupDir;
        private AlertMessage alertMessage;
    }
}