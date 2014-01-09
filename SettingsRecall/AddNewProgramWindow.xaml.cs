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

namespace SettingsRecall
{
    /// <summary>
    /// Interaction logic for AddNewProgramWindow.xaml
    /// </summary>
    public partial class AddNewProgramWindow : Window
    {
        private string programName { get; set; } // name entered

        public AddNewProgramWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Get the name entered into the form
        /// </summary>
        /// <returns></returns>
        public string GetProgramName() {
            return this.programName;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            // Make sure a name was entered
            if (String.IsNullOrEmpty(nameText.Text))
            {
                ErrorMessageBox nullErrorBox = new ErrorMessageBox("Please enter a name!");
                nullErrorBox.show();
                return;
            }

            // Make sure program name isn't already in the DB
            List<string> programList = new List<string>();
            programList = Globals.sqlite_api.GetProgramNameList();


            if (programList != null && programList.Contains(nameText.Text))
            {
                ErrorMessageBox errorBox = new ErrorMessageBox("This program already exists");
                errorBox.show();
                return;
            }

            // Everything is OK
            this.programName = nameText.Text;
            this.DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            // cancelled
            this.DialogResult = false;
        }

    }
}
