using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using System.Data;

namespace SettingsRecall
{
    /// <summary>
    /// Interaction logic for AddProgramWindow.xaml
    /// </summary>
    public partial class AddProgramWindow : Window
    {
        private ObservableCollection<string> PathList = new ObservableCollection<string>();
        
        public AddProgramWindow()
        {
            InitializeComponent();
            PathListBox.ItemsSource = PathList;
            DataTable programTable = Globals.sqlite_api.GetProgramList("String");
        }

        // click add files button
        // adds paths to the list from an open file dialog
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // instantiate new OpenFileDialog
            Microsoft.Win32.OpenFileDialog open_dlg = new Microsoft.Win32.OpenFileDialog();
            open_dlg.CheckFileExists = false; // user CAN enter nonexistent files!
            open_dlg.Multiselect = true; // allow selection of multiple files

            // show dialog
            Nullable<bool> result = open_dlg.ShowDialog();

            // add selected file paths to list
            if (result == true)
            {
                foreach (string path in open_dlg.FileNames)
                {
                    PathList.Add(path);
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteListItem(PathListBox.SelectedIndex);
        }

        /// <summary>
        /// delete item at index from PathList
        /// </summary>
        /// <param name="index">PathList index</param>
        private void DeleteListItem(int index)
        {
            if (index > -1) // an item is selected
            {
                PathList.RemoveAt(index);
            }
        }

        private void SaveProgramButton_Click(object sender, RoutedEventArgs e)
        {
            string name = ProgramNameText.Text;
            string version = VersionNameText.Text;
            string description = descriptionText.Text;

            // Make sure all fields are valid
            // check 'program name'. Make sure name is valid and not already
            // in the database.
            if (name.Length < 1)
            {
                ErrorMessageBox no_name_msg = new ErrorMessageBox("Please enter a program name.");
                no_name_msg.show();
                return;
            }
            else
            {
                ;// search the db
            }

            // check the 'version name'
            if (version.Length < 1)
            {
                ErrorMessageBox no_ver_msg = new ErrorMessageBox("Please enter a version name.");
                no_ver_msg.show();
                return;
            }
            else
            {
                ; // other restrictions?
            }

            // check the file list
            if (PathList.Count < 1)
            {
                ErrorMessageBox no_files_msg = new ErrorMessageBox("No files selected.");
                no_files_msg.show();
                return;
            }
            else
            {
                ; // other restrictions?
            }

            // NEEDS UPDATING TO NEW API
            /*
            // Add program to db. GeneratePaths creates the 'paths' dictionary for the db.
            if (!Globals.sqlite_api.AddProgram(name, GeneratePaths(version, PathList), description)) {
                // Error!
                // Handle Me!
                ;
            }
            */

            this.Close();
        }

        /// <summary>
        /// Helper - Generates a paths dictionary which can be used in the db.
        /// </summary>
        /// <param name="version">Version name</param>
        /// <param name="files">Files list</param>
        /// <returns></returns>
        private Dictionary<string, Dictionary<string, string>> GeneratePaths(string version, ObservableCollection<string> files)
        {
            int index = 0;
            Dictionary<string, Dictionary<string, string>> paths =
                new Dictionary<string,Dictionary<string,string>>();
            Dictionary<string, string> inner = 
                new Dictionary<string,string>();
            
            // create the inner dictionary first
            foreach (string path in PathList)
            {
                inner.Add(index.ToString(), path);
                index++;
            }

            // insert the inner into paths along with the version name
            paths.Add(version, inner);

            return paths;
        }


    }
}
