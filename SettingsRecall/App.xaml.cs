using System.Windows;

namespace SettingsRecall {
    public partial class App : Application {
        public static MainWindow mainWindow;

        private void AppStartup(object sender, StartupEventArgs e) {
            // instantiate and show the main window
            mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
