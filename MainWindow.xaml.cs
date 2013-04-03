/*********************************************************************************
 * This is the code-behind file for the main program window
 * 
 * Changelog:
 * 4/1/13 Ryan Causey: Added checks for the DragDeltaHandlers to fix a error case
 *                     where the width/height value for the window could become
 *                     negative.
 */ 
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

namespace SoftwareEng
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary> 
    public partial class MainWindow : Window
    {
        //DATABINDING SOURCE 
        ReadOnlyObservableCollection<SimpleAlbumData> listOfAlbums;

        //--didn't know what to call it, so I named it the literal spanish translation
        public SoftwareEng.PhotoBomb bombaDeFotos;

        //--stores the albumImageList index of the default image for albums
        private const short defaultAlbumImageListIndex = 0;

        //--used to specify the UID of the "add new album" icon
        private const int addAlbumID = 0;

        //--A more stable storage for the ID of the user album instead
        //-- of relying on a form's selected items collection
        //private int albumChosenbyUser;

        public MainWindow()
        {
            InitializeComponent();

            //for now the gui will determine filepaths(set to same folder as exe) in case it is ever made a user choice
            String libraryPath = System.IO.Path.Combine(Environment.CurrentDirectory, "photo library");
            bombaDeFotos = new PhotoBomb();
            bombaDeFotos.init(guiConstructorCallback, "albumRC1.xml", "photoRC1.xml", libraryPath);

            hideAddAlbumBox();

            

            populateAlbumView(true);
        }


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: an object of ErrorReport which will be used to check if backend was successful
        * return type: void
        * purpose: simply calls back end
        *********************************************************************************************/
        private void guiConstructorCallback(ErrorReport status)
        {
            if (status.reportID != ErrorReport.SUCCESS)
            {
                bombaDeFotos.rebuildBackendOnFilesystem(new generic_callback(guiGenericErrorFunction));
            }
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: an object of ErrorReport which will be used to check if backend was successful
        * return type: void
        * purpose: says generic error, but is used for blowing up the backend and restarting 
        *********************************************************************************************/
        private void guiGenericErrorFunction(ErrorReport status)
        {
            if (status.reportID != ErrorReport.SUCCESS)
            {
                if (Directory.Exists("photo library_backup"))
                {
                    Directory.Delete("photo library_backup", true);
                    bombaDeFotos.rebuildBackendOnFilesystem(new generic_callback(guiGenericErrorFunction));
                }
            }
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: bool that determines whether list is cleared or repopulated
        * return type: void
        * purpose: refreshes list of albums
        *********************************************************************************************/
        private void populateAlbumView(bool refreshView)
        {

            /*
            ListViewItem.ListViewSubItem[] itemHolderSubitems;
            ListViewItem itemHolder = null;

            albumListView.Items.Clear();

            //--code from here to the if statement is to regenerate the 
            //--the "add new album icon" as it's not an album
            itemHolderSubitems = new ListViewItem.ListViewSubItem[]{
                new ListViewItem.ListViewSubItem(itemHolder, "Add New Album"),
                new ListViewItem.ListViewSubItem(itemHolder, addAlbumID.ToString() )
                };

            itemHolder = new ListViewItem(itemHolderSubitems, defaultAlbumImageListIndex);
            albumListView.Items.Add(itemHolder);

            */



            if (refreshView == true)
            {
                bombaDeFotos.getAllAlbums(new getAllAlbumNames_callback(guiAlbumsRetrieved));
            }


            
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: an object of ErrorReport which will be used to check if backend was successful,
        *   and a list <type is SimpleAlbumData> containing data to identify all albums requested
        * return type: void
        * purpose: list of albums returned from the backend
        *********************************************************************************************/
        public void guiAlbumsRetrieved(ErrorReport status, ReadOnlyObservableCollection<SimpleAlbumData> albumsRetrieved)
        {
            if (status.reportID == ErrorReport.SUCCESS)
            {
                listOfAlbums = albumsRetrieved;

                mainWindowAlbumList.ItemsSource = listOfAlbums;

                //mainWindowAlbumList.listItemsControl.DataContext = listOfAlbums;



                

            }
            else
            {
                //show an Error
            }



                /*
                //--stops the drawing of albumListView
                albumListView.BeginUpdate();

                ListViewItem itemHolder = null;
                ListViewItem.ListViewSubItem[] itemHolderSubitems;

                foreach (SimpleAlbumData singleAlbum in albumsRetrieved)
                {
                    try
                    {
                        itemHolderSubitems = new ListViewItem.ListViewSubItem[]{
                            new ListViewItem.ListViewSubItem(itemHolder, singleAlbum.albumName),
                            new ListViewItem.ListViewSubItem(itemHolder, singleAlbum.UID.ToString() )
                            };

                        itemHolder = new ListViewItem(itemHolderSubitems, defaultAlbumImageListIndex);
                        albumListView.Items.Add(itemHolder);
                    }
                    catch (Exception)
                    {
                        showError("Error: Album Missing");
                    }
                }

                //--resumes drawing of albumListView
                albumListView.EndUpdate();
            }
            else if (status.reportID == ErrorReport.FAILURE)
            {
                showError("Error: Album Missing");
            }
            */
        }


        private void mainWindowDock_MouseLeave(object sender, MouseEventArgs e)
        {
            mainWindowDock.Height = 1;
        }

        private void mainWindowContextMenu_LostMouseCapture(object sender, MouseEventArgs e)
        {
            libraryContextMenu.IsOpen = false;
        }

        private void dockHitBox_MouseEnter(object sender, MouseEventArgs e)
        {
            mainWindowDock.Height = Double.NaN;
        }

        private void exitDockButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void mainWindowAlbumList_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            libraryContextMenu.IsOpen = true;
        }

        private void addMenuItemLibraryButton_Click(object sender, RoutedEventArgs e)
        {
            showAddAlbumBox();
            
            SoftwareEng.ViewImage imageViewer = new ViewImage();

            imageViewer.Show();

            
        }

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

        private void maximizeToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            toggleWindowState();
        }






        private void toggleWindowState()
        {
            if(this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void showAddAlbumBox()
        {
            generalPurposeLabel.Visibility = Visibility.Visible;
            generalPurposeTextBox.Visibility = Visibility.Visible;

            acceptAddToolbarButton.Visibility = Visibility.Visible;
            cancelAddToolbarButton.Visibility = Visibility.Visible;
        }

        private void hideAddAlbumBox()
        {
            generalPurposeLabel.Visibility= Visibility.Hidden;
            generalPurposeTextBox.Visibility = Visibility.Hidden;

            acceptAddToolbarButton.Visibility = Visibility.Hidden;
            cancelAddToolbarButton.Visibility = Visibility.Hidden;
        }

        private void cancelAddToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            hideAddAlbumBox();
        }

        private void mainWindowAlbumList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void VirtualizingStackPanel_MouseRightButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            libraryContextMenu.IsOpen = true;
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
            if (this.Height > this.MinHeight)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative height.
                if ((this.Height + e.VerticalChange) > this.MinHeight)
                {
                    this.Height += e.VerticalChange;
                }
            }
            else
            {
                this.Height = MinHeight + 1;
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
            if (this.Height > this.MinHeight)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative height.
                if ((this.Height - e.VerticalChange) > this.MinHeight)
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
            if (this.Width > this.MinWidth )
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative width.
                if ((this.Width + e.HorizontalChange) > this.MinWidth)
                {
                    this.Width += e.HorizontalChange;
                }
            }
            else
            {
                this.Width = MinWidth + 1;
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
            if (this.Width > this.MinWidth)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative width.
                if ((this.Width - e.HorizontalChange) > this.MinWidth)
                {
                    this.Width -= e.HorizontalChange;
                    this.Left += e.HorizontalChange;
                }

            }
            else
            {
                this.Width = MinWidth + 1;
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
            if (this.Height > this.MinHeight)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative height.
                if ((this.Height + e.VerticalChange) > this.MinHeight)
                {
                    this.Height += e.VerticalChange;
                }
            }
            else
            {
                this.Height = MinHeight + 1;
            }

            //resize from right
            if (this.Width > this.MinWidth)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative width.
                if ((this.Width + e.HorizontalChange) > this.MinWidth)
                {
                    this.Width += e.HorizontalChange;
                }
            }
            else
            {
                this.Width = MinWidth + 1;
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
            if (this.Height > this.MinHeight)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative height.
                if ((this.Height - e.VerticalChange) > this.MinHeight)
                {
                    this.Height -= e.VerticalChange;
                    this.Top += e.VerticalChange;
                }

            }
            else
            {
                this.Height = MinHeight + 1;
            }

            //resize from right
            if (this.Width > this.MinWidth)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative width.
                if ((this.Width + e.HorizontalChange) > this.MinWidth)
                {
                    this.Width += e.HorizontalChange;
                }
            }
            else
            {
                this.Width = MinWidth + 1;
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
            if (this.Height > this.MinHeight)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative height.
                if ((this.Height + e.VerticalChange) > this.MinHeight)
                {
                    this.Height += e.VerticalChange;
                }
            }
            else
            {
                this.Height = MinHeight + 1;
            }

            //resize from left
            if (this.Width > this.MinWidth)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative width.
                if ((this.Width - e.HorizontalChange) > this.MinWidth)
                {
                    this.Width -= e.HorizontalChange;
                    this.Left += e.HorizontalChange;
                }
            }
            else
            {
                this.Width = MinWidth + 1;
            }
        }

        private void aboutButtonPressed_eventHandler(object sender, RoutedEventArgs e)
        {
            aboutWindow someWindow = new aboutWindow();

            someWindow.ShowDialog();
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Minimized;
            }
        }

        /********************************************************************
         * TEST FUNCTION SECTION
         * Author: Ryan Causey
         * I am using the following functions to test backend functionality
         * Destroy these when done testing
         *******************************************************************/
        private void testEvent(object sender, RoutedEventArgs e)
        {
            SimpleAlbumData testData = new SimpleAlbumData();
            testData.albumName = "LOOKITDISNAME";
            bombaDeFotos.addNewAlbum(new generic_callback(dummyCallback), testData);
        }

        public void dummyCallback(ErrorReport er)
        {
        }
        /*******************************************************************
         * End Test Functions
         ******************************************************************/
        
        /**************************************************************************************************
         * end region of thumb bar resize events
        **************************************************************************************************/
    }
}
