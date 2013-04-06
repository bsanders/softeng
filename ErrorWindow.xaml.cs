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
using System.Windows.Shapes;

namespace SoftwareEng
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        public ErrorWindow(String errorMessage)
        {
            InitializeComponent();

            errorMessageTextBox.Text = errorMessage;
        }

        private void exitButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
