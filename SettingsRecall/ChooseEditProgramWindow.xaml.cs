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
using System.Windows.Shapes;
using System.Data;
using System.Collections.ObjectModel;

namespace SettingsRecall
{
    /// <summary>
    /// Interaction logic for ChooseEditProgramWindow.xaml
    /// </summary>
    public partial class ChooseEditProgramWindow : Window
    {
        private List<string> programList;
        private string selectedName { get; set; }

        public ChooseEditProgramWindow()
        {
            InitializeComponent();
            PopulateList();
        }

        /// <summary>
        /// Populates the program ListBox with a list of programs available to edit.
        /// </summary>
        private void PopulateList()
        {
            programList = Globals.sqlite_api.GetProgramNameList();
            ObservableCollection<string> observableList = new ObservableCollection<string>(programList);
            programListBox.ItemsSource = observableList;
        }

        private void programListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedName = programListBox.SelectedValue.ToString();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (programListBox.SelectedIndex != -1) {
                this.DialogResult = true;
                Close();
            }

            // nothing selected
            ErrorMessageBox noSelectErrorBox = new ErrorMessageBox("Please choose a program to edit.");
            noSelectErrorBox.show();
            return;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public string GetProgramName()
        {
            return this.selectedName;
        }



    }
}
