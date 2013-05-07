/*
 * Chage Log:
 * Alejandro Sosa(?/?/13)
 * This class was made.
 */
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
    /// By Alejandro Sosa
    /// Edited Julian Nguyen(5/7/13)
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        /// By Alejandro Sosa
        /// Edited Julian Nguyen(5/7/13)
        /// <summary>
        /// Class default constructor. 
        /// </summary>
        /// <param name="errorMessage"></param>
        public ErrorWindow(String errorMessage)
        {
            InitializeComponent();
            errorMessageTextBox.Text = errorMessage;
        }

        /// By Alejandro Sosa
        /// Edited Julian Nguyen(5/7/13)
        /// <summary>
        /// Will close the error windows.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
