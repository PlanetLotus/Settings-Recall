using Newtonsoft.Json;
using SettingsRecall.Utility;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BackupDataModel = SettingsRecall.BackupService.BackupDataModel;

namespace SettingsRecall {
    public partial class RestorePage : StackPanel {
        public RestorePage() {
            InitializeComponent();
            alertMessage = new AlertMessage(this);
            restoreDir = null;

            restoreButton.IsEnabled = false;
        }

        private void chooseFolderButton_Click(object sender, RoutedEventArgs e) {
            restoreDir = Helpers.GetFolderPathFromDialog();

            if (restoreDir == null) return;

            alertMessage.ClearAllAlerts();

            // Find backup data file
            string[] jsonFileMatches = Directory.GetFiles(restoreDir, "*.json");

            if (jsonFileMatches.Length == 0) {
                alertMessage.AddAlertLabel("Restore info not found.");
                return;
            } else if (jsonFileMatches.Length > 1) {
                alertMessage.AddAlertLabel("Multiple possible databases found.");
                return;
            }

            // Display directory in label
            folderLabel.Content = restoreDir;

            string backupDataString = File.ReadAllText(jsonFileMatches.Single());
            allBackedUpPrograms = JsonConvert.DeserializeObject<IEnumerable<BackupDataModel>>(backupDataString);
            HashSet<string> allDbProgramNames = allBackedUpPrograms.Select(p => p.ProgramName).ToHashSet();

            // Generate list of restorable programs, based on json data, filtered by directories backed up
            // TODO: Write unit test for directory backed up that's not in json data
            IEnumerable<ProgramListBoxItem> restorableProgramsEnumerable = Directory.GetDirectories(restoreDir)
                .Select(dir => Helpers.TrimFilename(dir))
                .Where(programName => allDbProgramNames.Contains(programName))
                .Select(name => new ProgramListBoxItem { Name = name, Visibility = Visibility.Visible, IsSupported = true, IsChecked = false });

            programListBoxItems = new ObservableCollection<ProgramListBoxItem>(restorableProgramsEnumerable);
            restorePageProgramList.ItemsSource = programListBoxItems;

            selectAllButton.IsEnabled = programListBoxItems.Count != 0;
            selectNoneButton.IsEnabled = programListBoxItems.Count != 0;
        }

        private void selectAllButton_Click(object sender, RoutedEventArgs e) {
            foreach (ProgramListBoxItem item in programListBoxItems.Where(item => item.IsSupported && !item.IsChecked))
                item.IsChecked = true;
        }

        private void selectNoneButton_Click(object sender, RoutedEventArgs e) {
            foreach (ProgramListBoxItem item in programListBoxItems.Where(item => item.IsSupported && item.IsChecked))
                item.IsChecked = false;
        }

        private void restoreButton_Click(object sender, RoutedEventArgs e) {
            alertMessage.ClearAllAlerts();

            List<ProgramListBoxItem> checkedItems = programListBoxItems.Where(item => item.IsChecked).ToList();

            if (checkedItems.Count == 0) {
                alertMessage.AddAlertLabel("No programs selected to restore!");
                return;
            }

            // Make sure save directory has been selected
            if (restoreDir == null || restoreDir == "") {
                alertMessage.AddAlertLabel("Must set load location before restoring backup.");
                return;
            }

            HashSet<string> selectedProgramNames = checkedItems.Select(item => item.Name).ToHashSet();
            IEnumerable<BackupDataModel> selectedPrograms = allBackedUpPrograms.Where(p => selectedProgramNames.Contains(p.ProgramName));

            CopyHandler copyHandler = new CopyHandler(restoreDir);

            RestoreService.RestoreBackup(selectedPrograms, copyHandler);
        }

        private ObservableCollection<ProgramListBoxItem> programListBoxItems;
        private IEnumerable<BackupDataModel> allBackedUpPrograms;
        private AlertMessage alertMessage;
        private string restoreDir;
    }
}
