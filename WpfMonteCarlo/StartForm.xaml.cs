﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfChordMethod
{
    /// <summary>
    /// Interaction logic for StartForm.xaml
    /// </summary>
    public partial class StartForm : Window
    {
        public string PIB;
        public StartForm()
        {
            InitializeComponent();
            PIB = " ";
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            PIB = tbPIB.Text;
            Close();
        }
    }
}
