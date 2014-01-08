﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SettingsRecall
{
    /// <summary>
    /// Interaction logic for EditProgramWindow.xaml
    /// </summary>
    public partial class EditProgramWindow : Window
    {

        // Window global variables
        private string currentName;
        private ProgramEntry selectedEntry;
        
        public EditProgramWindow(string name)
        {
            InitializeComponent();
            this.currentName = name;
        }
    }
}
