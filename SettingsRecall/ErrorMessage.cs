using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SettingsRecall {
    class ErrorMessage : Border {
        public ErrorMessage(string bodyText, Window parentWindow = null) {
            CornerRadius = new CornerRadius(4);
            BrushConverter converter = new BrushConverter();
            BorderBrush = (Brush)converter.ConvertFromString("#ebccd1");
            Background = (Brush)converter.ConvertFromString("#f2dede");
            Brush textBrush = (Brush)converter.ConvertFromString("#a94442");

            Label body = new Label { Content = bodyText, Foreground = textBrush };

            StackPanel stackPanel = new StackPanel { Children = { body } };
            Child = stackPanel;

            if (parentWindow != null)
                ShowInWindow(parentWindow);
        }

        public void ShowInWindow(Window window) {
            ((Panel)window.Content).Children.Add(this);
        }

        public void RemoveFromWindow(Window window) {
            ((Panel)window.Content).Children.Remove(this);
        }
    }
}
