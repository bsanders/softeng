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
 *                     Implemented an IValueConverter interface to allow us to bind to caches of image files
 *                     as to not lock the file reference causing issues on delete.
 *                     Added functionality to disable the addNewPhotos button while an import operation is in progress.
 *                     User can still browse around and add more albums however.
 *                     Updated handler for the close window operation to stop any import operations if they are occurring.
 *                     Added gui functionality to cancel the import operation while in progress.
 *                     Fixed the remove picture gui function so we pass the correct UID(idInAlbum) to the backend function.
 *                     Fixed an issue in the ImagePathConverter where it would not handle a case where the path was not yet loaded
 *                     to the image to convert.
 *                     Changed two having two context menu's, one for library view and one for album view.
 *                     Fixed a bug where the default image was no longer appearing.
 *        Bill Sanders: ditched the parameter on guiValidateName
 * 4/7/13 Ryan Causey: Fixed a bug where the error style would not clear after clicking the checkbox worked to add an album/photo.
 *                     Fixed a possible infinite loop.
 *                     Converting all hard coded prompt/error strings to use strings in the error/promptStrings files.
 *                     Fixed a bug where multiple view image windows could be opened.
 *                     Changed context menu behavior to match srs specification. It now only shows over an album or photo item in the
 *                     list view. Commented out old handlers that are no longer used.
 * 4/8/13 Ryan Causey: Adding comment box functionality to the GUI.
 *                     Fixed a bug with the viewImage function that would lead to an unhandled exception.
 *                     Changed the returnToLibraryView function to close any open viewImage windows.
 *                     Fixed a bug where the application would close the main window but not end the process.
 *                     Implemented changing of image captions. Behavior is as follows: If both the photo's name field is
 *                     empty, it only changes the photo's caption field. If the photo's name field is not empty, it will validate
 *                     the photo name and then validate the caption.
 *                     Changed the regex for caption validation to allow blank captions, in order to allow captions to be removed.
 * 4/28/13 Julian Nguyen:
 *                     rename PhotoBomb -> PhotoBomb_Controller.
 *         Alejandro Sosa:
 *                     Gui fixes,  implemented themes, added custom routed event
 *          
 * 4/30/13 Julian Nguyen:
 *                      ErrorReports constants numbers removed and replaced with ReportStatus enums.
 *                      Fun() with "Picture" in the name were changed to "Image"
 * 5/1/13 Julian Nguyen
 *                      Renamed the class fields to have '_'! 
 * 
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
using System.Timers;

