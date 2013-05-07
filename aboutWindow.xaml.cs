/********************************************************************************
 * About window for PhotoBomber program.
 * ******************************************************************************
 * Changelog:
 * 4/8/13 Ryan Causey: Added window deactivated event handler
 *******************************************************************************/
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
    public partial class AboutWindow : Window
    {
        


        public AboutWindow()
        {
            InitializeComponent();
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        private void exitButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception)
            {
                ;
            }
            
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        private void okButton_click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception)
            {
                ;
            }
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        private void DockPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
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

        /*
         * Created By: Ryan Causey
         * Created Date: 4/8/13
         * Last Edited By: 
         * Last Edited Date:
         */
        /// <summary>
        /// Handler for about window lost focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PhotoBomberAboutWindow_Deactivated(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception)
            {
                ;
            }
        }

        private void closingWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //throw new Exception();
        }

    }
}
