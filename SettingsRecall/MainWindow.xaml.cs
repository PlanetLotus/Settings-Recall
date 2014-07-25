using System.Windows;

namespace SettingsRecall {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void BackupButton_Click(object sender, RoutedEventArgs e) {
            BackupPage backup_page = new BackupPage();
            this.Content = backup_page;
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e) {
            RestorePage restore_page = new RestorePage();
            this.Content = restore_page;
        }
    }
}
