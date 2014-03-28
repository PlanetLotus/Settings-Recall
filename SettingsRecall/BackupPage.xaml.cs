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
using System.Windows.Shapes;

namespace SettingsRecall
{
    /// <summary>
    /// Interaction logic for BackupPage.xaml
    /// </summary>
    public partial class BackupPage : UserControl
    {
        // TODO: Keep these private. They're marked public now so that TestBackup.cs can use them.
        public List<ProgramEntry> supportedPrograms;
        public List<string> supportedProgramNames;
        public List<ProgramEntry> selectedPrograms;

        public BackupPage()
        {
            InitializeComponent();

            supportedPrograms = new List<ProgramEntry>();
            supportedProgramNames = new List<string>();
            selectedPrograms = new List<ProgramEntry>();

            // Initialize the left list with the names of the supported programs whose paths exist on the machine
            // Not using ItemsSource here because we don't want the list to be read-only
            GetUserPrograms();
            foreach (string name in supportedProgramNames)
                backupPageLeftList.Items.Add(name);
        }

        private void GetUserPrograms() {
            // Get supported programs
            List<ProgramEntry> programEntries = Globals.sqlite_api.GetProgramList();
            if (programEntries == null) return;

            // Get OS of machine we're on
            string this_os = Helpers.GetOSFriendlyName();

            // Filter list down to programs where at least one path for that program exists on the machine
            // Filter program entries by:
            // - At least one path in the entry exists on user's computer
            foreach (ProgramEntry entry in programEntries) {
                Console.WriteLine(entry.Name);
                foreach (string path in entry.Paths) {
                    if (File.Exists(path) || Directory.Exists(path)) {
                        supportedPrograms.Add(entry);
                        supportedProgramNames.Add(entry.Name);
                        break;
                    }
                }
            }
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
            editWindow.ShowDialog();
        }

        private void editProgramButton_Click(object sender, RoutedEventArgs e)
        {
            // instantiate 'choose program' dialog box
            ChooseEditProgramWindow chooseWindow = new ChooseEditProgramWindow();

            // configure dialog box and open modally
            chooseWindow.Owner = App.mainWindow;
            chooseWindow.ShowDialog();

            if (chooseWindow.DialogResult == false) return;

            // get the name entered into the add dialog
            string programName = chooseWindow.GetProgramName();

            // instantiate edit window
            EditProgramWindow editWindow = new EditProgramWindow(programName);

            // open the edit window dialog
            editWindow.Owner = App.mainWindow;
            editWindow.ShowDialog();
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
            object selected = backupPageLeftList.SelectedItem;
            backupPageRightList.Items.Add(selected);
            backupPageLeftList.Items.Remove(selected);
        }

        private void removeFromBackupButton_Click(object sender, RoutedEventArgs e) {
            // Remove from right list, add to left list
            object selected = backupPageRightList.SelectedItem;
            backupPageLeftList.Items.Add(selected);
            backupPageRightList.Items.Remove(selected);
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

            // Determine which programs are selected
            selectedPrograms.Clear();
            foreach (ProgramEntry program in supportedPrograms) {
                if (backupPageRightList.Items.Contains(program.Name))
                    selectedPrograms.Add(program);
            }

            // Create folder at save location if it doesn't exist already
            Directory.CreateDirectory(backupDir);

            // Create log file
            StreamWriter log = new StreamWriter(backupDir + "log.txt");
            log.WriteLine("--- SettingsRecall backup initiated " + DateTime.Now + ". ---");

            log.WriteLine("Copying Files...");
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
                    string filename = path.Split('\\').Last();
                    if (File.Exists(path)) {
                        log.WriteLine("Found " + path);
                        File.Copy(path, programDir + "\\" + filename);
                    } else {
                        log.WriteLine("Couldn't find " + path);
                        Console.WriteLine("File not found: " + path);
                    }
                }
            }
            log.WriteLine("Copying complete.");

            // End log file
            log.WriteLine("--- SettingsRecall backup completed " + DateTime.Now + " ---");
            log.Close();
        }
    }
}