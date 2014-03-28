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

        public RestorePage()
        {
            InitializeComponent();
            Globals.load_save_location = null;
            restorablePrograms = new ObservableCollection<string>();
            addedPrograms = new ObservableCollection<string>();
            this.restorePageLeftList.ItemsSource = restorablePrograms;
            this.restorePageRightList.ItemsSource = addedPrograms;
            this.restoreButton.IsEnabled = false;
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
            {
                Globals.load_save_location = open_dialog.SelectedPath;
            }
            else
            {
                return;
            }

            string restoreDir = Globals.load_save_location;

            // Connect to local database
            string dbPath = restoreDir + @"\" + "test.db"; // or whatever it will be called
            if (File.Exists(dbPath)) 
            {
                Globals.sqlite_db = new SQLiteDatabase(dbPath);
            }
            else
            {
                // Display an error message - db file not found
                ErrorMessageBox err = new ErrorMessageBox("Restore info not found.");
                err.show();
                return;
            }

            // Display directory in label
            folderLabel.Content = Globals.load_save_location;

            // Generate list of restorable programs
            // Currently this list is just the directories in the load location path
            Array dirs = Directory.GetDirectories(restoreDir);
            restorablePrograms.Clear();
            foreach (string progName in dirs)
            {
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

        private void restoreButton_Click(object sender, RoutedEventArgs e)
        {
            string restoreDir = Globals.load_save_location;
            // Create a mapping for each file to be restored to its new location
            // WARNING: NAIVE IMPLEMENTATION
            // TODO: Account for multiple occurrences of a file in the db

            // Create log file
            StreamWriter log = new StreamWriter(restoreDir + @"\" + "restore_log.txt");
            log.WriteLine("--- SettingsRecall restore initiated " + DateTime.Now + ". ---");

            Array dirs = Directory.GetDirectories(restoreDir);
            Array files;
            List<Tuple<string, string>> fileMap = new List<Tuple<string, string>>();
            ProgramEntry currentEntry;

            log.WriteLine("\n*Creating restoration queue*\n");

            foreach (string dir in dirs)
            {
                files = Directory.GetFiles(dir);
                currentEntry = Globals.sqlite_api.GetProgram(Helpers.TrimFilename(dir));
               
                foreach (string srcPath in files)
                {
                    // Find restore path in db for current file
                    foreach(string dstPath in currentEntry.Paths) {
                        if (Helpers.TrimFilename(srcPath).Equals(Helpers.TrimFilename(dstPath))) {
                            // Map the file to it's restore path
                            fileMap.Add(new Tuple<string,string>(srcPath, dstPath));
                            log.WriteLine("Queued " + srcPath);
                            log.WriteLine("  to copy to " + dstPath);
                        }
                    }   
                }
            }

            // Copy files
            // COPYING CURRENTLY DISABLED FOR SAFETY!!
            log.WriteLine("\nCopying files...");
            foreach (Tuple<string, string> map in fileMap)
            {
                // TODO: THIS IMPLEMENTATION IS CRAP. ADD COPY "RULES"
                //File.Copy(map.Item1, map.Item2);
                log.WriteLine("Copied " + map.Item1);
                log.WriteLine("  to " + map.Item2);
            }
            log.WriteLine("Copying complete.\n");

            // End log file
            log.WriteLine("--- SettingsRecall restore completed " + DateTime.Now + " ---");
            log.Close();

        }


    }
}
