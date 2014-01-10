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

namespace SettingsRecall
{
    /// <summary>
    /// Interaction logic for EditProgramWindow.xaml
    /// </summary>
    public partial class EditProgramWindow : Window
    {

        // Window global variables
        private string currentName;
        private ProgramEntry selectedEntry;
        private List<EntryChange> changeList;                   // list of changes made
        private ObservableCollection<VersionListEntry> entryList;   // list of all entries for the program
        
        public EditProgramWindow(string name)
        {
            InitializeComponent();

            // update currentName
            this.currentName = name;
            this.programNameLabel.Content = this.currentName;

            // instantiate global changeList for edit session
            changeList = new List<EntryChange>();

            // get the full program list for the current program
            //entryList = new ObservableCollection<VersionListEntry>();
            entryList = generateProgramList();
            versionListBox.ItemsSource = entryList;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            // instantiate new change
            EntryChange change = new EntryChange();
            change.entry_type = "add";

            // instantiate entry
            ProgramEntry entry = new ProgramEntry();
            entry.Name = this.currentName;
            
            // add entry to entry change
            change.entry = entry;

            // add entry change to list
            changeList.Add(change);

            // add entry to entryList
            VersionListEntry vle = new VersionListEntry(entry);
            System.Console.WriteLine(vle.ToString());
            entryList.Add(vle);
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            // Make sure a version is selected
            if (versionListBox.SelectedIndex >= 0)
            {
                EnableFieldsForEdit(true);
            }
            else
            {
                ErrorMessageBox selectVersionErrorBox = new ErrorMessageBox("Please select a version to edit");
                selectVersionErrorBox.show();
            }
        }

        private void deleteVersionButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            // create a new ProgramEntry
            ProgramEntry entry_edit = new ProgramEntry();
            entry_edit = selectedEntry;

            // add edits
            //entry_edit

            // disable the edit fields
            EnableFieldsForEdit(false);    
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            // process changeList
            foreach (EntryChange change in changeList)
            {
                switch (change.entry_type)
                {
                    case "add":
                        if (!Globals.sqlite_api.AddProgramEntry(change.entry))
                        {
                            // handle the error
                        }
                        break;
                    case "edit":
                        if (!Globals.sqlite_api.EditProgramEntry(change.entry))
                        {
                            // handle the error
                        }
                        break;
                    case "delete":
                        if (!Globals.sqlite_api.DeleteProgramEntry(change.entry.Program_ID))
                        {
                            // handle the error
                        }
                        break;
                }
            }
            this.Close();
        }

        // get the program entry list and convert it into an observable collection of VersionListEntry
        private ObservableCollection<VersionListEntry> generateProgramList()
        {
            ObservableCollection<VersionListEntry> oc = new ObservableCollection<VersionListEntry>();
            List<ProgramEntry> list = Globals.sqlite_api.GetProgramEntryList(currentName);
            
            if (list == null) return oc;

            // convert list into observable collection
            foreach (ProgramEntry item in list) {
                VersionListEntry vle = new VersionListEntry(item);
                oc.Add(vle);
            }
            return oc;
        }

        private void EnableFieldsForEdit(bool editing)
        {
            // Enable edit fields
            versionText.IsEnabled = editing;
            osText.IsEnabled = editing;
            descriptionText.IsEnabled = editing;
            fileListBox.IsEnabled = editing;
            addFilesButton.IsEnabled = editing;
            deleteFilesButton.IsEnabled = editing;
            doneButton.IsEnabled = editing;

            // Disable other fields
            changeNameButton.IsEnabled = !editing;
            addButton.IsEnabled = !editing;
            editButton.IsEnabled = !editing;
            deleteVersionButton.IsEnabled = !editing;
            versionListBox.IsEnabled = !editing;
            saveButton.IsEnabled = !editing;
        }

        // update the selectedEntry when the entry selection is changed
        private void versionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                // set selectedEntry
                selectedEntry = entryList[versionListBox.SelectedIndex].entry;
        }

    }
}
