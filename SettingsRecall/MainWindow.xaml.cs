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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SettingsRecall
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        // Displays the backup page when the user chooses to
        // from the startup window
        public void ShowBackupPage()
        {
            // instantiate the UserControl
            BackupPage backup_page = new BackupPage();
            this.Content = backup_page;
        }

        // Display restore page
        public void ShowRestorePage()
        {
            // instantiate the UserControl       
            RestorePage restore_page = new RestorePage();
            this.Content = restore_page;
        }


    }
}
