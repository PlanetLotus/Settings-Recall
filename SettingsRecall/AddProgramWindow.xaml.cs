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
    /// Interaction logic for AddProgramWindow.xaml
    /// </summary>
    public partial class AddProgramWindow : Window
    {
        private List<string> PathList = new List<string>();
        

        public AddProgramWindow()
        {
            InitializeComponent();
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
                    System.Diagnostics.Debug.WriteLine(path);
                }
            }
            PathListBox.ItemsSource = PathList;
        }




    }
}
