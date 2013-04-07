/*********************************************************************************
 * This is the code behind file for the ViewImage window
 * 
 * *******************************************************************************
 * Changelog:
 * 4/6/13 Ryan Causey: Trying to get this to databind to the image the user wishes to view.
 * 4/7/13 Ryan Causey: Databinding is working, including fallback values! I am the greatest!
 *                     Image now scales to window size.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ViewImage.xaml
    /// </summary>
    public partial class ViewImage : Window
    {
        private ReadOnlyObservableCollection<ComplexPhotoData> _picturesCollection;
        private ComplexPhotoData _currentPicture;
        //property for data binding
        public ComplexPhotoData currentPicture
        {
            get
            {
                return _currentPicture;
            }
        }

        public ViewImage(ReadOnlyObservableCollection<ComplexPhotoData> picturesCollectionFromAlbum, int imageUID)
        {
            _picturesCollection = picturesCollectionFromAlbum;
            _currentPicture = _picturesCollection.FirstOrDefault(photo => photo.UID == imageUID);
            InitializeComponent();
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        private void exitButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        private void DockPanel_MouseDown_1(object sender, MouseButtonEventArgs e)
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

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        private void maximizeToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            toggleWindowState();
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Minimized;
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


        /**************************************************************************************************
         * start region of thumb bar resize events
        **************************************************************************************************/
        /*
         *Created By: Alejandro Sosa
         *Last Edited By: Ryan Causey
         *Last Edited Date: 4/1/13
         *This handles the resizing via dragging from the bottom edge of the window, making sure the window height
         *does not go below a certain minimum
         */
        private void bottomThumb_DragDeltaHandler(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            //resize from bottom
            if (this.Height > this.ViewImageWindowGrid.MinHeight)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative height.
                if ((this.Height + e.VerticalChange) > this.ViewImageWindowGrid.MinHeight)
                {
                    this.Height += e.VerticalChange;
                }
            }
            else
            {
                this.Height = this.ViewImageWindowGrid.MinHeight + 1;
            }

        }

        /*
         *Created By: Alejandro Sosa
         *Last Edited By: Ryan Causey
         *Last Edited Date: 4/1/13
         *This handles the resizing via dragging from the top edge of the window, making sure the window height
         *does not go below a certain minimum
         */
        private void topThumb_DragDeltaHandler(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

            //resize from top
            if (this.Height > this.ViewImageWindowGrid.MinHeight)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative height.
                if ((this.Height - e.VerticalChange) > this.ViewImageWindowGrid.MinHeight)
                {
                    this.Height -= e.VerticalChange;
                    this.Top += e.VerticalChange;
                }
            }
            else
            {
                this.Height = MinHeight + 1;
                this.Top -= e.VerticalChange;
            }
        }

        /*
         *Created By: Alejandro Sosa
         *Last Edited By: Ryan Causey
         *Last Edited Date: 4/1/13
         *This handles the resizing via dragging from the right edge of the window, making sure the window width
         *does not go below a certain minimum
         */
        private void rightThumb_DragDeltaHandler(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            //resize from right
            if (this.Width > this.ViewImageWindowGrid.MinWidth)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative width.
                if ((this.Width + e.HorizontalChange) > this.ViewImageWindowGrid.MinWidth)
                {
                    this.Width += e.HorizontalChange;
                }
            }
            else
            {
                this.Width = this.ViewImageWindowGrid.MinWidth + 1;
            }
        }

        /*
         *Created By: Alejandro Sosa
         *Last Edited By: Ryan Causey
         *Last Edited Date: 4/1/13
         *This handles the resizing via dragging from the left edge of the window, making sure the window width
         *does not go below a certain minimum
         */
        private void leftThumb_DragDeltaHandler(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            //resize from Left
            if (this.Width > this.ViewImageWindowGrid.MinWidth)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative width.
                if ((this.Width - e.HorizontalChange) > this.ViewImageWindowGrid.MinWidth)
                {
                    this.Width -= e.HorizontalChange;
                    this.Left += e.HorizontalChange;
                }

            }
            else
            {
                this.Width = this.ViewImageWindowGrid.MinWidth + 1;
            }
        }

        /*
         *Created By: Alejandro Sosa
         *Last Edited By: Ryan Causey
         *Last Edited Date: 4/1/13
         *This handles the resizing via dragging from the bottom right corner of the window, making sure the window height
         *and width do not go below a certain minimum
         */
        private void bottomRightThumb_DragDeltaHandler(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            //resize from bottom
            if (this.Height > this.ViewImageWindowGrid.MinHeight)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative height.
                if ((this.Height + e.VerticalChange) > this.ViewImageWindowGrid.MinHeight)
                {
                    this.Height += e.VerticalChange;
                }
            }
            else
            {
                this.Height = this.ViewImageWindowGrid.MinHeight + 1;
            }

            //resize from right
            if (this.Width > this.ViewImageWindowGrid.MinWidth)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative width.
                if ((this.Width + e.HorizontalChange) > this.ViewImageWindowGrid.MinWidth)
                {
                    this.Width += e.HorizontalChange;
                }
            }
            else
            {
                this.Width = this.ViewImageWindowGrid.MinWidth + 1;
            }
        }

        /*
         *Created By: Alejandro Sosa
         *Last Edited By: Ryan Causey
         *Last Edited Date: 4/1/13
         *This handles the resizing via dragging from the top right corner of the window, making sure the window height
         *and width do not go below a certain minimum
         */
        private void topRightThumb_DragDeltaHandler(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            //resize from top
            if (this.Height > this.ViewImageWindowGrid.MinHeight)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative height.
                if ((this.Height - e.VerticalChange) > this.ViewImageWindowGrid.MinHeight)
                {
                    this.Height -= e.VerticalChange;
                    this.Top += e.VerticalChange;
                }

            }
            else
            {
                this.Height = this.ViewImageWindowGrid.MinHeight + 1;
            }

            //resize from right
            if (this.Width > this.ViewImageWindowGrid.MinWidth)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative width.
                if ((this.Width + e.HorizontalChange) > this.ViewImageWindowGrid.MinWidth)
                {
                    this.Width += e.HorizontalChange;
                }
            }
            else
            {
                this.Width = this.ViewImageWindowGrid.MinWidth + 1;
            }
        }

        /*
         *Created By: Alejandro Sosa
         *Last Edited By: Ryan Causey
         *Last Edited Date: 4/1/13
         *This handles the resizing via dragging from the bottom left corner of the window, making sure the window height
         *and width do not go below a certain minimum
         */
        private void bottomLeftThumb_DragDeltaHandler(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            //resize from bottom
            if (this.Height > this.ViewImageWindowGrid.MinHeight)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative height.
                if ((this.Height + e.VerticalChange) > this.ViewImageWindowGrid.MinHeight)
                {
                    this.Height += e.VerticalChange;
                }
            }
            else
            {
                this.Height = MinHeight + 1;
            }

            //resize from left
            if (this.Width > this.ViewImageWindowGrid.MinWidth)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative width.
                if ((this.Width - e.HorizontalChange) > this.ViewImageWindowGrid.MinWidth)
                {
                    this.Width -= e.HorizontalChange;
                    this.Left += e.HorizontalChange;
                }
            }
            else
            {
                this.Width = this.ViewImageWindowGrid.MinWidth + 1;
            }
        }

        private void nextDockButton_Click(object sender, RoutedEventArgs e)
        {
            //need to call the next function here.
        }


    }
}
