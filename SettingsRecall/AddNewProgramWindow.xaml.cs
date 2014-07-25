using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SettingsRecall {
    public partial class AddNewProgramWindow : Window {
        public AddNewProgramWindow() {
            InitializeComponent();

            errorMessage = new ErrorMessage((Panel)this.Content);

            // Give window focus to the first control in tab order
            Loaded += (sender, e) =>
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        public string GetProgramName() {
            return programName;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e) {
            // Clear any previous errors
            errorMessage.ClearAllErrors();

            // Make sure a name was entered
            if (string.IsNullOrEmpty(nameText.Text)) {
                errorMessage.AddErrorLabel("Please enter a name.");
                return;
            }

            // Make sure program name isn't already in the DB
            List<string> programList = SQLiteAPI.GetProgramNameList();

            if (programList.Contains(nameText.Text)) {
                errorMessage.AddErrorLabel("This program already exists.");
                return;
            }

            programName = nameText.Text;
            DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
        }

        private void nameText_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter)
                OKButton_Click(OKButton, null);
        }

        private ErrorMessage errorMessage;
        private string programName { get; set; }
    }
}
