using System;
using System.Collections.Generic;
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
using System.IO;
using System.Collections.ObjectModel;

namespace SettingsRecall
{
    /// <summary>
    /// Interaction logic for RestorePage.xaml
    /// </summary>
    public partial class RestorePage : UserControl
    {
        private ObservableCollection<string> restorablePrograms;
        private ObservableCollection<string> addedPrograms;
        private IEnumerable<ProgramEntry> allDbPrograms;
        private IEnumerable<string> allDbProgramNames;

        public RestorePage()
        {
            InitializeComponent();
            Globals.load_save_location = null;
            restorablePrograms = new ObservableCollection<string>();
            addedPrograms = new ObservableCollection<string>();

            restorePageLeftList.ItemsSource = restorablePrograms;
            restorePageRightList.ItemsSource = addedPrograms;
            restoreButton.IsEnabled = false;
        }

        // click 'choose folder' button
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
            else
                return;

            // Make sure save directory has a trailing slash (so we can append to it)
            if (!Globals.load_save_location.Last().Equals('\\'))
                Globals.load_save_location += "\\";

            string restoreDir = Globals.load_save_location;

            // Find database file
            string[] dbFileMatches = Directory.GetFiles(restoreDir, "*.db");

            if (dbFileMatches.Length == 0) {
                ErrorMessageBox err = new ErrorMessageBox("Restore info not found.");
                err.show();
                return;
            } else if (dbFileMatches.Length > 1) {
                ErrorMessageBox err = new ErrorMessageBox("Multiple possible databases found.");
                err.show();
                return;
            }

            allDbPrograms = SQLiteAPI.GetProgramList();
            allDbProgramNames = allDbPrograms.Select(p => p.Name).ToList();

            // Display directory in label
            folderLabel.Content = Globals.load_save_location;

            // Generate list of restorable programs, based on db, filtered by directories backed up
            // TODO: Write unit test for directory backed up that's not in db
            restorablePrograms.Clear();
            string[] dirs = Directory.GetDirectories(restoreDir);

            foreach (string progName in dirs)
            {
                if (allDbProgramNames.Contains(Helpers.TrimFilename(progName)))
                    restorablePrograms.Add(progName.Split('\\').Last());
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            // Make sure something is selected
            if (restorePageLeftList.SelectedIndex == -1)
                return;

            // Remove from left list, add to right list
            string selected = restorePageLeftList.SelectedItem.ToString();
            addedPrograms.Add(selected);
            restorablePrograms.Remove(selected);

            // Enable the 'Restore' button if necessary
            if (addedPrograms.Count > 0 && Globals.load_save_location != null)
            {
                restoreButton.IsEnabled = true;
            }
        }

        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            // Make sure something is selected
            if (restorePageRightList.SelectedIndex == -1)
                return;

            // Remove from right list, add to left list
            string selected = restorePageRightList.SelectedItem.ToString();
            restorablePrograms.Add(selected);
            addedPrograms.Remove(selected);

            // Disable the 'Restore button if necessary
            if (addedPrograms.Count == 0 || Globals.load_save_location == null)
            {
                restoreButton.IsEnabled = false;
            }
        }

        private void restoreButton_Click(object sender, RoutedEventArgs e) {
            IEnumerable<ProgramEntry> selectedPrograms = allDbPrograms.Where(p => addedPrograms.Contains(p.Name));

            foreach (ProgramEntry program in selectedPrograms) {
                string backupProgramDir = Globals.load_save_location + program.Name;

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
    }
}
