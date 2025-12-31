using System;
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

namespace NumericControls
{
    /// <summary>
    /// Interaction logic for NumericEntry.xaml
    /// </summary>
    public partial class NumericEntry : Window
    {
        public NumericEntry()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            ValueTextBox.SelectAll();
        }
    }
}
