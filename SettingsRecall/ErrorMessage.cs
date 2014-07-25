using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SettingsRecall {
    class ErrorMessage : Border {
        public ErrorMessage(Panel panel) {
            windowContent = panel;

            CornerRadius = new CornerRadius(4);
            Margin = new Thickness(0, 5, 0, 0);
            BorderBrush = (Brush)converter.ConvertFromString("#ebccd1");
            Background = (Brush)converter.ConvertFromString("#f2dede");
            stackPanel = new StackPanel();
            Child = stackPanel;
        }

        public ErrorMessage(string bodyText, Panel panel) {
            windowContent = panel;

            CornerRadius = new CornerRadius(4);
            BorderBrush = (Brush)converter.ConvertFromString("#ebccd1");
            Background = (Brush)converter.ConvertFromString("#f2dede");
            stackPanel = new StackPanel();
            Child = stackPanel;

            AddErrorLabel(bodyText);
            ShowInWindow();
        }

        public void ShowInWindow() {
            if (!windowContent.Children.Contains(this))
                windowContent.Children.Add(this);
        }

        public void RemoveFromWindow() {
            windowContent.Children.Remove(this);
        }

        public void AddErrorLabel(string errorText) {
            foreach (Label label in stackPanel.Children) {
                if (label.Content.ToString() == errorText)
                    return;
            }

            Label errorLabel = new Label { Content = errorText, Foreground = textBrush };
            stackPanel.Children.Add(errorLabel);

            ShowInWindow();
        }

        public void ClearAllErrors() {
            stackPanel.Children.Clear();
        }

        private static BrushConverter converter = new BrushConverter();
        private Brush textBrush = (Brush)converter.ConvertFromString("#a94442");
        private StackPanel stackPanel;
        private Panel windowContent;
    }
}
