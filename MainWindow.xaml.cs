/*********************************************************************************
 * This is the code-behind file for the main program window
 * 
 * Changelog:
 * 4/1/13 Ryan Causey: Added checks for the DragDeltaHandlers to fix a error case
 *                     where the width/height value for the window could become
 *                     negative.
 * 4/3/13 Ryan Causey: Implementing validation of album names.
 */ 
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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

        //The regex for validation of album names
        private String albumValidationRegex = @"^[\w\d][\w\d ]{0,31}$"; //must be at least 1 character, max 32 in length

        //--A more stable storage for the ID of the user album instead
        //-- of relying on a form's selected items collection
        //private int albumChosenbyUser;

        public MainWindow()
        {
            InitializeComponent();

            // Set the library path to be in the User's application data folder under "PhotoBomber Studios" for now.
            String libraryPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Properties.Settings.Default.OrgName, 
                Properties.Settings.Default.PhotoLibraryName);

            bombaDeFotos = new PhotoBomb();
            bombaDeFotos.init(guiConstructorCallback,
                Properties.Settings.Default.AlbumXMLFile,
                Properties.Settings.Default.PhotoXMLFile,
                libraryPath);

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

        /*
         * Author: Ryan Causey
         * Created: 4/3/13
         * Function to validate a string against the given regex.
         * @Return: True = the string is valid, False = the string is not valid
         * @Params: Regex = the regular expression by which to evaluate the string
         * stringToValidate = the string which we will validate against the regex
         * Last Edited By:
         * Last Edited Date:
         */
        private bool validateTheString(String regex, String stringToValidate)
        {
            RegexStringValidator validator = new RegexStringValidator(regex);

            try
            {
                validator.Validate(stringToValidate);
            }
            catch (ArgumentException argException)
            {
                return false;
            }

            return true;
        }

        /*
         * Author: Ryan Causey
         * Created on: 4/3/13
         * Function for validating that a new album name is valid and unique.
         * @Params: validationRegex = regex to be used to validate the album name
         * albumName = the desired album name
         * Last Edited By:
         * Last Edited Date:
         */
        private void guiValidateAlbumName(String validationRegex)
        {
            generalPurposeTextBox.Text.Trim();

            if (validateTheString(validationRegex, generalPurposeTextBox.Text))
            {
                //check to see if the album name is unique in the program
                bombaDeFotos.checkIfAlbumNameIsUnique(new generic_callback(guiValidateAlbumName_Callback), generalPurposeTextBox.Text);
            }
            else
            {
                //apply error template to the text box.
                MessageBox.Show("This is a temporary error check message box failed at guiValidateAlbumName");//temporary as fuuu
                //focus the text box and select all the text
                generalPurposeTextBox.Focus();
                generalPurposeTextBox.SelectAll();
            }
        }

        /*
         * Author: Ryan Causey
         * Created on: 4/3/13
         * Callback for checking uniqueness of a new album name. This will be called after the back end finishes checking if the album
         * name is unique
         * Last Edited By:
         * Last Edited Date:
         */
        public void guiValidateAlbumName_Callback(ErrorReport error)
        {
            //if the album name was not unique
            if (error.reportID == ErrorReport.FAILURE || error.reportID == ErrorReport.SUCCESS_WITH_WARNINGS)
            {
                //apply error template to the text box
                MessageBox.Show("This is a temporary error check message box. Failed at guiValidateAlbumName_Callback");//temporary as fuuuu
                //focus the text box and select all the text
                generalPurposeTextBox.Focus();
                generalPurposeTextBox.SelectAll();
            }
            //it was unique, great success!
            else
            {
                guiCreateNewAlbum(generalPurposeTextBox.Text);
                hideAddAlbumBox();
                generalPurposeTextBox.Clear();
            }
        }

        /*
         * Author: Ryan Causey
         * Created on: 4/3/13
         * Function for creating a new album name. Only to be called after the album name is validated.
         * @Param: albumName = the name for the new album being created
         * Last Edited By:
         * Last Edited Date:
         */
        private void guiCreateNewAlbum(String albumName)
        {
            SimpleAlbumData newAlbum = new SimpleAlbumData();

            newAlbum.albumName = albumName;

            bombaDeFotos.addNewAlbum(new generic_callback(guiCreateNewAlbum_Callback), newAlbum);
        }

        /*
         * Author: Ryan Causey
         * Created on: 4/3/13
         * Callback for guiCreateNewAlbum. Not sure at the moment what to do if the error report is failure
         * since that means something really really bad happened.
         * Last Edited By:
         * Last Edited Date:
         */
        public void guiCreateNewAlbum_Callback(ErrorReport error)
        {
            if (error.reportID == ErrorReport.FAILURE || error.reportID == ErrorReport.SUCCESS_WITH_WARNINGS)
            {
                //something really bad happened
            }
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

        private void mainWindowAlbumList_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
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

        /*
         * Created by: Ryan Causey
         * Created on: 4/3/13
         * Event handler for when the user clicks the checkbox for creating a new album with the name
         * they specified in the text box.
         * Last Edited By:
         * Last Edited Date:
         */
        private void acceptAddToolBarButton_Click(object sender, RoutedEventArgs e)
        {
            guiValidateAlbumName(albumValidationRegex);
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
