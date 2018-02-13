using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Globalization;

namespace WpfChordMethod
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }
        
        string PIB;
        Double[] koeff = new Double[3];
        Double[] limits = new Double[3];
        Double integral;
        int FunkType = 2;
        bool isChange;

        double f(double a, double b, double c, int FunkType)
        {
            if (FunkType == 2) return Math.Pow(a, 3) + Math.Pow(b, 2) + c;
            if (FunkType == 0) return a * Math.Sin(b) + c;
            if (FunkType == 1) return a * Math.Log(b) + c;
            return 0;
        }
        double fi(double a, double b, double c, int FunkType)
        {
            if (FunkType == 2) return Math.Pow(a, 3) + Math.Pow(b, 2) + c;
            if (FunkType == 0) return a * Math.Sin(b) + c;
            if (FunkType == 1) return a * Math.Log(b) + c; 
            return 0;
        }
        // n - кількість випробовувань
        double montecarlo(int n)
        {
            int i;
            double sum = 0;
            double S = 0;
            Random random = new Random();
            for (i = 0; i < n; i++)
            {
                koeff[0] = limits[0] * random.Next() / Int32.MaxValue;//генерируем x в интервале
                koeff[1] = limits[1] * random.Next() / Int32.MaxValue;//генерируем y в интервале
                koeff[2] = limits[2] * random.Next() / Int32.MaxValue;//генерируем z в интервале

                if (fi(koeff[0], koeff[1], koeff[2], FunkType) <= 1)
                  {
                      sum += f(koeff[0], koeff[1], koeff[2], FunkType);
                  }
               }
            S = (((limits[0] - 0) * (limits[1] - 0) * (limits[2] - 0)) / n) * sum;
            return S; 
        }

        public void OpenData(string filename)
        {
            StreamReader sr = new StreamReader(@filename);
            if (sr != null)
            {
                PIB = sr.ReadLine();
                string buff = sr.ReadLine();
                string[] split = buff.Split('\t');
                for (Int16 j = 0; j < 3; j++)
                    if (!Double.TryParse(split[j], out koeff[j]))
                        throw new Exception("Невірний формат файлу даних!!!");
                textA.Text = Convert.ToString(koeff[0]);
                textB.Text = Convert.ToString(koeff[1]);
                textC.Text = Convert.ToString(koeff[2]);
                
                sr.Close();
            }
        }
        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opendata = new OpenFileDialog();
            opendata.Filter = "Файли даних|*.dat";
            if (opendata.ShowDialog() == true)
            {
                OpenData(opendata.FileName);
                isChange = false;
                AddListBox();
            }      
        }

        public void SaveData(string filename)
        {
            StreamWriter sr = new StreamWriter(@filename);
            if (sr != null)
            {
                sr.WriteLine(PIB);
                string buff = "";
                foreach (double i in koeff)
                {
                    buff += Convert.ToString(i);
                    buff += "\t";
                }
                sr.WriteLine(buff);
                sr.WriteLine(integral);
                sr.Close();
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ReadKoeff();
            SaveFileDialog savedata = new SaveFileDialog();
            savedata.Filter = "Файли даних|*.dat";
            if (savedata.ShowDialog() == true)
            {
                SaveData(savedata.FileName);
                isChange = false;
            }
        }

        private void AddListBox()
        {
            listBoxResult.Items.Clear();
            listBoxResult.Items.Add("Студент " + PIB);
            if (FunkType == 2) listBoxResult.Items.Add("Функція " + koeff[0] + "^3+" + koeff[1] + "^2+" + koeff[2]);
            if (FunkType == 1) listBoxResult.Items.Add("Функція " + koeff[0] + "*Ln(" + koeff[1] + ")+" + koeff[2]);
            if (FunkType == 0) listBoxResult.Items.Add("Функція " + koeff[0] + "*Sin(" + koeff[1] + ")+" + koeff[2]);
            listBoxResult.Items.Add("Інтеграл = " + integral);
        }
        private void ReadKoeff() 
        {
            koeff[0] = Convert.ToDouble(textA.Text);
            koeff[1] = Convert.ToDouble(textB.Text);
            koeff[2] = Convert.ToDouble(textC.Text);
            limits[0] = Convert.ToDouble(textX.Text);
            limits[1] = Convert.ToDouble(textY.Text);
            limits[2] = Convert.ToDouble(textZ.Text);
        }

        private void btnCalc_Click(object sender, RoutedEventArgs e)
        {
            isChange = true;
            ReadKoeff();
            integral = montecarlo(100);
            AddListBox();
        }
 
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void textA_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string pattern;
            NumberFormatInfo nfi = NumberFormatInfo.CurrentInfo;
            pattern = @"^[0-9]$";  
            Regex inputRegex = new Regex(pattern);
            Match match = inputRegex.Match(e.Text);
            if (!match.Success)
            {
                TextBox tb = (TextBox)sender;
                pattern = @"^\" + nfi.NumberDecimalSeparator + "$";
                Regex inputRegex_ = new Regex(pattern);
                Match match_ = inputRegex_.Match(e.Text);
                if (!match_.Success || (tb.Text.IndexOf(nfi.NumberDecimalSeparator) != -1))
                e.Handled = true;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isChange)
                if (MessageBox.Show("Зберегти зміни ?", "Увага", MessageBoxButton.YesNo, MessageBoxImage.Question)
                    == MessageBoxResult.Yes) btnSave_Click(sender, null);
            if (MessageBox.Show("Ви дійсно бажаєте вийти із програми ?", "Увага", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.No)
                e.Cancel = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartForm f = new StartForm();
            f.ShowDialog();
            PIB = f.PIB;
            isChange = false;
        }

        private void btnFunk_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void cbFunk_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FunkType = cbFunk.SelectedIndex;
        }     
    }
}
