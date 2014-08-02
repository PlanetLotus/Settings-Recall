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

            GetUserPrograms();

            backupPageProgramList.ItemsSource = programListBoxItems;
        }

        public ObservableCollection<ListBoxItem> ProgramListItems { get; set; }

        private void GetUserPrograms() {
            supportedPrograms.Clear();
            programListBoxItems.Clear();

            List<ProgramEntry> programEntries = SQLiteAPI.GetProgramList();

            foreach (ProgramEntry entry in programEntries) {
                bool isSupported = entry.Paths.Any(path => File.Exists(path) || Directory.Exists(path));
                Visibility visibility = isSupported ? Visibility.Visible : Visibility.Collapsed;

                ProgramListBoxItem item = new ProgramListBoxItem {
                    IsChecked = false, Name = entry.Name, IsSupported = isSupported, Visibility = visibility
                };
                programListBoxItems.Add(item);

                if (isSupported)
                    supportedPrograms.Add(entry);
            }

            selectAllButton.IsEnabled = supportedPrograms.Count != 0;
            selectNoneButton.IsEnabled = supportedPrograms.Count != 0;
        }

        private void addProgramButton_Click(object sender, RoutedEventArgs e) {
            EditProgramWindow editWindow = new EditProgramWindow(null) { Owner = App.mainWindow };
            bool? programEdited = editWindow.ShowDialog();

            if (programEdited.HasValue && programEdited.Value == true)
                GetUserPrograms();
        }

        private void editProgramButton_Click(object sender, RoutedEventArgs e) {
            string programName = ((ProgramListBoxItem)backupPageProgramList.SelectedValue).Name;

            if (programName == null || programName == "") return;

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
            if (showAllProgramsCheckbox.IsChecked == true) {
                foreach (ProgramListBoxItem item in programListBoxItems.Where(item => !item.IsSupported))
                    item.Visibility = Visibility.Visible;
            } else {
                foreach (ProgramListBoxItem item in programListBoxItems.Where(item => !item.IsSupported))
                    item.Visibility = Visibility.Collapsed;
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
        private string backupDir;
        private AlertMessage alertMessage;
    }
}