namespace SoftwareEng
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary> 
    public partial class MainWindow : Window
    {
        //check for determining which view the window is in
        private bool _isInsideAlbum;

        // A handy shortcut to the settings class...
        Properties.Settings Settings = Properties.Settings.Default;

        //ugly hacks needed for proper hit testing for Dock animations
        private bool isMouseInDockHitBox = false;
        private bool isMouseInDock = false;

        //the view image window
        ViewImage _view = new ViewImage();

        //the about window
        AboutWindow _photoBomberAboutWindow;

        //DATABINDING SOURCE 
        private ReadOnlyObservableCollection<SimpleAlbumData> _listOfAlbums;
        private ReadOnlyObservableCollection<ComplexPhotoData> _listOfPhotos;
        private List<ComplexPhotoData> _clipboardOfPhotos = new List<ComplexPhotoData>();
        private Guid _clipboardAlbumSrcID = Guid.Empty;

        public ReadOnlyObservableCollection<SimpleAlbumData> listOfAlbums
        {
            get
            {
                return _listOfAlbums;
            }
        }

        public ReadOnlyObservableCollection<ComplexPhotoData> listOfPhotos
        {
            get
            {
                return _listOfPhotos;
            }
        }

        //--didn't know what to call it, so I named it the literal spanish translation
        public SoftwareEng.PhotoBomb_Controller _bombaDeFotos;

        //--stores the albumImageList index of the default image for albums
        private const short _defaultAlbumImageListIndex = 0;

        //--used to specify the UID of the "add new album" icon
        //private const int addAlbumID = 0;

        //The regex for validation of album names
        private String _albumValidationRegex = @promptStrings.albumValidationRegex; //must be at least 1 character, max 32 in length

        //The regex for validation of photo names
        private String _photoValidationRegex = @promptStrings.photoValidationRegex; //must be at least 1 character, max 32 in length

        //The regex for validation of captions
        private String _captionValidationRegex = @promptStrings.captionValidationRegex;

        //current album UID, defaults to a invalid value(because we eont be in an album)
        private Guid _currentAlbumUID = Guid.Empty;

        //keep track of if an import operation is occurring
        private bool _isImporting = false;

        //--A more stable storage for the ID of the user album instead
        //-- of relying on a form's selected items collection
        //private int albumChosenbyUser;

        //temp variable to test out collection views
        private CollectionView _ImageListCollectionView;

        public MainWindow()
        {
            InitializeComponent();

            // Set the base path for all data to be in the User's application data folder under "PhotoBomber Studios" for now.
            // This is equivalent to whatever %localappdata% resolves to in Explorer
            string basePath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Settings.OrgName);

            _bombaDeFotos = new PhotoBomb_Controller();
            _bombaDeFotos.init(guiConstructorCallback,
                System.IO.Path.Combine(basePath, Settings.AlbumXMLFile),
                System.IO.Path.Combine(basePath, Settings.PhotoXMLFile),
                System.IO.Path.Combine(basePath, Settings.PhotoLibraryName));

            isMouseInDockHitBox = false;
            isMouseInDock = false;

            hideAddAlbumBox();

            populateAlbumView(true);

            _isInsideAlbum = false;
            if (mainWindowAlbumList.Items.IsEmpty == false)
            {
                mainWindowAlbumList.SelectedItem = mainWindowAlbumList.Items[0];
                mainWindowAlbumList.Focus();
            }

            gui_getCurrentTheme();
        }


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: an object of ErrorReport which will be used to check if backend was successful
        * return type: void
        * purpose: simply calls back end
        *********************************************************************************************/
        private void guiConstructorCallback(ErrorReport status)
        {
            if (status.reportStatus != ReportStatus.SUCCESS)
            {
                //showErrorMessage("Failed at guiConstructorCallback"); //super temporary
                _bombaDeFotos.rebuildBackendOnFilesystem(new generic_callback(rebuildBackend_Callback));
            }
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * Edited Last By: Ryan Causey
        * Edited Last Date: 4/7/13
        * parameters: an object of ErrorReport which will be used to check if backend was successful
        * return type: void
        * purpose: used for blowing up the backend and restarting 
        *********************************************************************************************/
        private void rebuildBackend_Callback(ErrorReport status)
        {
            if (status.reportStatus != ReportStatus.SUCCESS)
            {
                //showErrorMessage(errorStrings.rebuildBackendFailure); //super temporary
                _bombaDeFotos.rebuildBackendOnFilesystem(new generic_callback(rebuildBackend_Callback));
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
                _bombaDeFotos.getAllAlbums(new getAllAlbumNames_callback(guiAlbumsRetrieved));
            }
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * Edited Last By: Ryan Causey
        * Edited Last Date: 4/7/13
        * parameters: an object of ErrorReport which will be used to check if backend was successful,
        *   and a list <type is SimpleAlbumData> containing data to identify all albums requested
        * return type: void
        * purpose: list of albums returned from the backend
        *********************************************************************************************/
        public void guiAlbumsRetrieved(ErrorReport status, ReadOnlyObservableCollection<SimpleAlbumData> albumsRetrieved)
        {
            if (status.reportStatus == ReportStatus.FAILURE)
            {
                //show an Error
                showErrorMessage(errorStrings.retrieveAlbumsFailure);
            }
            else
            {
                if (status.reportStatus == ReportStatus.SUCCESS_WITH_WARNINGS)
                {
                    showErrorMessage(errorStrings.retrieveAlbumsWarning);
                }
                _listOfAlbums = albumsRetrieved;

                mainWindowAlbumList.ItemsSource = _listOfAlbums;
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
         * Last Edited By: Ryan Causey
         * Last Edited Date: 4/8/13
         **************************************************************************************************************************/
        private void guiValidateAlbumName()
        {
            // Trim the whitespace of this input, SRS Requires no leading/trailing whitespace
            nameTextBox.Text = nameTextBox.Text.Trim();
            Storyboard nameTextBoxErrorAnimation = this.FindResource("InvalidNameFlash") as Storyboard;
            try
            {
                nameTextBoxErrorAnimation.Stop();
                handleNameErrorPopup(false, "");
            }
            catch (Exception)
            {
                showErrorMessage("We're going to DIIEEEEEE-bleh");
            }

            if (!validateTheString(promptStrings.albumValidationRegex, nameTextBox.Text))
            {
                // If the text doesn't validate, display an error...
                //this is how to call a storyboard defined in resources from the code
                //this storyboard is for the name box


                try
                {
                    nameTextBoxErrorAnimation.Begin();
                }
                catch (Exception)
                {
                    showErrorMessage("We're going to DIIEEEEEE-bleh");
                }
                

                handleNameErrorPopup(true, errorStrings.invalidAlbumNameCharacter);
                //showErrorMessage("This is a temporary error check message box failed at guiValidateAlbumName");//temporary as fuuu
                //focus the text box and select all the text
                nameTextBox.Focus();
                nameTextBox.SelectAll();
                return;
            }

            // Otherwise the text was good, but still might not be unique.
            // Let's check Library vs Album check again...
            //check to see if the album name is unique in the program
            _bombaDeFotos.checkIfAlbumNameIsUnique(new generic_callback(guiValidateAlbumName_Callback), nameTextBox.Text);
        }

        /**************************************************************************************************************************
         * Author: Ryan Causey
         * Created on: 4/3/13
         * Callback for checking uniqueness of a new album name. This will be called after the back end finishes checking if the album
         * name is unique
         * Last Edited By: Ryan Causey
         * Last Edited Date: 4/7/13
         **************************************************************************************************************************/
        public void guiValidateAlbumName_Callback(ErrorReport error)
        {
            //if the album name was not unique
            Storyboard nameTextBoxErrorAnimation = this.FindResource("InvalidNameFlash") as Storyboard;
            try
            {
                nameTextBoxErrorAnimation.Stop();
                handleNameErrorPopup(false, "");
            }
            catch (Exception)
            {
                showErrorMessage("We're going to DIIEEEEEE-bleh");
            }
            if (error.reportStatus == ReportStatus.FAILURE || error.reportStatus == ReportStatus.SUCCESS_WITH_WARNINGS)
            {
                //this is how to call a storyboard defined in resources from the code
                //this storyboard is for the name box

                try
                {
                    nameTextBoxErrorAnimation.Begin();
                }
                catch (Exception)
                {
                    showErrorMessage("We're going to DIIEEEEEE-bleh");
                }
                

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
                if (mainWindowAlbumList.SelectedItem != null)
                {
                    guiRenameSelectedAlbum(nameTextBox.Text);
                }
                else
                {
                    guiCreateNewAlbum(nameTextBox.Text);
                }
                hideAddAlbumBox();
                nameTextBox.Clear();
                invalidInputPopup.IsOpen = false;

                //stop any error animations
                nameTextBoxErrorAnimation.Stop();
            }
        }

        /*
         * Created by: Ryan Causey
         * Created Date: 4/8/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// Function to validate the photo name entered. Also calls 
        /// </summary>
        private void guiValidatePhotoName()
        {
            // Trim the whitespace of this input, SRS Requires no leading/trailing whitespace
            photoNameTextBox.Text = photoNameTextBox.Text.Trim();

            Storyboard nameTextBoxErrorAnimation = this.FindResource("InvalidNameFlash") as Storyboard;
            try
            {
                nameTextBoxErrorAnimation.Stop();
                handleNameErrorPopup(false, "");
            }
            catch (Exception)
            {
                showErrorMessage("We're going to DIIEEEEEE-bleh");
            }

            if (!validateTheString(promptStrings.albumValidationRegex, photoNameTextBox.Text))
            {
                // If the text doesn't validate, display an error...
                //this is how to call a storyboard defined in resources from the code
                //this storyboard is for the name box
                try
                {
                    nameTextBoxErrorAnimation.Begin();

                }
                catch (Exception)
                {
                    showErrorMessage("We're going to DIIEEEEEE-bleh");
                }

                handlePhotoNameErrorPopup(true, errorStrings.invalidImageNameCharacter);
                //showErrorMessage("This is a temporary error check message box failed at guiValidateAlbumName");//temporary as fuuu
                //focus the text box and select all the text
                photoNameTextBox.Focus();
                photoNameTextBox.SelectAll();
                return;
            }
            // Otherwise the text was good, but still might not be unique.
            //check to see if the photo name is unique in the program
            _bombaDeFotos.isImageNameUnique(
                new generic_callback(guiValidatePhotoName_Callback),
                photoNameTextBox.Text,
                _currentAlbumUID);
        }

        /**************************************************************************************************************************
         * Author: Bill Sanders, based on code by Ryan Causey
         * Created on: 4/3/13
         * Last Edited By: Ryan Causey
         * Last Edited Date: 4/8/13
         * Callback for checking uniqueness of a new photo name. This will be called after the back end finishes checking if the photo
         * name is unique
         **************************************************************************************************************************/
        public void guiValidatePhotoName_Callback(ErrorReport error)
        {
            Storyboard photoNameTextBoxErrorAnimation = this.FindResource("InvalidPhotoNameFlash") as Storyboard;
            try
            {
                photoNameTextBoxErrorAnimation.Stop();
                handleNameErrorPopup(false, "");
            }
            catch (Exception)
            {
                showErrorMessage("We're going to DIIEEEEEE-bleh");
            }


            //if the photo name was not unique
            if (error.reportStatus == ReportStatus.FAILURE || error.reportStatus == ReportStatus.SUCCESS_WITH_WARNINGS)
            {
                //this is how to call a storyboard defined in resources from the code
                //this storyboard is for the name box

                try
                {
                    photoNameTextBoxErrorAnimation.Begin();
                }
                catch (Exception)
                {
                    showErrorMessage("We're going to DIIEEEEEE-bleh");
                }

                handlePhotoNameErrorPopup(true, errorStrings.invalidImageNameUnique);
                //apply error template to the text box
                //focus the text box and select all the text
                photoNameTextBox.Focus();
                photoNameTextBox.SelectAll();
            }
            //it was unique, great success!
            else
            {
                try
                {
                    photoNameTextBoxErrorAnimation.Stop();
                    handleNameErrorPopup(false, "");
                }
                catch (Exception)
                {
                    showErrorMessage("We're going to DIIEEEEEE-bleh");
                }
                //now need to validate the caption with the photo, if there is one.
                guiValidateCaptionContent();
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

            _bombaDeFotos.addNewAlbum(new generic_callback(guiCreateNewAlbum_Callback), newAlbum);
        }

        /**************************************************************************************************************************
         * Author: Ryan Causey
         * Created on: 4/3/13
         * Last Edited By: Ryan Causey
         * Last Edited Date: 4/7/13
         **************************************************************************************************************************/
        /// <summary>
        /// Callback for guiCreateNewAlbum. If there is an error something really bad happened.
        /// Notify the user, rebuild the database, and consolidate all photographs into a single backup album
        /// </summary>
        /// <param name="error">Error report from the back end.</param>
        public void guiCreateNewAlbum_Callback(ErrorReport error)
        {
            if (error.reportStatus == ReportStatus.FAILURE || error.reportStatus == ReportStatus.SUCCESS_WITH_WARNINGS)
            {
                //something really bad happened
                //notify the user, rebuild the database and consolidate all photographs into a single backup album
                showErrorMessage(errorStrings.addAlbumFailure); //super temporary
                _bombaDeFotos.rebuildBackendOnFilesystem(new generic_callback(rebuildBackend_Callback));
            }
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/8/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// GUI function to validate the caption content and add it if it is valid.
        /// If we have gotten this far we have already validated the photo name.
        /// </summary>
        private void guiValidateCaptionContent()
        {
            //trim the input of leading and trailing whitespace
            commentTextBox.Text = commentTextBox.Text.Trim();
            //if it is not a valid string
            if (!validateTheString(_captionValidationRegex, commentTextBox.Text))
            {
                Storyboard commentTextBoxAnimation = this.FindResource("InvalidCommentFlash") as Storyboard;
                commentTextBoxAnimation.Begin();

                handleCommentErrorPopup(true, errorStrings.invalidComment);
                commentTextBox.Focus();
                commentTextBox.SelectAll();
            }
            else
            {
                guiChangePhotoCaption(commentTextBox.Text);
                if (photoNameTextBox.Text != "")
                {
                    guiRenameSelectedPhoto(photoNameTextBox.Text);
                }
                //clean up the comment box error dialogues and also clear the text boxes
                Storyboard commentTextBoxAnimation = this.FindResource("InvalidCommentFlash") as Storyboard;
                commentTextBoxAnimation.Stop();
                handleCommentErrorPopup(false, "");
                commentTextBox.Clear();
                photoNameTextBox.Clear();
                hideAddAlbumBox();
            }
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/8/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// Gui function to change a photo's caption
        /// </summary>
        /// <param name="caption">The new caption value</param>
        private void guiChangePhotoCaption(String caption)
        {
            if (mainWindowAlbumList.SelectedItem != null)
            {
                _bombaDeFotos.setImageCaption(new generic_callback(guiChangePhotoCaption_Callback), _currentAlbumUID, ((ComplexPhotoData)mainWindowAlbumList.SelectedItem).idInAlbum, caption);
            }
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/8/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// Callback for guiChangePhotoCaption. Simply displays an error if there was one.
        /// </summary>
        /// <param name="error"></param>
        public void guiChangePhotoCaption_Callback(ErrorReport error)
        {
            if (error.reportStatus == ReportStatus.FAILURE)
            {
                showErrorMessage(errorStrings.changeCommentFailure);
            }
        }

        /**************************************************************************************************************************
         * Author: Bill Sanders
         * Created on: 4/7/13
         **************************************************************************************************************************/
        /// <summary>
        /// Copies the selected photos to the background clipboard.
        /// </summary>
        private void guiCopySelectedPhotosToClipboard()
        {
            //make sure an item is selected
            if (mainWindowAlbumList.SelectedItems != null)
            {
                // if the clipboard is currently empty, we're in copy-mode
                ComplexPhotoData[] clipArray = new ComplexPhotoData[mainWindowAlbumList.SelectedItems.Count];
                mainWindowAlbumList.SelectedItems.CopyTo(clipArray, 0);
                _clipboardOfPhotos = clipArray.ToList();
                _clipboardAlbumSrcID = _currentAlbumUID;
            }
        }

        /**************************************************************************************************************************
         * Author: Bill Sanders
         * Created on: 4/7/13
         * Last Edited By: Ryan Causey
         * Last Edited Date: 4/7/13
         **************************************************************************************************************************/
        /// <summary>
        /// Pastes the photos from the background clipboard to the selected album.
        /// </summary>
        private void guiPasteClipboardPhotosToAlbum()
        {
            // First check if the clipboard even has anything...
            if (_clipboardOfPhotos.Count == 0)
            {
                // Nope, just ignore the action.
                //showErrorMessage(errorStrings.failedToPasteImages);
                return;
            }

            // So we have photos, where are we pasting them?
            // Check if we're in library view or album view
            Guid albumUID = _currentAlbumUID;
            if (albumUID == Guid.Empty)
            {
                // library view
                if (mainWindowAlbumList.SelectedItem == null)
                {
                    //showErrorMessage(errorStrings.copyToClipboardWarning);
                    return;
                }

                albumUID = ((SimpleAlbumData)mainWindowAlbumList.SelectedItem).UID;
            }

            // Now that we have the album and a given set of pictures

            // Tell the GUI we are importing
            _isImporting = true;
            //hide the addPhotosDockButton
            addPhotosDockButton.Visibility = Visibility.Collapsed;
            //show the cancel import dock button
            //cancelPhotoImportDockButton.Visibility = Visibility.Visible;

            //pass all the files names to a backend function call to start adding the files.
            // sneakily reuse the guiImportPhotos_Callback....
            _bombaDeFotos.addExistingImagesToAlbum(
                new addNewPictures_callback(guiImportPhotos_Callback),
                _clipboardOfPhotos,
                albumUID);

            // repopulate the list of albums, mostly to get the new thumbnail generated, if applicable.
            populateAlbumView(true);

            // empty the clipbaord.
            _clipboardOfPhotos.Clear();
            _clipboardAlbumSrcID = Guid.Empty;
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
                SimpleAlbumData albumToDelete = (SimpleAlbumData)mainWindowAlbumList.SelectedItem;

                // If we're deleting the album that has the clipboard images on it, clear the clipboard.
                if ((_clipboardOfPhotos.Count > 0) && (albumToDelete.UID == _clipboardAlbumSrcID))
                {
                    _clipboardOfPhotos.Clear();
                    _clipboardAlbumSrcID = Guid.Empty;
                }

                _bombaDeFotos.removeAlbum(new generic_callback(guiDeleteSelectedAlbum_Callback), albumToDelete.UID);
            }
        }

        /**************************************************************************************************************************
         * Created By: Ryan Causey
         * Created On: 4/3/13
         * Last Edited By: Ryan Causey
         * Last Edited Date: 4/7/13
         **************************************************************************************************************************/
        /// <summary>
        /// Callback for guiDeleteSelectedAlbum. If there is an error something really bad happened.
        /// Notify the user, rebuild the database and consolidate all photographs into a single backup album.
        /// </summary>
        /// <param name="error">Error report from the back end.</param>
        public void guiDeleteSelectedAlbum_Callback(ErrorReport error)
        {
            if (error.reportStatus == ReportStatus.FAILURE || error.reportStatus == ReportStatus.SUCCESS_WITH_WARNINGS)
            {
                //something really bad happened
                //notify the user, rebuild the database and consolidate all photographs into a single backup album
                //showErrorMessage(errorStrings.deleteAlbumFailure); //super temporary
                _bombaDeFotos.rebuildBackendOnFilesystem(new generic_callback(rebuildBackend_Callback));
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

                SimpleAlbumData currentAlbumData = (SimpleAlbumData)mainWindowAlbumList.SelectedItem;

                //call the backend to get all photos in this album.
                _currentAlbumUID = currentAlbumData.UID;
                // Set the app's titlebar to display the app name and the album name
                this.appTitleBarLabel.Content = "PhotoBomber - " + currentAlbumData.albumName;
                _bombaDeFotos.getAllImagesInAlbum(new getAllPhotosInAlbum_callback(guiEnterAlbumView_Callback), _currentAlbumUID);
            }
        }

        /**************************************************************************************************************************
         * Created By: Ryan Causey
         * Created On: 4/5/13
         * Last Edited By: Ryan Causey
         * Last Edited Date: 4/7/13
         **************************************************************************************************************************/
        /// <summary>
        /// Callback for guiEnterAlbumView. Takes the returned ReadOnlyObservableCollection and binds the listView to it
        /// as well as swaps the data template.
        /// </summary>
        /// <param name="error">Error report from backend</param>
        /// <param name="picturesInAlbum">Collection of photos in the album.</param>
        public void guiEnterAlbumView_Callback(ErrorReport error, ReadOnlyObservableCollection<ComplexPhotoData> picturesInAlbum)
        {
            if (error.reportStatus == ReportStatus.FAILURE)
            {
                //show user an error message that retrieving the pictures did not work
                showErrorMessage(errorStrings.getPhotosFailure);
            }
            else
            {
                //going into album
                _isInsideAlbum = true;

                if (error.reportStatus == ReportStatus.SUCCESS_WITH_WARNINGS)
                {
                    //show the user a notification that some pictures are not displayed
                    showErrorMessage(errorStrings.getPhotosWarning);
                }
                //swap data templates and change bindings.
                mainWindowAlbumList.ItemTemplate = this.Resources["ListItemTemplate"] as DataTemplate;
                _listOfPhotos = picturesInAlbum;
                mainWindowAlbumList.ItemsSource = _listOfPhotos;

                _ImageListCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(_listOfPhotos);
                //change the selection mode to Extended
                mainWindowAlbumList.SelectionMode = SelectionMode.Extended;
                //show the return to library view button on the dock
                libraryDockButton.Visibility = Visibility.Visible;
                //show the addPhotos dock button if we are not running an import operation
                if (!_isImporting)
                {
                    addPhotosDockButton.Visibility = Visibility.Visible;
                }
                //hide the add new album button on the dock
                addDockButton.Visibility = Visibility.Collapsed;
                /* Commenting out these as we will swap whole context menu's!
                //temporary fix to prevent an unhandled exception
                viewMenuItemLibraryButton.Visibility = Visibility.Collapsed;
                //hide the delete album button
                deleteMenuItemLibraryButton.Visibility = Visibility.Collapsed;
                //show the delete photo button
                deleteMenuItemPhotoButton.Visibility = Visibility.Visible;
                 */

                if (mainWindowAlbumList.Items.IsEmpty == false)
                {
                    mainWindowAlbumList.SelectedItem = mainWindowAlbumList.Items[0];
                    mainWindowAlbumList.Focus();
                }

                imageSortingButton.Visibility = Visibility.Visible;

                ItemAddOrEditDialogBar.Visibility = Visibility.Collapsed;
            }
        }

        /**************************************************************************************************************************
         * Created By: Ryan Causey
         * Created Date: 4/5/13
         * Last Edited By: Ryan Causey
         * Last Edited Date: 4/8/13
         **************************************************************************************************************************/
        /// <summary>
        /// GUI function to transition back to the library view by changing the data template and item source.
        /// </summary>
        private void guiReturnToLibraryView()
        {
            mainWindowAlbumList.ItemTemplate = this.Resources["LibraryListItemFrontTemplate"] as DataTemplate;
            //refresh the view to make sure we update with new album thumbnails
            populateAlbumView(true);
            // Set the titlebar back to the default: Appname and "my photo library!"
            this.appTitleBarLabel.Content = Settings.AppTitleBarText;
            mainWindowAlbumList.ItemsSource = _listOfAlbums;
            //change the selection mode to single
            mainWindowAlbumList.SelectionMode = SelectionMode.Single;
            //collapse the go back button
            libraryDockButton.Visibility = Visibility.Collapsed;
            //collapse the addPhotos button
            addPhotosDockButton.Visibility = Visibility.Collapsed;
            //show the add album dock button
            addDockButton.Visibility = Visibility.Visible;
            /* Commenting out these as we will swap whole context menu's!
            //temporary fix to prevent an unhandled exception
            viewMenuItemLibraryButton.Visibility = Visibility.Visible;
            //show the delete album button
            deleteMenuItemLibraryButton.Visibility = Visibility.Visible;
            //hide the delete photo button
            deleteMenuItemPhotoButton.Visibility = Visibility.Collapsed;
             */
            //hise the sorting button
            imageSortingButton.Visibility = Visibility.Collapsed;

            ItemAddOrEditDialogBar.Visibility = Visibility.Collapsed;

            //close any open viewImage windows
            if (_view != null)
            {
                _view.Close();
            }

            _currentAlbumUID = Guid.Empty;

            //returning to libraryView
            _isInsideAlbum = false;
            if (mainWindowAlbumList.Items.IsEmpty == false)
            {
                mainWindowAlbumList.SelectedItem = mainWindowAlbumList.Items[0];
                mainWindowAlbumList.Focus();
            }
        }

        /**************************************************************************************************************************
         * Created By: Ryan Causey
         * Created Date: 4/5/13
         * Last Edited By: Ryan Causey
         * Last Edited Date: 4/7/13
         **************************************************************************************************************************/
        /// <summary>
        /// GUI function that shows a file dialogue and then calls the back end to add the selected photographs
        /// </summary>
        private void guiImportPhotos()
        {
            OpenFileDialog photoDialogue = new OpenFileDialog();
            //filter the file types available
            photoDialogue.Filter = promptStrings.addFileDialogueFilter;
            //set the intial directory to the my pictures directory
            photoDialogue.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            //allow multiple photographs to be selected
            photoDialogue.Multiselect = true;

            //show the dialogue
            Nullable<bool> openFilesResult = photoDialogue.ShowDialog();

            //if the user selected files
            if ((bool)openFilesResult)
            {
                //tell the GUI at large we are importing
                _isImporting = true;
                //hide the addPhotosDockButton
                addPhotosDockButton.Visibility = Visibility.Collapsed;
                //show the cancel import dock button
                cancelPhotoImportDockButton.Visibility = Visibility.Visible;

                //need to get the file extensions for the goddamned splicers
                List<string> extensions = new List<string>();

                //GODDAMNED SPLICERS!
                foreach (String s in photoDialogue.FileNames)
                {
                    extensions.Add(System.IO.Path.GetExtension(s));
                }

                //show the progress bar
                progressBar.Visibility = Visibility.Visible;
                progressBar.Minimum = 1;
                progressBar.Maximum = photoDialogue.FileNames.GetLength(0);

                //pass all the files names to a backend function call to start adding the files.
                //fix the function parameters before releasing.
                _bombaDeFotos.addNewImages(new addNewPictures_callback(guiImportPhotos_Callback), new List<string>(photoDialogue.FileNames), extensions, _currentAlbumUID, null, new ProgressChangedEventHandler(guiUpdateProgressBar_Callback), 1);
            }
        }

        /**************************************************************************************************************************
         * Created By: Ryan Causey
         * Created Date: 4/5/13
         * Last Edited By: Ryan Causey
         * Last Edited Date: 4/7/13
         **************************************************************************************************************************/
        /// <summary>
        /// Callback for guiImportPhotos which recieves the error report from the back end.
        /// </summary>
        /// <param name="error">Error report from the back end addNewPictures function.</param>
        public void guiImportPhotos_Callback(ErrorReport error, Guid albumUID)
        {
            //deal with it
            if (error.reportStatus == ReportStatus.FAILURE)
            {
                //shut down all garbage compactors on the detention level
                showErrorMessage(errorStrings.addImageFailure);
                //let the gui know we are done with an import
                _isImporting = false;

                //reset the progress bar.
                progressBar.Visibility = Visibility.Collapsed;
                progressBar.Value = 0;

                //remove the cancel import button
                cancelPhotoImportDockButton.Visibility = Visibility.Collapsed;
                //if we are in an album view
                if (_currentAlbumUID != Guid.Empty)
                {
                    //show the addPhotosButton again
                    addPhotosDockButton.Visibility = Visibility.Visible;
                }

                //if we are in the album we are importing photos too then get all the photos and refresh the view
                if (_currentAlbumUID == albumUID)
                {
                    _bombaDeFotos.getAllImagesInAlbum(new getAllPhotosInAlbum_callback(guiImportPhotosRefreshView_Callback), _currentAlbumUID);
                }
            }
            else
            {
                if (error.reportStatus == ReportStatus.SUCCESS_WITH_WARNINGS)
                {
                    //set phasers to stun
                    //showErrorMessage(errorStrings.addImageWarning);
                }
                //let the gui know we are done with an import
                _isImporting = false;

                //reset the progress bar.
                progressBar.Visibility = Visibility.Collapsed;
                progressBar.Value = 0;

                //remove the cancel import button
                cancelPhotoImportDockButton.Visibility = Visibility.Collapsed;
                //if we are in an album view
                if (_currentAlbumUID != Guid.Empty)
                {
                    //show the addPhotosButton again
                    addPhotosDockButton.Visibility = Visibility.Visible;
                }

                //if we are in the album we are importing photos too then get all the photos and refresh the view
                if (_currentAlbumUID == albumUID)
                {
                    _bombaDeFotos.getAllImagesInAlbum(new getAllPhotosInAlbum_callback(guiImportPhotosRefreshView_Callback), _currentAlbumUID);
                }
            }
        }

        /**************************************************************************************************************************
         * Created By: Ryan Causey
         * Created Date: 4/5/13
         * Last Edited By: Ryan Causey
         * Last Edited Date: 4/7/13
         **************************************************************************************************************************/
        public void guiImportPhotosRefreshView_Callback(ErrorReport error, ReadOnlyObservableCollection<ComplexPhotoData> picturesInAlbum)
        {
            if (error.reportStatus == ReportStatus.FAILURE)
            {
                //show user an error message that retrieving the pictures did not work
                showErrorMessage(errorStrings.getPhotosFailure);
            }
            else
            {
                if (error.reportStatus == ReportStatus.SUCCESS_WITH_WARNINGS)
                {
                    //show the user a notification that some pictures are not displayed
                    showErrorMessage(errorStrings.getPhotosWarning);
                }
                _listOfPhotos = picturesInAlbum;
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
                // Grab the photo obj from the selected gui item.
                ComplexPhotoData photo = (ComplexPhotoData)mainWindowAlbumList.SelectedItem;
                
                // If this is a photo currently in the clipboard, remove it from the clipboard
                // This predicate compares the hash of the photo to the ones in the list
                // Since the clipboard is immutable,
                // we know that there can only be a single photo instance in the clipboard at anytime
                if (_clipboardOfPhotos.Count > 0)
                {
                    _clipboardOfPhotos.RemoveAll(p => p.hash == photo.hash);
                }

                _bombaDeFotos.removeImageFromAlbum(
                    new generic_callback(guiDeleteSelectedPhoto_Callback),
                    photo.idInAlbum,
                    _currentAlbumUID);
            }
        }

        /**************************************************************************************************************************
         * Created By: Bill Sanders
         * Created Date: 4/6/13
         * Last Edited By:
         * Last Edited Date:
         **************************************************************************************************************************/
        /// <summary>
        /// GUI function to rename the selected Album
        /// </summary>
        private void guiRenameSelectedAlbum(String albumName)
        {
            if (mainWindowAlbumList.SelectedItem != null)
            {
                _bombaDeFotos.setAlbumName(new generic_callback(guiRenameSelectedAlbum_Callback), ((SimpleAlbumData)mainWindowAlbumList.SelectedItem).UID, albumName);
            }
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        /* Created By: Bill Sanders
         * Created Date: 4/6/13
         * Last Edited By: Ryan Causey
         * Last Edited Date: 4/7/13
         */
        /// <summary>
        /// Callback for guiRenameSelectedAlbum. Just shows an error message if there is one.
        /// </summary>
        /// <param name="error">Error report from the back end.</param>
        public void guiRenameSelectedAlbum_Callback(ErrorReport error)
        {
            if (error.reportStatus == ReportStatus.FAILURE)
            {
                showErrorMessage(errorStrings.renameAlbumFailure);
            }
        }

        /**************************************************************************************************************************
         * Created By: Bill Sanders
         * Created Date: 4/6/13
         * Last Edited By:
         * Last Edited Date:
         **************************************************************************************************************************/
        /// <summary>
        /// GUI function to rename the selected Photo
        /// </summary>
        private void guiRenameSelectedPhoto(string newName)
        {
            if (mainWindowAlbumList.SelectedItem != null)
            {
                _bombaDeFotos.setImageName(new generic_callback(guiRenameSelectedPhoto_Callback), _currentAlbumUID, ((ComplexPhotoData)mainWindowAlbumList.SelectedItem).idInAlbum, newName);
            }
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        /* Created By: Bill Sanders
         * Created Date: 4/6/13
         * Last Edited By: Ryan Causey
         * Last Edited Date: 4/7/13
         */
        /// <summary>
        /// Callback for guiRenameSelectedPhoto. Just shows an error message if there is one.
        /// </summary>
        /// <param name="error">Error report from the back end.</param>
        public void guiRenameSelectedPhoto_Callback(ErrorReport error)
        {
            if (error.reportStatus == ReportStatus.FAILURE)
            {
                showErrorMessage(errorStrings.renamePhotoFailure);
            }
        }

        /* Created By: Ryan Causey
         * Created Date: 4/5/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// Callback for guiDeleteSelectedPhoto. Just shows an error message if there is one.
        /// </summary>
        /// <param name="error">Error report from the back end.</param>
        public void guiDeleteSelectedPhoto_Callback(ErrorReport error)
        {
            if (error.reportStatus == ReportStatus.FAILURE)
            {
                showErrorMessage(errorStrings.deletePhotoFailure);
            }
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/6/13
         * Last Edited By: Ryan Causey
         * Last Edited Date: 4/7/13
         */
        /// <summary>
        /// GUI function call to cancel the import of pictures.
        /// </summary>
        private void guiCancelImport()
        {
            ErrorReport error = _bombaDeFotos.cancelAddNewImagesThread();

            //if theres an error, show the error message, otherwise hide the cancel button.
            if (error.reportStatus == ReportStatus.FAILURE)
            {
                showErrorMessage(errorStrings.stopImportFailure);
            }
            else
            {
                cancelPhotoImportDockButton.Visibility = Visibility.Collapsed;
            }
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/6/13
         * Last Edited By: Ryan Causey
         * Last Edited Date: 4/8/13
         */
        /// <summary>
        /// GUI function that will instantiate a viewImage window and give it the information to display the image.
        /// </summary>
        /// <param name="slideShowStart">True if you want to skip viewing the photo and go directly into a slideshow, defaults to false</param>
        private void guiViewPicture(Boolean slideShowStart = false)
        {
            //close the previous view
            if (_view != null)
            {
                _view.Close();
            }

            // determine if we're in the library view...
            if (_currentAlbumUID == Guid.Empty)
            {
                ErrorReport errorReport = new ErrorReport();

                // transition to the album the user selected for a slideshow
                guiEnterAlbumView();

                // if there are pictures in the album
                if (_listOfPhotos.Count > 0)
                {
                    // start the slideshow at picture[0]
                    _view = new ViewImage(_ImageListCollectionView, ((ComplexPhotoData)mainWindowAlbumList.Items[0]).UID, _currentAlbumUID, new greyScaleConverterDelegate(guiConvertToGreyscale), slideShowStart);
                }
                else
                {
                    // If the album is empty, transition back to the library.
                    guiReturnToLibraryView();
                }
            }
            // ... or the album view
            else
            {
                try
                {
                    // start the slideshow at the selected photo.
                    _view = new ViewImage(_ImageListCollectionView, ((ComplexPhotoData)mainWindowAlbumList.SelectedItem).UID, _currentAlbumUID, new greyScaleConverterDelegate(guiConvertToGreyscale), slideShowStart);
                }
                catch (NullReferenceException)
                {
                    return;
                }
            }

            // finally, show the form, if there's anything to show.
            if (_listOfPhotos.Count > 0)
            {
                _view.Show();
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
         * Created By: Alejandro Sosa
         * Created Date:
        **************************************************************************************************************************/
        /*
        private void dockHitBox_MouseLeave(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (dockHitBox.IsMouseDirectlyOver == true && isMousenDock == false)
            {
                try
                {
                    Storyboard dockDisappearAnimation = this.FindResource("DockDisappear") as Storyboard;
                    dockDisappearAnimation.Begin();
                }
                catch (Exception)
                {
                    ;
                }
            }
        }
         */


        /**************************************************************************************************************************
         * Created By: Alejandro Sosa
         * Created Date:
        **************************************************************************************************************************/
        private void dockHitBox_MouseLeave(object sender, MouseEventArgs e)
        {
            isMouseInDockHitBox = false;
            playDockDisappearAnimation();


        }


        /**************************************************************************************************************************
         * Created By: Alejandro Sosa
         * Created Date:
        **************************************************************************************************************************/
        private void dockHitBox_MouseEnter(object sender, MouseEventArgs e)
        {
            isMouseInDockHitBox = true;

            if (mainWindowDock.Visibility == System.Windows.Visibility.Visible)
                return;
            
            playDockAppearAnimation();
        }

        /**************************************************************************************************************************
         * Created By: Alejandro Sosa
         * Created Date:
        **************************************************************************************************************************/
        private void DockMouseEnter_Handler(object sender, MouseEventArgs e)
        {
            isMouseInDock = true;

            if (mainWindowDock.Visibility == System.Windows.Visibility.Visible)
                return;
            
            playDockAppearAnimation();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DockMouseLeave_Handler(object sender, MouseEventArgs e)
        {
            isMouseInDock = false;
            playDockDisappearAnimation();
        }

        /// <summary>
        /// 
        /// </summary>
        private void playDockAppearAnimation()
        {
            try
            {
                Storyboard dockAppearAnimation = this.FindResource("DockAppear") as Storyboard;
                dockAppearAnimation.Begin();

            }
            catch (Exception) { }
        }

        /// <summary>
        /// 
        /// </summary>
        private void playDockDisappearAnimation()
        {
            try
            {
                Storyboard dockDisappearAnimation = this.FindResource("DockDisappear") as Storyboard;
                dockDisappearAnimation.Begin();
            }
            catch (Exception) { }
        }


        /**************************************************************************************************************************
         * Created By: Alejandro Sosa
         * Created Date:
        **************************************************************************************************************************/
        private void mainWindowContextMenu_LostMouseCapture(object sender, MouseEventArgs e)
        {
            libraryContextMenu.IsOpen = false;
        }


        /**************************************************************************************************************************
        **************************************************************************************************************************/
        /*
         * Created By: Alejandro Sosa
         * Last Edited By: Ryan Causey
         * Last Edited Date: 4/8/13
         */
        /// <summary>
        /// Event handler for the exit button click event.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event args</param>
        private void exitDockButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        /**************************************************************************************************************************
         * Commenting out because no longer needed.
         * Created By: Alejandro Sosa
         * Edited Last By: Ryan Causey
         * Edited Last Date: 4/6/13
         **************************************************************************************************************************/
        /*
        private void mainWindowAlbumList_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //if we aren't in an album
            if (currentAlbumUID == -1)
            {
                //show the context menu for the library
                libraryContextMenu.IsOpen = true;
            }
            //if we are in an album
            else
            {
                //show the context menu for the album.
                AlbumContextMenu.IsOpen = true;
            }
        }
        */
        /**************************************************************************************************************************
         * Created By: Ryan Causey
         * Created On: 4/3/13
         * Event Handler for the large + button on the dock. Shows the add new album box.
         * Last Edited By:
         * Last Edited Date:
         **************************************************************************************************************************/
        private void addAlbumDockButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindowAlbumList.SelectedItem = null;
            showAddAlbumBox();
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        private void Titlebar_DoubleClick(object sender, MouseButtonEventArgs e)
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
            if (this.WindowState == WindowState.Maximized)
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
         * Edited Last Date: 4/8/13
         **************************************************************************************************************************/
        private void showAddAlbumBox()
        {
            ItemAddOrEditDialogBar.Visibility = Visibility.Visible;

            acceptAddToolbarButton.Visibility = Visibility.Visible;
            cancelAddToolbarButton.Visibility = Visibility.Visible;

            if (_currentAlbumUID != Guid.Empty)
            {
                // Hide the album stuff, if its displayed.
                NameTextBlock.Visibility = Visibility.Collapsed;
                nameTextBox.Visibility = Visibility.Collapsed;
                nameTextBox.Text = string.Empty;

                // Now show the photo stuff.
                photoNameTextBlock.Visibility = Visibility.Visible;
                photoNameTextBox.Visibility = Visibility.Visible;
                photoNameTextBox.Text = string.Empty;
                commentTextBlock.Visibility = Visibility.Visible;
                commentTextBox.Visibility = Visibility.Visible;
                commentTextBox.Text = ((ComplexPhotoData)mainWindowAlbumList.SelectedItem).caption;
                Keyboard.Focus(commentTextBox);
            }
            else
            {
                // Hide the photo details, if displayed
                photoNameTextBlock.Visibility = Visibility.Collapsed;
                photoNameTextBox.Visibility = Visibility.Collapsed;
                photoNameTextBox.Text = string.Empty;
                commentTextBlock.Visibility = Visibility.Collapsed;
                commentTextBox.Visibility = Visibility.Collapsed;
                commentTextBox.Text = string.Empty;


                // Now show the album stuff.
                NameTextBlock.Visibility = Visibility.Visible;
                nameTextBox.Visibility = Visibility.Visible;
                Keyboard.Focus(nameTextBox);
            }
        }

        /**************************************************************************************************************************
         * Created By: Alejandro Sosa
         * Edited Last By: Ryan Causey
         * Edited Last Date: 4/8/13
         **************************************************************************************************************************/
        private void hideAddAlbumBox()
        {
            ItemAddOrEditDialogBar.Visibility = Visibility.Collapsed;

            NameTextBlock.Visibility = Visibility.Collapsed;
            nameTextBox.Visibility = Visibility.Collapsed;

            acceptAddToolbarButton.Visibility = Visibility.Collapsed;
            cancelAddToolbarButton.Visibility = Visibility.Collapsed;

            commentTextBlock.Visibility = Visibility.Collapsed;
            commentTextBox.Visibility = Visibility.Collapsed;

            photoNameTextBox.Visibility = Visibility.Collapsed;
            photoNameTextBlock.Visibility = Visibility.Collapsed;
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



        //thumbar functions for resizing the window
        #region thumbarResizefunctions

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

        #endregion


        /**************************************************************************************************************************
        **************************************************************************************************************************/
        private void aboutButtonPressed_eventHandler(object sender, RoutedEventArgs e)
        {
            if (PhotoBomberMenu.IsSubmenuOpen == true)
            {
                PhotoBomberMenu.IsSubmenuOpen = false;
            }
            else
            {
                PhotoBomberMenu.IsSubmenuOpen = true;
            }
        }

        private void aboutMenuItemPressed_eventHandler(object sender, RoutedEventArgs e)
        {
            //if this window is already open, close it.

            _photoBomberAboutWindow = new AboutWindow();

            _photoBomberAboutWindow.Show();
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
         * Created by: Ryan Causey
         * Created on: 4/3/13
         * Event handler for when the user clicks the checkbox for album names, photo names, and captions
         * they specified in the text box.
         * Last Edited By: Ryan Causey
         * Last Edited Date: 4/8/13
         **************************************************************************************************************************/
        private void acceptAddToolBarButton_Click(object sender, RoutedEventArgs e)
        {
            //if we aren't in the library view
            if (_currentAlbumUID != Guid.Empty)
            {
                //if the photo name text box is empty, but the caption text box is not
                if (photoNameTextBox.Text == "")
                {
                    //only try to validate and add the photo caption
                    guiValidateCaptionContent();
                }
                //else the photo name is not blank
                else
                {
                    //so validate the photo name, which will also call caption validation if needed
                    guiValidatePhotoName();
                }
            }
            //else we are in the library view so validate the album name.
            else
            {
                guiValidateAlbumName();
            }
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
        /* commenting out as we do not need this -Ryan Causey
        private void PopupMouseClick_Handler(object sender, MouseButtonEventArgs e)
        {
            libraryContextMenu.IsOpen = false;
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        private void handleNameErrorPopup(bool showIt, string errorMessage)
        {
            errorBalloon.Content = errorMessage;
            invalidInputPopup.IsOpen = showIt;
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/8/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// Handle the showing or hiding of the error message popup for the photo name box.
        /// </summary>
        /// <param name="show">Boolean saying whether to open or close the popup. True = open, False = close</param>
        /// <param name="errorMessage">The error string to display in the error popup.</param>
        private void handlePhotoNameErrorPopup(bool show, String errorMessage)
        {
            photoErrorBalloon.Content = errorMessage;
            invalidPhotoNamePopup.IsOpen = show;
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/8/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// Handle the showing or hiding of the error message popup for the comment box.
        /// </summary>
        /// <param name="show">Boolean saying whether to open or close the popup. True = open, False = close</param>
        /// <param name="errorMessage">The error string to display in the error popup.</param>
        private void handleCommentErrorPopup(bool show, String errorMessage)
        {
            invalidCommentPopup.IsOpen = show;
            commentErrorBalloon.Content = errorMessage;
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
         * Last Edited By: Ryan Causey
         * Last Edited Date: 4/6/13
         **************************************************************************************************************************/
        /// <summary>
        /// Event handler for the click event on the delete context menu button for the album view.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event args</param>
        private void deleteMenuItemAlbumButton_Click(object sender, RoutedEventArgs e)
        {
            //call guideletephoto function here.
            guiDeleteSelectedPhoto();
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/6/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// Handler for the cancel import photo dock button.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event args</param>
        private void cancelPhotoImportDockButton_Click(object sender, RoutedEventArgs e)
        {
            guiCancelImport();
        }

        /*Commenting out because not longer needed
         * Created By: Ryan Causey
         * Created Date: 4/6/13
         * Last Edited By:
         * Last Edited Date:
         */
        /*
        private void albumContextMenuPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            AlbumContextMenu.IsOpen = false;
        }
        */

        private void renameMenuItemLibraryButton_Click(object sender, RoutedEventArgs e)
        {
            showAddAlbumBox();
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/6/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// Handler for the view image button. Call the GUI function to open the image view.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event args</param>
        private void viewMenuItemAlbumButton_Click(object sender, RoutedEventArgs e)
        {
            guiViewPicture();
        }

        private void addMenuItemLibraryButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainWindowAlbumList.SelectedItem != null)
            {
                guiEnterAlbumView();
                guiImportPhotos();
            }
        }

        private void addMenuItemAlbumButton_Click(object sender, RoutedEventArgs e)
        {
            guiImportPhotos();
        }

        private void renameMenuItemAlbumButton_Click(object sender, RoutedEventArgs e)
        {
            showAddAlbumBox();
        }

        private void copyMenuItemLibraryButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainWindowAlbumList.SelectedItem == null)
            {
                // you gotta click on something if you want to copy it...
            }
            else
            {
                guiPasteClipboardPhotosToAlbum();
            }
        }

        private void copyMenuItemAlbumButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainWindowAlbumList.SelectedItem == null)
            {
                // you gotta click on something if you want to copy it...
            }
            else
            {
                guiCopySelectedPhotosToClipboard();
            }
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/7/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// handler for the library view tiles to bring up the context menu only when the right click
        /// occurs on an album.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void libraryItemBackGrid_PreviewRightMouseUp(object sender, MouseButtonEventArgs e)
        {
            libraryContextMenu.IsOpen = true;
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/7/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// Handler to hide the context menu after a left mouse click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainWindow_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            closePopups();
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/7/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// Handler to hide the context menu after a right mouse click not on any item that the
        /// context menu should open over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainWindow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            closePopups();
        }

        /*
         *Created By Alejandro Sosa
         *function to close all open popups (such as when main window is clicked)
         */
        private void closePopups()
        {
            libraryContextMenu.IsOpen = false;
            AlbumContextMenu.IsOpen = false;
            imageSortingMenu.IsSubmenuOpen = false;

        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/7/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// Handler for the album view tiles, front face, to show the album context menu only if the right
        /// click occurs on a photo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void albumItemFrontGrid_PreviewRightMouseUp(object sender, MouseButtonEventArgs e)
        {
            closePopups();
            AlbumContextMenu.IsOpen = true;
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/7/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// Handler for the album view tiles, rear face, to show the album context menu only if the right
        /// click occurs on a photo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemBackSideContainer_PreviewRightMouseUp(object sender, MouseButtonEventArgs e)
        {
            closePopups();
            AlbumContextMenu.IsOpen = true;
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/8/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// Handler for the slideshow context menu item on the album view context menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void slideshowMenuItemAlbumButton_Click(object sender, RoutedEventArgs e)
        {
            //call with true to directly begin slideshow.
            guiViewPicture(true);
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/8/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// Handler for the slideshow context menu item on the library view context menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void slideshowMenuItemLibraryButton_Click(object sender, RoutedEventArgs e)
        {
            //call with true to directly begin slideshow.
            guiViewPicture(true);
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/8/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// Handler for the selectionChanged event to ensure the context menu closes when selection is changed by a means
        /// other than the mouse.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainWindowAlbumList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            closePopups();
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/8/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// Intercept any closing event and gracefully shut down the program.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainWindow_Closing(object sender, CancelEventArgs e)
        {
            //if we are importing we need to handle stopping the thread.
            if (_isImporting)
            {
                ErrorReport error = _bombaDeFotos.cancelAddNewImagesThread();
                //if the thread failed to be stopped.
                if (error.reportStatus == ReportStatus.FAILURE)
                {
                    showErrorMessage(errorStrings.stopImportFailure);
                }
                //else we are all good to close
                else
                {
                    // Make sure the backend saves cleanly
                    _bombaDeFotos.saveAlbumsXML(null);
                    _bombaDeFotos.saveImagesXML(null);

                    //add this line to make sure the app properly closes now that we've screwed with the
                    //magic wizardry of App.xaml.cs to ensure only one instance of the application can launch.
                    App.Current.Shutdown();
                }
            }
            else
            {
                // Make sure the backend saves cleanly
                _bombaDeFotos.saveAlbumsXML(null);
                _bombaDeFotos.saveImagesXML(null);

                //add this line to make sure the app properly closes now that we've screwed with the
                //magic wizardry of App.xaml.cs to ensure only one instance of the application can launch.
                App.Current.Shutdown();
            }
        }

        private void mainWindowAlbumList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            mainWindowListItemActivation();
        }

        private void mainWindowAlbumList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                mainWindowListItemActivation();
            }
        }

        private void mainWindowListItemActivation()
        {
            if (_isInsideAlbum == false)
            {
                guiEnterAlbumView();
            }
            else
            {
                guiViewPicture();
            }
        }

        private void dockSizeChanged_EventHandler(object sender, SizeChangedEventArgs e)
        {
            //if (mainWindowDock.Height != 48)
            //{
            //    return;
            //}
        }


        /**************************************************************************************************************************
        **************************************************************************************************************************/

        private void closePopupsAndMenus()
        {
            libraryContextMenu.IsOpen = false;
            AlbumContextMenu.IsOpen = false;
            imageSortingMenu.IsSubmenuOpen = false;
        }


        //this Function region deals with image sorting
        #region sortingFunctionRegion

        private void clearSortingCheckBoxes()
        {
            dateTakenMenuItem.IsChecked = false;
            nameMenuItem.IsChecked = false;
            dateAddedMenuItem.IsChecked = false;
            //equipmentManufacturerMenuItem.IsChecked = false;
            equipmentModelMenuItem.IsChecked = false;
        }

        

        private void SortImageList()
        {
            if (_ImageListCollectionView.SortDescriptions == null)
            {
                return;
            }
            _ImageListCollectionView.SortDescriptions.Clear();

            String sortByThis;

            if (nameMenuItem.IsChecked == true)
            {
                sortByThis = "name";
            }
            else if (dateTakenMenuItem.IsChecked == true)
            {
                sortByThis = "takenDate";
            }
            else if (dateAddedMenuItem.IsChecked == true)
            {
                sortByThis = "addedDate";
            }
            //else if (equipmentManufacturerMenuItem.IsChecked == true)
            //{
            //    sortByThis = "equipmentManufacturer";
            //}
            else if (equipmentModelMenuItem.IsChecked == true)
            {
                sortByThis = "equipmentModel";
            }
            else
            {
                return;
            }


            switch (ascendingMenuItem.IsChecked)
            {
                case true:
                    _ImageListCollectionView.SortDescriptions.Add(new SortDescription(sortByThis, ListSortDirection.Ascending));
                    break;
                case false:
                    _ImageListCollectionView.SortDescriptions.Add(new SortDescription(sortByThis, ListSortDirection.Descending));
                    break;
            }
        }

        private void sortingDockMenu_Click(object sender, RoutedEventArgs e)
        {
            if (imageSortingMenu.IsSubmenuOpen == false)
            {
                imageSortingMenu.IsSubmenuOpen = true;
            }
            else
            {
                imageSortingMenu.IsSubmenuOpen = false;
            }

        }

        //private void extensionMenuItem_Click(object sender, RoutedEventArgs e)
        //{
        //    if (extensionMenuItem.IsChecked == false)
        //    {
        //        clearSortingCheckBoxes();
        //        extensionMenuItem.IsChecked = true;
        //        SortImageList();
        //    }
        //}

        private void nameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (nameMenuItem.IsChecked == true)
            {
                clearSortingCheckBoxes();
                
                SortImageList();
            }
            nameMenuItem.IsChecked = true;
        }


        private void dateAddedMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (dateAddedMenuItem.IsChecked == true)
            {
                clearSortingCheckBoxes();
                
                SortImageList();
            }
            dateAddedMenuItem.IsChecked = true;
        }


        private void dateTakenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (dateTakenMenuItem.IsChecked == true)
            {
                clearSortingCheckBoxes();
                
                SortImageList();
            }
            dateTakenMenuItem.IsChecked = true;
        }

        //private void equipmentManufacturerMenuItem_Click(object sender, RoutedEventArgs e)
        //{
        //    if (equipmentManufacturerMenuItem.IsChecked == true)
        //    {
        //        clearSortingCheckBoxes();
                
        //        SortImageList();
        //    }
        //    equipmentManufacturerMenuItem.IsChecked = true;
        //}

        private void equipmentModelMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (equipmentModelMenuItem.IsChecked == true)
            {
                clearSortingCheckBoxes();

                SortImageList();
            }
                equipmentModelMenuItem.IsChecked = true;
        }



        private void ascendingMenuItem_CheckToggled(object sender, RoutedEventArgs e)
        {
            SortImageList();
        }

        #endregion


        //This function region deals with choosing themes
        #region ThemeRelatedFunctions

        //getCurrentPhotoBomberTheme
        private void gui_getCurrentTheme()
        {
            String storedTheme;
            _bombaDeFotos.getCurrentTheme(out storedTheme);
            clearThemecheckboxes();
            var program = App.Current as App;

            try
            {
                program.setTheme(storedTheme);
            }
            catch (Exception)
            {
                //showErrorMessage("We're going to DIIEEEEEE-bleh");
                program.setTheme("/Themes/ExpressionDark.xaml");
            }
        }

        private void gui_setTheme(String appliedTheme)
        {
            var program = App.Current as App;

            program.setTheme(appliedTheme);

            ReportStatus success = _bombaDeFotos.setCurrentTheme(appliedTheme);

            if (success != ReportStatus.SUCCESS)
            {
                //showErrorMessage("We're going to DIIEEEEEE-bleh");
            }
        }

        private void clearThemecheckboxes()
        {
            bureauBlackThemeMenuItem.IsChecked = false;
            bureauBlueThemeMenuItem.IsChecked = false;
            expressionDarkThemeMenuItem.IsChecked = false;
            expressionLightThemeMenuItem.IsChecked = false;
            //shinyBlueThemeMenuItem.IsChecked = false;
            //shinyRedThemeMenuItem.IsChecked = false;
            whistlerBlueThemeMenuItem.IsChecked = false;
        }



        private void bureauBlackThemeMenuItem_CheckToggle(object sender, RoutedEventArgs e)
        {
            String currentheme = "/Themes/BureauBlack.xaml";
            clearThemecheckboxes();
            bureauBlackThemeMenuItem.IsChecked = true;

            //ThemeSelector.SetCurrentThemeDictionary(this, new Uri("/Themes/BureauBlack.xaml", UriKind.Relative));


            gui_setTheme(currentheme);
        }

        private void bureauBlueThemeMenuItem_CheckToggle(object sender, RoutedEventArgs e)
        {
            String currentheme = "/Themes/BureauBlue.xaml";
            clearThemecheckboxes();
            bureauBlueThemeMenuItem.IsChecked = true;

            //ThemeSelector.SetCurrentThemeDictionary(this, new Uri("/Themes/BureauBlue.xaml", UriKind.Relative)); 

            gui_setTheme(currentheme);
        }

        private void expressionDarkThemeMenuItem_CheckToggle(object sender, RoutedEventArgs e)
        {
            String currentheme = "/Themes/ExpressionDark.xaml";
            clearThemecheckboxes();
            expressionDarkThemeMenuItem.IsChecked = true;

            //ThemeSelector.SetCurrentThemeDictionary(this, new Uri("/Themes/ExpressionDark.xaml", UriKind.Relative));  

            gui_setTheme(currentheme);
        }

        private void expressionLightThemeMenuItem_CheckToggle(object sender, RoutedEventArgs e)
        {
            String currentheme = "/Themes/ExpressionLight.xaml";
            clearThemecheckboxes();
            expressionLightThemeMenuItem.IsChecked = true;

            //ThemeSelector.SetCurrentThemeDictionary(this, new Uri("/Themes/ExpressionLight.xaml", UriKind.Relative));  

            gui_setTheme(currentheme);
        }

        private void whistlerBlueThemeMenuItem_CheckToggle(object sender, RoutedEventArgs e)
        {
            String currentheme = "/Themes/WhistlerBlue.xaml";
            clearThemecheckboxes();
            whistlerBlueThemeMenuItem.IsChecked = true;

            //ThemeSelector.SetCurrentThemeDictionary(this, new Uri("/Themes/WhistlerBlue.xaml", UriKind.Relative)); 

            gui_setTheme(currentheme);
        }

        //private void shinyBlueThemeMenuItem_CheckToggle(object sender, RoutedEventArgs e)
        //{
        //    clearThemecheckboxes();
        //    shinyBlueThemeMenuItem.IsChecked = true;

        //    //ThemeSelector.SetCurrentThemeDictionary(this, new Uri("/Themes/ShinyBlue.xaml", UriKind.Relative)); 

        //    var program = App.Current as App;

        //    program.setTheme("/Themes/ShinyBlue.xaml");
        //}

        //private void shinyRedThemeMenuItem_CheckToggle(object sender, RoutedEventArgs e)
        //{
        //    clearThemecheckboxes();
        //    shinyRedThemeMenuItem.IsChecked = true;

        //    //ThemeSelector.SetCurrentThemeDictionary(this, new Uri("/Themes/ShinyRed.xaml", UriKind.Relative));     

        //    var program = App.Current as App;

        //    program.setTheme("/Themes/ShinyRed.xaml");
        //}



        #endregion



        #region GrayscaleRElatedFunctions

        public void guiConvertToGreyscale(addNewPictures_callback guiGrayscaleCallback, ComplexPhotoData desiredImage, Guid albumGuid)
        {
            _bombaDeFotos.addImageAsGrayscale(guiGrayscaleCallback, albumGuid, desiredImage.fullPath);
        }



 



        #endregion





    }

}





