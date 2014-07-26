using System.Windows;

namespace SettingsRecall {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void BackupButton_Click(object sender, RoutedEventArgs e) {
            this.Content = new BackupPage();
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e) {
            this.Content = new RestorePage();
        }
    }
}
