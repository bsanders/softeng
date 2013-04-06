/*********************************************************************************
 * This is the code-behind file for the main program window
 * 
 * Changelog:
 * 4/1/13 Ryan Causey: Added checks for the DragDeltaHandlers to fix a error case
 *                     where the width/height value for the window could become
 *                     negative.
 * 4/3/13 Ryan Causey: Implementing validation of album names and GUI functions to handle
 *                     when the user wants to make a new album.
 *                     Made sure the new album name text box got cleared when the user
 *                     pressed the X to cancel creation.
 *                     Implementing gui functions to delete an album via the context menu's
 *                     delete button(X button).
 * 4/4/13 Ryan Causey: Fixed a bug where on recovery two recovery albums would appear in the library view.
 *                     Implementing switching to the album view on the album view context menu click.
 * 4/5/13 Ryan Causey: Implemented GUI function to provide a means to transition back to the Library View from
 *                     the album view. Also making sure the correct dock buttons are displayed between views.
 *                     Added temporary messagebox.show()'s for debugging.
 *                     Fixing the error for the new album name/enter comments GUI element not appearing and
 *                     dissapearing correctly.
 * 4/6/13 Ryan Causey: Implemented delete photo context menu button and gui functions. Can now delete photos.
 */ 
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace SoftwareEng
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary> 
    public partial class MainWindow : Window
    {
        // A handy shortcut to the settings class...
        Properties.Settings Settings = Properties.Settings.Default;

        //DATABINDING SOURCE 
        ReadOnlyObservableCollection<SimpleAlbumData> listOfAlbums;
        ReadOnlyObservableCollection<ComplexPhotoData> listOfPhotos;

        //--didn't know what to call it, so I named it the literal spanish translation
        public SoftwareEng.PhotoBomb bombaDeFotos;

        //--stores the albumImageList index of the default image for albums
        private const short defaultAlbumImageListIndex = 0;

        //--used to specify the UID of the "add new album" icon
        private const int addAlbumID = 0;

        //The regex for validation of album names
        private String albumValidationRegex = @"^[\w\d][\w\d ]{0,31}$"; //must be at least 1 character, max 32 in length

        private int currentAlbumUID = -1;

        //--A more stable storage for the ID of the user album instead
        //-- of relying on a form's selected items collection
        //private int albumChosenbyUser;

        public MainWindow()
        {
            InitializeComponent();

            // Set the base path for all data to be in the User's application data folder under "PhotoBomber Studios" for now.
            // This is equivalent to whatever %localappdata% resolves to in Explorer
            string basePath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Settings.OrgName);

            bombaDeFotos = new PhotoBomb();
            bombaDeFotos.init(guiConstructorCallback,
                System.IO.Path.Combine(basePath, Settings.AlbumXMLFile),
                System.IO.Path.Combine(basePath, Settings.PhotoXMLFile),
                System.IO.Path.Combine(basePath, Settings.PhotoLibraryName));

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
                showErrorMessage("Failed at guiConstructorCallback"); //super temporary
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
                showErrorMessage("failed at guiGenericErrorFunction"); //super temporary
                if (Directory.Exists("photo library_backup"))
                {
                    Directory.Delete("photo library_backup", true);
                    bombaDeFotos.rebuildBackendOnFilesystem(new generic_callback(guiGenericErrorFunction));
                }
            }
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * Edited Last By: Ryan Causey
        * Edited Last Date: 4/6/13
        * parameters: bool that determines whether list is cleared or repopulated
        * return type: void
        * purpose: refreshes list of albums
        *********************************************************************************************/
        private void populateAlbumView(bool refreshView)
        {

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

            }
            else
            {
                //show an Error
                showErrorMessage("Failed at guiAlbumsRetrieved"); //super temporary
            }

        }

        /**************************************************************************************************************************
         * Author: Ryan Causey
         * Created: 4/3/13
         * Function to validate a string against the given regex.
         * @Return: True = the string is valid, False = the string is not valid
         * @Params: Regex = the regular expression by which to evaluate the string
         * stringToValidate = the string which we will validate against the regex
         * Last Edited By:
         * Last Edited Date:
         **************************************************************************************************************************/
        private bool validateTheString(String regex, String stringToValidate)
        {
            RegexStringValidator validator = new RegexStringValidator(regex);

            try
            {
                validator.Validate(stringToValidate);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        /**************************************************************************************************************************
         * Author: Ryan Causey
         * Created on: 4/3/13
         * Function for validating that a new album name is valid and unique.
         * @Params: validationRegex = regex to be used to validate the album name
         * albumName = the desired album name
         * Last Edited By:
         * Last Edited Date:
         **************************************************************************************************************************/
        private void guiValidateAlbumName(String validationRegex)
        {
            nameTextBox.Text = nameTextBox.Text.Trim();

            if (validateTheString(validationRegex, nameTextBox.Text))
            {
                //check to see if the album name is unique in the program
                bombaDeFotos.checkIfAlbumNameIsUnique(new generic_callback(guiValidateAlbumName_Callback), nameTextBox.Text);
            }
            else
            {
                //this is how to call a storyboard defined in resources from the code
                //this storyboard is for the name box
                Storyboard nameTextBoxErrorAnimation = this.FindResource("InvalidNameFlash") as Storyboard;
                nameTextBoxErrorAnimation.Begin();

                handleNameErrorPopup(true, errorStrings.invalidAlbumNameCharacter);

                //apply error template to the text box.
                //showErrorMessage("This is a temporary error check message box failed at guiValidateAlbumName");//temporary as fuuu
                //focus the text box and select all the text
                nameTextBox.Focus();
                nameTextBox.SelectAll();
            }
        }

        /**************************************************************************************************************************
         * Author: Ryan Causey
         * Created on: 4/3/13
         * Callback for checking uniqueness of a new album name. This will be called after the back end finishes checking if the album
         * name is unique
         * Last Edited By: Ryan Causey
         * Last Edited Date: 4/5/13
         **************************************************************************************************************************/
        public void guiValidateAlbumName_Callback(ErrorReport error)
        {
            //if the album name was not unique
            if (error.reportID == ErrorReport.FAILURE || error.reportID == ErrorReport.SUCCESS_WITH_WARNINGS)
            {
                //this is how to call a storyboard defined in resources from the code
                //this storyboard is for the name box
                Storyboard nameTextBoxErrorAnimation = this.FindResource("InvalidNameFlash") as Storyboard;
                nameTextBoxErrorAnimation.Begin();

                handleNameErrorPopup(true, errorStrings.invalidAlbumNameUnique);
                //apply error template to the text box
                //showErrorMessage("This is a temporary error check message box. Failed at guiValidateAlbumName_Callback");//temporary as fuuuu
                //focus the text box and select all the text
                nameTextBox.Focus();
                nameTextBox.SelectAll();
            }
            //it was unique, great success!
            else
            {
                guiCreateNewAlbum(nameTextBox.Text);
                hideAddAlbumBox();
                nameTextBox.Clear();
            }
        }

        /**************************************************************************************************************************
         * Author: Ryan Causey
         * Created on: 4/3/13
         * Function for creating a new album name. Only to be called after the album name is validated.
         * @Param: albumName = the name for the new album being created
         * Last Edited By:
         * Last Edited Date:
         **************************************************************************************************************************/
        private void guiCreateNewAlbum(String albumName)
        {
            SimpleAlbumData newAlbum = new SimpleAlbumData();

            newAlbum.albumName = albumName;

            bombaDeFotos.addNewAlbum(new generic_callback(guiCreateNewAlbum_Callback), newAlbum);
        }

        /**************************************************************************************************************************
         * Author: Ryan Causey
         * Created on: 4/3/13
         * Last Edited By:
         * Last Edited Date:
         **************************************************************************************************************************/
        /// <summary>
        /// Callback for guiCreateNewAlbum. If there is an error something really bad happened.
        /// Notify the user, rebuild the database, and consolidate all photographs into a single backup album
        /// </summary>
        /// <param name="error">Error report from the back end.</param>
        public void guiCreateNewAlbum_Callback(ErrorReport error)
        {
            if (error.reportID == ErrorReport.FAILURE || error.reportID == ErrorReport.SUCCESS_WITH_WARNINGS)
            {
                //something really bad happened
                //notify the user, rebuild the database and consolidate all photographs into a single backup album
                showErrorMessage("Failed at guiCreateNewAlbum_Callback"); //super temporary
                bombaDeFotos.rebuildBackendOnFilesystem(new generic_callback(guiGenericErrorFunction));
            }
        }

        /**************************************************************************************************************************
         * Created By: Ryan Causey
         * Created On: 4/3/13
         * Last Edited By:
         * Last Edited Date:
         **************************************************************************************************************************/
        /// <summary>
        /// Deletes the selected album from the program.
        /// </summary>
        private void guiDeleteSelectedAlbum()
        {
            //make sure an item is selected
            if (mainWindowAlbumList.SelectedItem != null)
            {
                bombaDeFotos.removeAlbum(new generic_callback(guiDeleteSelectedAlbum_Callback), ((SimpleAlbumData)mainWindowAlbumList.SelectedItem).UID); 
            }
        }

        /**************************************************************************************************************************
         * Created By: Ryan Causey
         * Created On: 4/3/13
         * Last Edited By:
         * Last Edited Date:
         **************************************************************************************************************************/
        /// <summary>
        /// Callback for guiDeleteSelectedAlbum. If there is an error something really bad happened.
        /// Notify the user, rebuild the database and consolidate all photographs into a single backup album.
        /// </summary>
        /// <param name="error">Error report from the back end.</param>
        public void guiDeleteSelectedAlbum_Callback(ErrorReport error)
        {
            if (error.reportID == ErrorReport.FAILURE || error.reportID == ErrorReport.SUCCESS_WITH_WARNINGS)
            {
                //something really bad happened
                //notify the user, rebuild the database and consolidate all photographs into a single backup album
                showErrorMessage("Failed at guiDeleteSelectedAlbum_Callback"); //super temporary
                bombaDeFotos.rebuildBackendOnFilesystem(new generic_callback(guiGenericErrorFunction));
            }
        }

        /**************************************************************************************************************************
         * Created By: Ryan Causey
         * Created On: 4/4/13
         * Last Edited By:
         * Last Edited Date:
         **************************************************************************************************************************/
        /// <summary>
        /// GUI function to begin the transition to the album view from the library view.
        /// </summary>
        private void guiEnterAlbumView()
        {
            //make sure an item is selected
            if (mainWindowAlbumList.SelectedItem != null)
            {
                //call the backend to get all photos in this album.
                currentAlbumUID = ((SimpleAlbumData)mainWindowAlbumList.SelectedItem).UID;
                bombaDeFotos.getAllPhotosInAlbum(new getAllPhotosInAlbum_callback(guiEnterAlbumView_Callback), currentAlbumUID);
            }
        }

        /**************************************************************************************************************************
         * Created By: Ryan Causey
         * Created On: 4/5/13
         * Last Edited By:
         * Last Edited Date:
         **************************************************************************************************************************/
        /// <summary>
        /// Callback for guiEnterAlbumView. Takes the returned ReadOnlyObservableCollection and binds the listView to it
        /// as well as swaps the data template.
        /// </summary>
        /// <param name="error">Error report from backend</param>
        /// <param name="picturesInAlbum">Collection of photos in the album.</param>
        public void guiEnterAlbumView_Callback(ErrorReport error, ReadOnlyObservableCollection<ComplexPhotoData> picturesInAlbum)
        {
            if (error.reportID == ErrorReport.FAILURE)
            {
                //show user an error message that retrieving the pictures did not work
                showErrorMessage("Failed at guiEnterAlbumView_Callback"); //super temporary
            }
            else
            {
                if (error.reportID == ErrorReport.SUCCESS_WITH_WARNINGS)
                {
                    //show the user a notification that some pictures are not displayed
                    showErrorMessage("Warnings at guiEnterAlbumView_Callback"); //super temporary
                }
                //swap data templates and change bindings.
                mainWindowAlbumList.ItemTemplate = this.Resources["ListItemTemplate"] as DataTemplate;
                listOfPhotos = picturesInAlbum;
                mainWindowAlbumList.ItemsSource = listOfPhotos;
                //show the return to library view button on the dock
                libraryDockButton.Visibility = Visibility.Visible;
                //show the addPhotos dock button
                addPhotosDockButton.Visibility = Visibility.Visible;
                //hide the add new album button on the dock
                addDockButton.Visibility = Visibility.Collapsed;
                //temporary fix to prevent an unhandled exception
                viewMenuItemLibraryButton.Visibility = Visibility.Collapsed;
                //hide the delete album button
                deleteMenuItemLibraryButton.Visibility = Visibility.Collapsed;
                //show the delete photo button
                deleteMenuItemPhotoButton.Visibility = Visibility.Visible;
            }
        }

        /**************************************************************************************************************************
         * Created By: Ryan Causey
         * Created Date: 4/5/13
         * Last Edited By:
         * Last Edited Date:
         **************************************************************************************************************************/
        /// <summary>
        /// GUI function to transition back to the library view by changing the data template and item source.
        /// </summary>
        private void guiReturnToLibraryView()
        {
            mainWindowAlbumList.ItemTemplate = this.Resources["LibraryListItemFrontTemplate"] as DataTemplate;
            //refresh the view to make sure we update with new album thumbnails
            populateAlbumView(true);
            mainWindowAlbumList.ItemsSource = listOfAlbums;
            //collapse the go back button
            libraryDockButton.Visibility = Visibility.Collapsed;
            //collapse the addPhotos button
            addPhotosDockButton.Visibility = Visibility.Collapsed;
            //show the add album dock button
            addDockButton.Visibility = Visibility.Visible;
            //temporary fix to prevent an unhandled exception
            viewMenuItemLibraryButton.Visibility = Visibility.Visible;
            //show the delete album button
            deleteMenuItemLibraryButton.Visibility = Visibility.Visible;
            //hide the delete photo button
            deleteMenuItemPhotoButton.Visibility = Visibility.Collapsed;

            currentAlbumUID = -1;
        }

        /**************************************************************************************************************************
         * Created By: Ryan Causey
         * Created Date: 4/5/13
         * Last Edited By:
         * Last Edited Date:
         **************************************************************************************************************************/
        /// <summary>
        /// GUI function that shows a file dialogue and then calls the back end to add the selected photographs
        /// </summary>
        private void guiImportPhotos()
        {
            OpenFileDialog photoDialogue = new OpenFileDialog();
            //filter the file types available
            photoDialogue.Filter = "Jpeg images|*.jpg;*.jpeg;*.jpe;*.jfif;";
            //set the intial directory to the my pictures directory
            photoDialogue.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            //allow multiple photographs to be selected
            photoDialogue.Multiselect = true;

            //show the dialogue
            Nullable<bool> openFilesResult = photoDialogue.ShowDialog();

            //if the user selected files
            if ((bool)openFilesResult)
            {
                //need to get the file extensions for the goddamned splicers
                List<string> extensions = new List<string>();

                //GODDAMNED SPLICERS!
                foreach(String s in photoDialogue.FileNames)
                {
                    extensions.Add(System.IO.Path.GetExtension(s));
                }

                //show the progress bar
                progressBar.Visibility = Visibility.Visible;
                progressBar.Minimum = 1;
                progressBar.Maximum = photoDialogue.FileNames.GetLength(0);

                //pass all the files names to a backend function call to start adding the files.
                //fix the function parameters before releasing.
                bombaDeFotos.addNewPictures(new generic_callback(guiImportPhotos_Callback), new List<string>(photoDialogue.FileNames), extensions, currentAlbumUID, null, new ProgressChangedEventHandler(guiUpdateProgressBar_Callback), 1); 
            }
        }

        /**************************************************************************************************************************
         * Created By: Ryan Causey
         * Created Date: 4/5/13
         * Last Edited By:
         * Last Edited Date:
         **************************************************************************************************************************/
        /// <summary>
        /// Callback for guiImportPhotos which recieves the error report from the back end.
        /// </summary>
        /// <param name="error">Error report from the back end addNewPictures function.</param>
        public void guiImportPhotos_Callback(ErrorReport error)
        {
            //deal with it
            if (error.reportID == ErrorReport.FAILURE)
            {
                //shit done fucked up
                showErrorMessage("Failed at guiImportPhotos_Callback"); //super temporary
            }
            else
            {
                if (error.reportID == ErrorReport.SUCCESS_WITH_WARNINGS)
                {
                    //warn about shit
                    showErrorMessage("Warning at guiImportPhotos_Callback"); //super temporary
                }

                progressBar.Visibility = Visibility.Collapsed;
                bombaDeFotos.getAllPhotosInAlbum(new getAllPhotosInAlbum_callback(guiImportPhotosRefreshView_Callback), currentAlbumUID);
            }
        }

        /**************************************************************************************************************************
         * Created By: Ryan Causey
         * Created Date: 4/5/13
         * Last Edited By:
         * Last Edited Date:
         **************************************************************************************************************************/
        public void guiImportPhotosRefreshView_Callback(ErrorReport error, ReadOnlyObservableCollection<ComplexPhotoData> picturesInAlbum)
        {
            if (error.reportID == ErrorReport.FAILURE)
            {
                //show user an error message that retrieving the pictures did not work
                showErrorMessage("Failed at guiImportPhotosRefreshView_Callback"); //super temporary
            }
            else
            {
                if (error.reportID == ErrorReport.SUCCESS_WITH_WARNINGS)
                {
                    //show the user a notification that some pictures are not displayed
                    showErrorMessage("Warnings at guiImportPhotosRefreshView_Callback"); //super temporary
                }
                listOfPhotos = picturesInAlbum;
            }
        }

        /**************************************************************************************************************************
         * Created By: Ryan Causey
         * Created Date: 4/6/13
         * Last Edited By:
         * Last Edited Date:
         **************************************************************************************************************************/
        /// <summary>
        /// GUI function to delete the selecte photograph from the current album.
        /// If multiple photos are selected, it will remove the first selected item.
        /// Can probably do multiple delete later if we have time...
        /// </summary>
        private void guiDeleteSelectedPhoto()
        {
            if (mainWindowAlbumList.SelectedItem != null)
            {
                bombaDeFotos.removePictureFromAlbum(new generic_callback(guiDeleteSelectedPhoto_Callback), ((ComplexPhotoData)mainWindowAlbumList.SelectedItem).UID, currentAlbumUID);
            }
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        public void guiDeleteSelectedPhoto_Callback(ErrorReport error)
        {
            if (error.reportID == ErrorReport.FAILURE)
            {
                showErrorMessage(error.description);
            }
        }

        /**************************************************************************************************************************
         * Created By: Ryan Causey
         * Created Date: 4/5/13
         * Last Edited By:
         * Last Edited Date:
         **************************************************************************************************************************/
        /// <summary>
        /// Function to increment the progress bar as photos import.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event args</param>
        public void guiUpdateProgressBar_Callback(object sender, ProgressChangedEventArgs e)
        {
            ++progressBar.Value;
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        private void mainWindowDock_MouseLeave(object sender, MouseEventArgs e)
        {
            mainWindowDock.Height = 1;
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        private void mainWindowContextMenu_LostMouseCapture(object sender, MouseEventArgs e)
        {
            libraryContextMenu.IsOpen = false;
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        private void dockHitBox_MouseEnter(object sender, MouseEventArgs e)
        {
            mainWindowDock.Height = Double.NaN;
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        private void exitDockButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        private void mainWindowAlbumList_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            libraryContextMenu.IsOpen = true;
        }

        /**************************************************************************************************************************
         * Created By: Ryan Causey
         * Created On: 4/3/13
         * Event Handler for the large + button on the dock. Shows the add new album box.
         * Last Edited By:
         * Last Edited Date:
         **************************************************************************************************************************/
        private void addDockButton_Click(object sender, RoutedEventArgs e)
        {
            showAddAlbumBox();
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

        /**************************************************************************************************************************
         * Created By: Alejandro Sosa
         * Edited Last By: Ryan Causey
         * Edited Last Date: 4/5/13
         **************************************************************************************************************************/
        private void showAddAlbumBox()
        {
            ItemAddOrEditDialogBar.Visibility = Visibility.Visible;

            NameTextBlock.Visibility = Visibility.Visible;
            nameTextBox.Visibility = Visibility.Visible;

            acceptAddToolbarButton.Visibility = Visibility.Visible;
            cancelAddToolbarButton.Visibility = Visibility.Visible;

            Keyboard.Focus(nameTextBox);
        }

        /**************************************************************************************************************************
         * Created By: Alejandro Sosa
         * Edited Last By: Ryan Causey
         * Edited Last Date: 4/5/13
         **************************************************************************************************************************/
        private void hideAddAlbumBox()
        {
            ItemAddOrEditDialogBar.Visibility = Visibility.Collapsed;

            NameTextBlock.Visibility= Visibility.Hidden;
            nameTextBox.Visibility = Visibility.Hidden;

            acceptAddToolbarButton.Visibility = Visibility.Hidden;
            cancelAddToolbarButton.Visibility = Visibility.Hidden;
        }

        /**************************************************************************************************************************
         * Created By: Alejandro Sosa
         * Last Edited By: Ryan Causey
         * Last Edited Date: 4/3/13
         **************************************************************************************************************************/
        private void cancelAddToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            hideAddAlbumBox();
            //make sure to clear the text box and close error popup
            nameTextBox.Clear();
            invalidInputPopup.IsOpen = false;

            //stop any error animations
            Storyboard nameTextBoxErrorAnimation = this.FindResource("InvalidNameFlash") as Storyboard;
            nameTextBoxErrorAnimation.Stop();
        }



        /*****************************************
         * start region of thumb bar resize events
        *****************************************/



        /**************************************************************************************************************************
         *Created By: Alejandro Sosa
         *Last Edited By: Ryan Causey
         *Last Edited Date: 4/1/13
         *This handles the resizing via dragging from the bottom edge of the window, making sure the window height
         *does not go below a certain minimum
         **************************************************************************************************************************/
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

        /**************************************************************************************************************************
         *Created By: Alejandro Sosa
         *Last Edited By: Ryan Causey
         *Last Edited Date: 4/1/13
         *This handles the resizing via dragging from the top edge of the window, making sure the window height
         *does not go below a certain minimum
         **************************************************************************************************************************/
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

        /**************************************************************************************************************************
         *Created By: Alejandro Sosa
         *Last Edited By: Ryan Causey
         *Last Edited Date: 4/1/13
         *This handles the resizing via dragging from the right edge of the window, making sure the window width
         *does not go below a certain minimum
         **************************************************************************************************************************/
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

        /**************************************************************************************************************************
         *Created By: Alejandro Sosa
         *Last Edited By: Ryan Causey
         *Last Edited Date: 4/1/13
         *This handles the resizing via dragging from the left edge of the window, making sure the window width
         *does not go below a certain minimum
         **************************************************************************************************************************/
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

        /**************************************************************************************************************************
         *Created By: Alejandro Sosa
         *Last Edited By: Ryan Causey
         *Last Edited Date: 4/1/13
         *This handles the resizing via dragging from the bottom right corner of the window, making sure the window height
         *and width do not go below a certain minimum
         **************************************************************************************************************************/
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

        /**************************************************************************************************************************
         *Created By: Alejandro Sosa
         *Last Edited By: Ryan Causey
         *Last Edited Date: 4/1/13
         *This handles the resizing via dragging from the top right corner of the window, making sure the window height
         *and width do not go below a certain minimum
         **************************************************************************************************************************/
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

        /**************************************************************************************************************************
         *Created By: Alejandro Sosa
         *Last Edited By: Ryan Causey
         *Last Edited Date: 4/1/13
         *This handles the resizing via dragging from the bottom left corner of the window, making sure the window height
         *and width do not go below a certain minimum
         **************************************************************************************************************************/
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
        /***************************************
         * end region of thumb bar resize events
        ***************************************/


        /**************************************************************************************************************************
        **************************************************************************************************************************/
        private void aboutButtonPressed_eventHandler(object sender, RoutedEventArgs e)
        {
            aboutWindow someWindow = new aboutWindow();

            someWindow.ShowDialog();
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
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

        /**************************************************************************************************************************
         * Created by: Ryan Causey
         * Created on: 4/3/13
         * Event handler for when the user clicks the checkbox for creating a new album with the name
         * they specified in the text box.
         * Last Edited By:
         * Last Edited Date:
         **************************************************************************************************************************/
        private void acceptAddToolBarButton_Click(object sender, RoutedEventArgs e)
        {
            guiValidateAlbumName(albumValidationRegex);
        }

        /**************************************************************************************************************************
         * Created by: Ryan Causey
         * Created Date: 4/3/13
         * Last Edited By:
         * Last Edited Date:
         **************************************************************************************************************************/
        /// <summary>
        /// Event handler for the Click event on the delete button on the context menu.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event Args</param>
        private void deleteMenuItemLibraryButton_Click(object sender, RoutedEventArgs e)
        {
            //call a function here.
            guiDeleteSelectedAlbum();
        }

        /**************************************************************************************************************************
         * Created By: Ryan Causey
         * Created Date: 4/4/13
         * Last Edited By:
         * Last Edited Date:
         **************************************************************************************************************************/
        /// <summary>
        /// Event handler for the Click event on the view album button on the context menu.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event args</param>
        private void viewMenuItemLibraryButton_Click(object sender, RoutedEventArgs e)
        {
            //there shall be a function call that does shit
            guiEnterAlbumView();
        }

        /**************************************************************************************************************************
         * Created By: Ryan Causey
         * Created Date: 4/5/13
         * Last Edited By:
         * Last Edited Date:
         **************************************************************************************************************************/
        /// <summary>
        /// Event handler for the return to library view button on the dock.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event args</param>
        private void libraryDockButton_Click(object sender, RoutedEventArgs e)
        {
            //call some goddamned function here
            guiReturnToLibraryView();
        }

        /**************************************************************************************************************************
         * Created By: Ryan Causey
         * Created Date: 4/5/13
         * Last Edited By:
         * Last Edited Date:
         **************************************************************************************************************************/
        /// <summary>
        /// Event handler for the add photos button click.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event args</param>
        private void addPhotosDockButton_Click(object sender, RoutedEventArgs e)
        {
            //call another function
            guiImportPhotos();
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        private void PopupMouseClick_Handler(object sender, MouseButtonEventArgs e)
        {
            libraryContextMenu.IsOpen = false;
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        private void handleNameErrorPopup(bool showIt, string errorMessage)
        {
            if (showIt == false)
            {
                invalidInputPopup.IsOpen = false;
                return;
            }

            errorBalloon.Content = errorMessage;
            invalidInputPopup.IsOpen = true;

        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        private void showErrorMessage(String messageOfDoom)
        {
            ErrorWindow bearerOfBadNews = new ErrorWindow(messageOfDoom);

            bearerOfBadNews.ShowDialog();
        }

        /**************************************************************************************************************************
         * Created By: Ryan Causey
         * Created Date: 4/6/13
         * Last Edited By:
         * Last Edited Date:
         **************************************************************************************************************************/
        private void deleteMenuItemPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            //call guideletephoto function here.
            guiDeleteSelectedPhoto();
        }
    }
}
