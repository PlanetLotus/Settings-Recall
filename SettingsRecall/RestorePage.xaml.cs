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

        /// <summary>
        /// Set the global save/load location with a dialog
        /// </summary>
        public void SetLoadSaveLocation()
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

        }

        // click 'choose folder' button
        private void chooseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            SetLoadSaveLocation();
            string restoreDir = Globals.load_save_location;

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

    }
}
