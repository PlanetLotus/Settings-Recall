using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SettingsRecall
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MainWindow mainWindow;

        void App_Startup(object sender, StartupEventArgs e)
        {
            // instantiate and show the main window
            mainWindow = new MainWindow();
            mainWindow.Show();

            // instantiate the startup window
            StartupWindow start_window = new StartupWindow();

            start_window.Owner = mainWindow;

            // open window modally
            start_window.ShowDialog();

            // load the correct page in the main window
            if (start_window.main_page == "backup")
            {
                mainWindow.ShowBackupPage();
            }
            else if (start_window.main_page == "restore")
            {
                mainWindow.ShowRestorePage();
            }
        }
    }
}
