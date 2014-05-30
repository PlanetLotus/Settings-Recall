using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace SettingsRecall
{
    /// <summary>
    /// Interaction logic for BackupPage.xaml
    /// </summary>
    public partial class BackupPage : UserControl
    {
        // TODO: Keep these private. They're marked public now so that TestBackup.cs can use them.
        public List<ProgramEntry> supportedPrograms;
        public List<ProgramEntry> unsupportedPrograms;
        public List<string> supportedProgramNames;
        public List<ProgramEntry> selectedPrograms;

        private ListBox activeList;

        public BackupPage()
        {
            InitializeComponent();

            supportedPrograms = new List<ProgramEntry>();
            selectedPrograms = new List<ProgramEntry>();

            // Initialize the left list with the names of the supported programs whose paths exist on the machine
            // Not using ItemsSource here because we don't want the list to be read-only
            GetUserPrograms();
        }

        private void GetUserPrograms() {
            // Clear lists
            supportedPrograms.Clear();
            selectedPrograms.Clear();
            backupPageLeftList.Items.Clear();

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
                        backupPageLeftList.Items.Add(item);
                        break;
                    }
                }
            }

            unsupportedPrograms = programEntries.Where(entry => !supportedPrograms.Contains(entry)).ToList();
        }
        
        private void addProgramButton_Click(object sender, RoutedEventArgs e)
        {
            // instantiate dialog box
            AddNewProgramWindow ANPWindow = new AddNewProgramWindow();

            // configure dialog box and open modally
            ANPWindow.Owner = App.mainWindow;
            ANPWindow.ShowDialog();

            if (ANPWindow.DialogResult == false)
            {
                // do not continue
                return;
            }

            // get the name entered into the add dialog
            string programName = ANPWindow.GetProgramName();

            // instantiate edit window
            EditProgramWindow editWindow = new EditProgramWindow(programName);

            // open the edit window dialog
            editWindow.Owner = App.mainWindow;
            bool? programEdited = editWindow.ShowDialog();

            if (programEdited.HasValue && programEdited.Value == true)
                GetUserPrograms();
        }

        private void listSelectionChanged(object sender, SelectionChangedEventArgs e) {
            activeList = (ListBox) sender;
        }

        private void editProgramButton_Click(object sender, RoutedEventArgs e)
        {
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

        private void chooseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult result; // return value of dialog box

            // open a dialog
            System.Windows.Forms.FolderBrowserDialog open_dialog = new System.Windows.Forms.FolderBrowserDialog();
            open_dialog.ShowNewFolderButton = true;
            result = open_dialog.ShowDialog();

            // set global variable
            if (result == System.Windows.Forms.DialogResult.OK)
                Globals.load_save_location = open_dialog.SelectedPath;

            // display directory in label
            folder_label.Content = Globals.load_save_location;
        }

        private void addToBackupButton_Click(object sender, RoutedEventArgs e) {
            // Remove from left list, add to right list
            ListBoxItem selected = (ListBoxItem) backupPageLeftList.SelectedItem;
            backupPageLeftList.Items.Remove(selected);
            backupPageRightList.Items.Add(selected);

            selectedPrograms.Add(supportedPrograms.Where(p => p.Name == selected.Content.ToString()).Single());
        }

        private void removeFromBackupButton_Click(object sender, RoutedEventArgs e) {
            // Remove from right list, add to left list
            ListBoxItem selected = (ListBoxItem) backupPageRightList.SelectedItem;
            backupPageRightList.Items.Remove(selected);
            backupPageLeftList.Items.Add(selected);

            selectedPrograms.RemoveAll(p => p.Name == selected.Content.ToString());
        }

        private void showAllProgramsCheckbox_Click(object sender, RoutedEventArgs e) {
            if (showAllProgramsCheckbox.IsChecked.HasValue && showAllProgramsCheckbox.IsChecked.Value == true) {
                foreach (ProgramEntry entry in unsupportedPrograms) {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = entry.Name;
                    item.IsEnabled = false;
                    backupPageLeftList.Items.Add(item);
                }
            } else {
                // There's probably a better way to remove from a collection...
                for (int i = backupPageLeftList.Items.Count-1; i >= 0; i--) {
                    ListBoxItem item = (ListBoxItem) backupPageLeftList.Items[i];
                    if (unsupportedPrograms.Any(p => p.Name == item.Content.ToString()))
                        backupPageLeftList.Items.RemoveAt(i);
                }
            }
        }

        // TODO: Keep this private. It's marked public now so that TestBackup.cs can use it.
        public void createBackupButton_Click(object sender, RoutedEventArgs e) {
            // Make sure save directory has been selected
            string backupDir = Globals.load_save_location;
            if (backupDir == null || backupDir.Trim() == "") {
                Console.WriteLine("Must set save location before creating backup.");
                return;
            }

            // Make sure save directory has a trailing slash (so we can append to it)
            if (!backupDir[backupDir.Length-1].Equals('\\'))
                backupDir = backupDir + "\\";

            CopyHandler copyHandler = new CopyHandler(backupDir, "backup_log.txt");
            copyHandler.InitBackup();

            // Loop through selectedPrograms, copying files to save location
            foreach (ProgramEntry program in selectedPrograms) {
                // Create folder for program in save location
                string programDir = backupDir + program.Name;
                Directory.CreateDirectory(programDir);

                // Check edge case: Multiple files of same name
                // Implement later...
                // Get number of versions in each program, x
                // Get strings that have multiple versions, strList
                // Create x subdirs in program dir
                // When copying, if filename in strList, copy to a subdir that doesn't contain filename

                foreach (string path in program.Paths) {
                    // Copy files at path to programDir
                    // It's okay (and expected) for not all paths to exist
                    string filename = path.Split('\\').Last();
                    copyHandler.Copy(path, programDir + "\\" + filename);
                }
            }

            copyHandler.CloseBackup();
        }
    }
}