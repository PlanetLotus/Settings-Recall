using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SettingsRecall
{
    /// <summary>
    /// Contains a messagebox which is set up to display error messages.
    /// Constructor accepts 2 fields. Text: The text displayed and
    /// Caption: the caption on the window when show() is called in the
    /// ErrorMessageBox object.
    /// </summary>
    public class ErrorMessageBox
    {

        private string text;
        private string caption;
        private MessageBoxButton button = MessageBoxButton.OK;
        private MessageBoxImage icon = MessageBoxImage.Error;

        /// <summary>
        /// 1 parameter constructor for ErrorMessageBox
        /// Window caption is "Error" by default.
        /// </summary>
        /// <param name="t">Text displayed on the error box</param>
        public ErrorMessageBox(string t)
        {
            this.text = t;
            this.caption = "Error";
        }


        /// <summary>
        /// 2 parameter constructor for ErrorMessageBox
        /// </summary>
        /// <param name="t">Text displayed on the error box</param>
        /// <param name="c">Window caption</param>
        public ErrorMessageBox(string t, string c)
        {
            this.text = t;
            this.caption = c;
        }

        /// <summary>
        /// Show the error message box.
        /// </summary>
        public void show()
        {
            MessageBox.Show(text, caption, button, icon);
        }
    }
}
