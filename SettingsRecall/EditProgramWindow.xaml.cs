using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace SettingsRecall {
    public partial class EditProgramWindow : Window {
        public EditProgramWindow(string name) {
            InitializeComponent();

            // Give window focus to the first control in tab order
            Loaded += (sender, e) =>
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            // Get the data for the current programEntry object
            if (name != null)
                currentEntry = SQLiteAPI.GetProgram(name);

            if (name == null || currentEntry == null) {
                currentEntry = new ProgramEntry();
                newEntry = true;
                programNameText.IsEnabled = true;
                fileCollection = new ObservableCollection<string>();
            } else {
                programNameText.Text = name;
                descriptionText.Text = currentEntry.Description;
                fileCollection = new ObservableCollection<string>(currentEntry.Paths);

                if (currentEntry.Paths.Count != 0)
                    deleteFilesButton.IsEnabled = true;
            }

            fileListBox.ItemsSource = fileCollection;
        }

        private void addFilesButton_Click(object sender, RoutedEventArgs e) {
            // Open a file browser dialog
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Multiselect = true;

            if (openDlg.ShowDialog() == false)
                return;

            // Add paths to list
            foreach (string path in openDlg.FileNames)
                fileCollection.Add(path);

            deleteFilesButton.IsEnabled = true;
        }

        private void addFolderButton_Click(object sender, RoutedEventArgs e) {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrEmpty(dialog.SelectedPath)) {
                fileCollection.Add(dialog.SelectedPath);
                deleteFilesButton.IsEnabled = true;
            }
        }

        private void deleteFilesButton_Click(object sender, RoutedEventArgs e) {
            List<string> selectedFiles = fileListBox.SelectedItems.Cast<string>().ToList();
            foreach (string item in selectedFiles)
                fileCollection.Remove(item);

            if (fileCollection.Count == 0)
                deleteFilesButton.IsEnabled = false;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e) {
            currentEntry.Name = programNameText.Text;
            currentEntry.Description = descriptionText.Text;
            currentEntry.Paths = fileCollection.ToList();

            // edit (or add) the entry in the database
            if (newEntry)
                DialogResult = SQLiteAPI.AddProgram(currentEntry);
            else
                DialogResult = SQLiteAPI.EditProgram(currentEntry);

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
                DialogResult = SQLiteAPI.DeleteProgram(currentEntry.Name);
                Close();
            }
        }

        private ProgramEntry currentEntry;
        private bool newEntry = false;
        private ObservableCollection<string> fileCollection;
    }
}
