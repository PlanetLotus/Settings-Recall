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

        public ChooseEditProgramWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Populates the program ListBox with a list of programs available to edit.
        /// </summary>
        private void PopulateList()
        {
            programList = Globals.sqlite_api.GetProgramNameList("String");
            ObservableCollection<string> observableList = new ObservableCollection<string>(programList);
            programListBox.ItemsSource = observableList;
        }

    }
}
