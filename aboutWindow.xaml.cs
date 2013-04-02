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
    /// Interaction logic for aboutWindow.xaml
    /// </summary>
    public partial class aboutWindow : Window
    {
        public aboutWindow()
        {
            InitializeComponent();
        }

        private void exitButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void okButton_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DockPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount == 2)
                {
                    toggleWindowState();
                }
                else
                {
                    this.DragMove();
                }
            }
        }

        private void toggleWindowState()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }
    }
}
