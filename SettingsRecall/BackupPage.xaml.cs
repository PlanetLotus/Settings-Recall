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

namespace SettingsRecall
{
    /// <summary>
    /// Interaction logic for BackupPage.xaml
    /// </summary>
    public partial class BackupPage : UserControl
    {
        public BackupPage()
        {
            InitializeComponent();
        }
        
        // Set the global save/load location with a dialog
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

        private void addProgramButton_Click(object sender, RoutedEventArgs e)
        {
            // instantiate dialog box
            AddProgramWindow APWindow = new AddProgramWindow();

            // configure dialog box and open modally
            APWindow.Owner = App.mainWindow;
            APWindow.ShowDialog();
        }

        private void editProgramButton_Click(object sender, RoutedEventArgs e)
        {
            // instantiate 'choose program' dialog box
            ChooseEditProgramWindow chooseWindow = new ChooseEditProgramWindow();

            // configure dialog box and open modally
            chooseWindow.Owner = App.mainWindow;
            chooseWindow.ShowDialog();
        }

        private void chooseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            SetLoadSaveLocation();
            // display directory in label
            folder_label.Content = Globals.load_save_location;
        }

    }
}