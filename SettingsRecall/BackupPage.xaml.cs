﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
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
        List<ProgramEntry> supportedPrograms;
        List<string> supportedProgramNames;
        List<ProgramEntry> selectedPrograms;

        public BackupPage()
        {
            InitializeComponent();

            supportedPrograms = new List<ProgramEntry>();
            supportedProgramNames = new List<string>();
            selectedPrograms = new List<ProgramEntry>();

            // Initialize the left list with the names of the supported programs whose paths exist on the machine
            // Not using ItemsSource here because we don't want the list to be read-only
            GetUserPrograms();
            foreach (string name in supportedProgramNames)
                backupPageLeftList.Items.Add(name);
        }

        private void GetUserPrograms() {
            // Get supported programs
            List<ProgramEntry> programEntries = Globals.sqlite_api.GetProgramEntryList();

            // Get OS of machine we're on
            string this_os = GetOSFriendlyName();

            // Filter list down to programs where at least one path for that program exists on the machine
            // Filter program entries by:
            // - User's operating system
            // - At least one path in the entry exists on user's computer
            foreach (ProgramEntry entry in programEntries) {
                Console.WriteLine(entry.Name);
                if (entry.OS == this_os) {
                    foreach (string path in entry.Paths) {
                        if (File.Exists(path) || Directory.Exists(path)) {
                            supportedPrograms.Add(entry);
                            supportedProgramNames.Add(entry.Name);
                            break;
                        }
                    }
                }
            }
        }
        
        public static string GetOSFriendlyName() {
            string result = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
            foreach (ManagementObject os in searcher.Get())
            {
                result = os["Caption"].ToString();
                break;
            }

            // Convert result to either XP, Vista, 7, or 8 for our standards
            if (result.Contains("XP"))
                result = "Windows XP";
            else if (result.Contains("Vista"))
                result = "Windows Vista";
            else if (result.Contains("Windows 7"))
                result = "Windows 7";
            else if (result.Contains("Windows 8"))
                result = "Windows 8";
            else {
                Console.WriteLine("Unexpected OS name: " + result);
                return null;
            }

            // Append 32-bit or 64-bit
            if (Environment.Is64BitOperatingSystem == true)
                result += " 64-bit";
            else
                result += " 32-bit";

            return result;
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
            AddNewProgramWindow ANPWindow = new AddNewProgramWindow();

            // configure dialog box and open modally
            ANPWindow.Owner = App.mainWindow;
            ANPWindow.ShowDialog();

            if (ANPWindow.DialogResult == false)
            {
                // do not continue
                return;
            }

            // get the name entered into the add dialog
            string programName = ANPWindow.GetProgramName();

            // instantiate edit window
            EditProgramWindow editWindow = new EditProgramWindow(programName);

            // open the edit window dialog
            editWindow.Owner = App.mainWindow;
            editWindow.ShowDialog();
        }

        private void editProgramButton_Click(object sender, RoutedEventArgs e)
        {
            // instantiate 'choose program' dialog box
            ChooseEditProgramWindow chooseWindow = new ChooseEditProgramWindow();

            // configure dialog box and open modally
            chooseWindow.Owner = App.mainWindow;
            chooseWindow.ShowDialog();

            if (chooseWindow.DialogResult == false)
            {
                // do not continue
                return;
            }

            // get the name entered into the add dialog
            string programName = chooseWindow.GetProgramName();

            // instantiate edit window
            EditProgramWindow editWindow = new EditProgramWindow(programName);

            // open the edit window dialog
            editWindow.Owner = App.mainWindow;
            editWindow.ShowDialog();
        }

        private void chooseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            SetLoadSaveLocation();
            // display directory in label
            folder_label.Content = Globals.load_save_location;
        }

        private void addToBackupButton_Click(object sender, RoutedEventArgs e) {
            // Remove from left list, add to right list
            object selected = backupPageLeftList.SelectedItem;
            backupPageRightList.Items.Add(selected);
            backupPageLeftList.Items.Remove(selected);
        }

        private void removeFromBackupButton_Click(object sender, RoutedEventArgs e) {
            // Remove from right list, add to left list
            object selected = backupPageRightList.SelectedItem;
            backupPageLeftList.Items.Add(selected);
            backupPageRightList.Items.Remove(selected);
        }

        private void createBackupButton_Click(object sender, RoutedEventArgs e) {
            selectedPrograms.Clear();

            foreach (ProgramEntry program in supportedPrograms) {
                if (backupPageRightList.Items.Contains(program.Name))
                    selectedPrograms.Add(program);
            }
        }
    }
}