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

        public RestorePage()
        {
            InitializeComponent();
            restorablePrograms = new ObservableCollection<string>();
            this.leftListBox.ItemsSource = restorablePrograms;

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

        private void restoreButton_Click(object sender, RoutedEventArgs e)
        {
            string restoreDir = Globals.load_save_location;
            // Create a mapping for each file to be restored to its new location
            // TODO: Account for multiple occurrences of a file in the db

            Array files = Directory.GetFiles(restoreDir);
            List<Tuple<string, string>> fileMap = new List<Tuple<string, string>>();

        }

    }
}
