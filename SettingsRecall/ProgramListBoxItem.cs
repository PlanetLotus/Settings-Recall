using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

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

        protected void NotifyPropertyChanged(string strPropertyName) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(strPropertyName));
        }

        private string name;
        private bool isChecked;
        private bool isSupported;
    }
}
