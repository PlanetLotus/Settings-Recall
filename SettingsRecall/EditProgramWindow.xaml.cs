using System;
using System.Diagnostics;
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
using Microsoft.Win32;

namespace SettingsRecall
{
    /// <summary>
    /// Interaction logic for EditProgramWindow.xaml
    /// </summary>
    public partial class EditProgramWindow : Window
    {

        // Window global variables
        private ProgramEntry currentEntry;
        private bool newEntry = false;
        private ObservableCollection<string> fileCollection;
        
        public EditProgramWindow(string name)
        {
            InitializeComponent();

            // Give window focus to the first control in tab order
            Loaded += (sender, e) =>
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            // Get the data for the current programEntry object
            currentEntry = Globals.sqlite_api.GetProgram(name);
            if (currentEntry == null)
            {
                currentEntry = new ProgramEntry(name, false, new List<string>());
                newEntry = true;
            }

            programNameText.Text = name;
            descriptionText.Text = currentEntry.Description;

            // populate the list of files
            if (currentEntry.Paths.Count > 0)
            {
                fileCollection = new ObservableCollection<string>(currentEntry.Paths);
                deleteFilesButton.IsEnabled = true;
            }
            else
            {
                fileCollection = new ObservableCollection<string>();
                deleteFilesButton.IsEnabled = false;
            }

            fileListBox.ItemsSource = fileCollection;
        }

        private void addFilesButton_Click(object sender, RoutedEventArgs e)
        {
            // Open a file browser dialog
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Multiselect = true;

            Nullable<bool> result = openDlg.ShowDialog();

            if (result == false)
                return;

            // Add paths to list
            foreach (string path in openDlg.FileNames)
                fileCollection.Add(path);

            deleteFilesButton.IsEnabled = true;
        }

        private void deleteFilesButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedFiles = fileListBox.SelectedItems.Cast<string>().ToList();
            foreach (string item in selectedFiles)
                fileCollection.Remove(item);

            if (fileCollection.Count < 1)
                deleteFilesButton.IsEnabled = false;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            currentEntry.Name = programNameText.Text;
            currentEntry.Description = descriptionText.Text;
            currentEntry.Paths = fileCollection.ToList();

            // edit (or add) the entry in the database
            if (newEntry)
                Globals.sqlite_api.AddProgram(currentEntry);
            else
                Globals.sqlite_api.EditProgram(currentEntry);

            DialogResult = true;
            Close();
        }

        private void deleteProgramButton_Click(object sender, RoutedEventArgs e) {
            MessageBoxResult messageBoxResult = MessageBox.Show(
                this,
                "Are you sure you want to delete this program?",
                "Confirm Delete Program",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (messageBoxResult == MessageBoxResult.Yes) {
                Globals.sqlite_api.DeleteProgram(currentEntry.Name);
                DialogResult = true;
                Close();
            }
        }
    }
}
