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

        }

        private void deleteVersionButton_Click(object sender, RoutedEventArgs e)
        {

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
    }
}
