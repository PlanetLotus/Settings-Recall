using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SettingsRecall {
    class AlertMessage : Border {
        public AlertMessage(Panel panel) {
            windowContent = panel;

            CornerRadius = new CornerRadius(4);
            Margin = new Thickness(0, 5, 0, 0);
            stackPanel = new StackPanel();
            Child = stackPanel;
        }

        public enum AlertLevel {
            Warn,
            Error
        }

        public AlertMessage(string bodyText, Panel panel) {
            windowContent = panel;

            CornerRadius = new CornerRadius(4);
            stackPanel = new StackPanel();
            Child = stackPanel;

            AddAlertLabel(bodyText);
            ShowInWindow();
        }

        public void ShowInWindow() {
            if (!windowContent.Children.Contains(this))
                windowContent.Children.Add(this);
        }

        public void RemoveFromWindow() {
            windowContent.Children.Remove(this);
        }

        public void AddAlertLabel(string alertText, AlertLevel alertLevel = AlertLevel.Error) {
            foreach (Label label in stackPanel.Children) {
                if (label.Content.ToString() == alertText)
                    return;
            }

            Brush labelBackgroundBrush = (Brush)converter.ConvertFromString("#f2dede");
            Brush labelForegroundBrush = (Brush)converter.ConvertFromString("#a94442");

            if (alertLevel == AlertLevel.Warn) {
                labelBackgroundBrush = (Brush)converter.ConvertFromString("#fcf8e3");
                labelForegroundBrush = (Brush)converter.ConvertFromString("#8a6d3b");
            }

            Label alertLabel = new Label { Content = alertText, Foreground = labelForegroundBrush, Background = labelBackgroundBrush };
            stackPanel.Children.Add(alertLabel);

            ShowInWindow();
        }

        public void ClearAllAlerts() {
            stackPanel.Children.Clear();
        }

        private static BrushConverter converter = new BrushConverter();
        private StackPanel stackPanel;
        private Panel windowContent;
    }
}
