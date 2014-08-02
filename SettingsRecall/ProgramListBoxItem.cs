using System.ComponentModel;
using System.Windows;

namespace SettingsRecall {
    public class ProgramListBoxItem : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name {
            get { return name; }
            set {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public bool IsChecked {
            get { return isChecked; }
            set {
                isChecked = value;
                NotifyPropertyChanged("IsChecked");
            }
        }

        public bool IsSupported {
            get { return isSupported; }
            set {
                isSupported = value;
                NotifyPropertyChanged("IsSupported");
            }
        }

        public Visibility Visibility {
            get { return visibility; }
            set {
                visibility = value;
                NotifyPropertyChanged("Visibility");
            }
        }

        protected void NotifyPropertyChanged(string strPropertyName) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(strPropertyName));
        }

        private string name;
        private bool isChecked;
        private bool isSupported;
        private Visibility visibility;
    }
}
