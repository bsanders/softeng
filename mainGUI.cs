using System;
using System.Configuration;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SoftwareEng
{
    public partial class mainGUI : Form
    {
        //--didn't know what to call it, so I named it the literal spanish translation
        public SoftwareEng.PhotoBomb bombaDeFotos;

        //--stores the albumImageList index of the default image for albums
        private const short defaultAlbumImageListIndex = 0;

        //--used to specify the UID of the "add new album" icon
        private const int addAlbumID = 0;

        //--first selected item in a List View will have an index of zero, 
        //--this is necessary even if  listView multiselect is disabled
        private const int firstListViewItemIndex = 0;

        //--second subitem of first ListView item will be album uid
        private const int listViewSubItemUidIndex = 1;

        //--A more stable storage for the ID of the user album instead
        //-- of relying on a form's selected items collection
        private int albumChosenbyUser;

        private string photoNameTempBackup = "";

        //--this object was made a class member to alleviate issues with 
        //--different functions modifying
        private progressForm pictureImportProgress;


        //--for showing this class's errors 
        private ErrorDialogForm errorBox;


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: none
        * return type: none
        * purpose: creates a new instance of mainGUI
        *********************************************************************************************/
        public mainGUI()
        {
            InitializeComponent();
            
            //for now the gui will determine filepaths(set to same folder as exe) in case it is ever made a user choice
            String libraryPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    Properties.Settings.Default.PhotoLibraryName);
                
            bombaDeFotos = new PhotoBomb();
            bombaDeFotos.init(guiConstructorCallback,
                Properties.Settings.Default.AlbumXMLFile,
                Properties.Settings.Default.PhotoXMLFile,
                libraryPath);


            //ensures that the album list is visible 
            albumListView.BringToFront();

            //true tells the function to refresh list
            populateAlbumView(true);

            
            createNewAlbumToolStripMenuItem.Enabled = true;
            aboutToolStripMenuItem.Enabled = true;

            albumChosenbyUser = addAlbumID;

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
                if (Directory.Exists(Properties.Settings.Default.PhotoLibraryBackupName))
                {
                    Directory.Delete(Properties.Settings.Default.PhotoLibraryBackupName, true);
                    bombaDeFotos.rebuildBackendOnFilesystem(new generic_callback(guiGenericErrorFunction));
                }
            }
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: an object of ErrorReport which will be used to check if backend was successful
        * return type: void
        * purpose: simply calls back end
        *********************************************************************************************/
        //RM: why is this here???
        private void guiConstructorCallback(ErrorReport status)
        {
            if (status.reportID != ErrorReport.SUCCESS)
            {
                bombaDeFotos.rebuildBackendOnFilesystem(new generic_callback(guiGenericErrorFunction));
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

            if (refreshView == true)
            {
                bombaDeFotos.getAllUserAlbumNames(new getAllUserAlbumNames_callback(guiAlbumsRetrieved));
            }
        }


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: an object of ErrorReport which will be used to check if backend was successful,
        *   and a list <type is SimpleAlbumData> containing data to identify all albums requested
        * return type: void
        * purpose: list of albums returned from the backend
        *********************************************************************************************/
        public void guiAlbumsRetrieved(ErrorReport status, List<SimpleAlbumData> albumsRetrieved)
        {
            if (status.reportID == ErrorReport.SUCCESS)
            {
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
        }

        /************************************************************
        * temporary function to show an error. Can eventually be 
         * replaced by a function that shows a custom form.
         * -EDIT prophecy came true
        ************************************************************/
        private void showError(string theErrorMessage)
        {
            errorBox = new ErrorDialogForm(theErrorMessage);

            errorBox.ShowDialog();

            //MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK);
        }



        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: windows defined
        * return type: void
        * purpose: calls albumListActivation() which does the real work (to make sure different event
        *   handlers did the same thing)
        *********************************************************************************************/
        private void albumListView_ItemActivate(object sender, EventArgs e)
        {
            albumListActivation();
        }


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: none
        * return type: void
        * purpose: does the real work to handle when an item in albumListView is activated
        *********************************************************************************************/
        private void albumListActivation()
        {
            //check to make sure there actually are selected items
            if (albumListView.SelectedItems.Count > 0)
            {
                int selectedAlbumID = Convert.ToInt32(albumListView.SelectedItems[firstListViewItemIndex].SubItems[listViewSubItemUidIndex].Text);
                
                // check to make sure selected item isn't the "add album" icon
                if (selectedAlbumID > addAlbumID)
                {
                    albumChosenbyUser = selectedAlbumID;

                    createNewAlbumToolStripMenuItem.Enabled = false;

                    //true tells the function to refresh list
                    guiPopulatePhotoListView(true);

                    albumChosenbyUser = selectedAlbumID;
                }
                else
                {
                    guiShowCreateAlbumWindow();
                }
            }
        }


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: a bool that determines whether the photosList is clear
        * return type: void
        * purpose: clears and refreshes the list of photos
        *********************************************************************************************/
        public void guiPopulatePhotoListView(bool refreshView)
        {
            photoListView.Clear();

            if (refreshView == true)
            {
                bombaDeFotos.getAllPhotosInAlbum(new getAllPhotosInAlbum_callback(guiPhotosInAlbumRetrieved), albumChosenbyUser);
            }
        }


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: ErrorReport, and a list<SimplePhotoData> containing all photo data
        * return type: void
        * purpose: this function decides how the gui will respond when the back end finishes
        *********************************************************************************************/
        public void guiPhotosInAlbumRetrieved(ErrorReport status, List<SimplePhotoData> albumContents)
        {
            photoListView.BringToFront();
            addPhotosToExistingAlbumToolStripMenuItem.Enabled = true;
            mainFormBackbutton.Enabled = true;

            if (status.reportID == ErrorReport.SUCCESS)
            {
                photoListView.BeginUpdate();
                
                //--if switching to another kind of "view", from here to the end of catch block
                ListViewItem itemHolder = null;
                ListViewItem.ListViewSubItem[] itemHolderSubitems;

                foreach (SimplePhotoData singlePhoto in albumContents)
                {
                    try
                    {
                        itemHolderSubitems = new ListViewItem.ListViewSubItem[]{
                            new ListViewItem.ListViewSubItem(itemHolder, singlePhoto.picturesNameInAlbum),
                            new ListViewItem.ListViewSubItem(itemHolder, singlePhoto.GUID),
                            new ListViewItem.ListViewSubItem(itemHolder, singlePhoto.UID.ToString() )
                            };

                        itemHolder = new ListViewItem(itemHolderSubitems, defaultAlbumImageListIndex);
                        photoListView.Items.Add(itemHolder);
                    }
                    catch (Exception)
                    {
                        showError("Error: Album missing.");
                    }
                    //end of catch
                    
                }
                photoListView.EndUpdate();
            }
            else if (status.reportID == ErrorReport.FAILURE)
            {
                showError("Error: Album missing.");
            }
        }


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: none
        * return type: void
        * purpose: 
        *********************************************************************************************/
        private void guiShowCreateAlbumWindow()
        {
            albumListView.SelectedItems.Clear();

            addNewAlbum createAlbumDialog = new addNewAlbum(this);

            createAlbumDialog.ShowDialog();

            if (createAlbumDialog.DialogResult != DialogResult.Cancel)
            {
                //true tells the function to refresh list 
                populateAlbumView(true);
            }
        }


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: void
        * purpose: 
        *********************************************************************************************/
        public void guiCheckAlbumNameIsUnique(string userInput, generic_callback addNewAlbumDelegate)
        {
            bombaDeFotos.checkIfAlbumNameIsUnique(addNewAlbumDelegate, userInput);
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: void
        * purpose: 
        *********************************************************************************************/
        public void guiCreateThisAlbum(string userInput, generic_callback addNewAlbumDelegate)
        {
            SimpleAlbumData theNewAlbum = new SimpleAlbumData();

            theNewAlbum.albumName = userInput;

            bombaDeFotos.addNewAlbum(addNewAlbumDelegate, theNewAlbum);
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: int that specifies the UID of an album to add pictures to
        * return type: void
        * purpose: 
        *********************************************************************************************/
        private void addPicturesToAlbum(int albumId)
        {
                if (photoOpenFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                List<string> fullFileNames = new List<string>();

                List<string> photoExtensions = new List<string>();
                //--fills up both lists that will be passed to bombaDeFotos.addNewPictures
                foreach (string picFile in photoOpenFileDialog.FileNames)
                {
                    fullFileNames.Add(picFile);

                    photoExtensions.Add(".jpg");
                }

                progressForm photoImportProgress;

                pictureImportProgress = new progressForm(photoOpenFileDialog.FileNames.Length, bombaDeFotos);

                ComplexPhotoData newPicture = new ComplexPhotoData();

                bombaDeFotos.addNewPictures(guiPictureAdded,
                    fullFileNames,
                    photoExtensions,
                    albumChosenbyUser,
                    null,
                    new ProgressChangedEventHandler(guiUpdateImportProgress), 1);
                pictureImportProgress.ShowDialog();
        }

        public void guiUpdateImportProgress(object sender, ProgressChangedEventArgs e)
        {
            pictureImportProgress.updateProgress(1);
        }



        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: windows default
        * return type: void
        * purpose: class viewPhoto
        *********************************************************************************************/
        private void photoListView_ItemActivate(object sender, EventArgs e)
        {
            viewPhoto();
        }


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: an object of ErrorReport which will be used to check if backend was successful
        * return type: void
        * purpose: called by backend once it is done
        *********************************************************************************************/
        public void guiPictureAdded(ErrorReport status)
        {
            if (status.reportID == ErrorReport.FAILURE)
            {
                showError(status.description);
                pictureImportProgress.Close();
                guiPopulatePhotoListView(true);
                return;
            }
            if (status.reportID == ErrorReport.SUCCESS_WITH_WARNINGS)
            {
                //showError("Import picture warning");
            }

            //Do this even if there was a FAILURE
            if (pictureImportProgress.DialogResult != DialogResult.Cancel)
            {
                pictureImportProgress.finished();
            }
            guiPopulatePhotoListView(true);

        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: windows default
        * return type: void
        * purpose: calls workhorse function
        *********************************************************************************************/
        private void addPhotosToExistingAlbumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addPicturesToAlbum(albumChosenbyUser);
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: windows default
        * return type: void
        * purpose: calls workhorse function
        *********************************************************************************************/
        private void createNewAlbumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            guiShowCreateAlbumWindow();
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: windows default
        * return type: void
        * purpose: 
        *********************************************************************************************/
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutToolStripMenuItem.Enabled = false;

            aboutForm aboutBox = new aboutForm();

            aboutBox.ShowDialog();

            aboutToolStripMenuItem.Enabled = true;
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: windows default
        * return type: void
        * purpose: calls workhorse function
        *********************************************************************************************/
        private void mainFormBackbutton_Click(object sender, EventArgs e)
        {
            backButtonActivate();
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: none
        * return type: void
        * purpose: makes back button visible
        *********************************************************************************************/
        private void backButtonActivate()
        {
            mainFormBackbutton.Enabled = false;
            albumListView.BringToFront();
            albumChosenbyUser = 0;
            createNewAlbumToolStripMenuItem.Enabled = true;
            addPhotosToExistingAlbumToolStripMenuItem.Enabled = false;
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: windows default
        * return type: void
        * purpose: 
        *********************************************************************************************/
        private void albumListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (albumListView.SelectedItems.Count <= 0)
            {
                albumListView.ContextMenuStrip = null;
                return;
            }
            albumChosenbyUser = Convert.ToInt32(albumListView.SelectedItems[firstListViewItemIndex].SubItems[listViewSubItemUidIndex].Text);

            if (albumChosenbyUser > 0)
            {
                albumListView.ContextMenuStrip = openAlbumContextMenuStrip;
            }
            else
            {
                albumListView.ContextMenuStrip = null;
            }

        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: windows default
        * return type: void
        * purpose: calls workhorse function
        *********************************************************************************************/
        private void addNewAlbumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            guiShowCreateAlbumWindow();
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: windows default
        * return type: void
        * purpose: 
        *********************************************************************************************/
        private void photoListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (photoListViewItemRenameCheck(e.Label) == false)
            {
                renameToolStripMenuItem.Enabled = true;
                e.CancelEdit = true;
                showError("Invalid photo name.");
                return;
            }
            string selectedItemUid = photoListView.Items[e.Item].SubItems[listViewSubItemUidIndex].Text;

            bombaDeFotos.changePhotoNameByUID(photoNameChanged, albumChosenbyUser, selectedItemUid, e.Label);
            renameToolStripMenuItem.Enabled = true;
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: an object of ErrorReport which will be used to check if backend was successful
        * return type: void
        * purpose: used as a delegate/call_back function to conform to backend function parameters
        *   and is empty for now
        *********************************************************************************************/
        public void photoNameChanged(ErrorReport status)
        {
            ;
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: the string to be checked
        * return type: bool to indicate success of validation
        * purpose: checks the string parameter to see if it's valid according to
        *********************************************************************************************/
        private bool photoListViewItemRenameCheck(string newName)
        {
            RegexStringValidator inputChecker = new RegexStringValidator(@"^[\w\d][\w\d ]{0,30}[\w\d]$");

            try
            {
                inputChecker.Validate(newName);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: windows default
        * return type: void
        * purpose: event handler for when a photo item is activated
        *********************************************************************************************/
        private void photoListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (photoListView.SelectedItems.Count <= 0)
            {
                photoListView.ContextMenuStrip = null;
            }
            else
            {
                photoListView.ContextMenuStrip = photoContextMenuStrip;
                photoNameTempBackup = photoListView.SelectedItems[firstListViewItemIndex].Text;
            }
        }


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: windows default
        * return type: void
        * purpose: 
        *********************************************************************************************/
        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renameToolStripMenuItem.Enabled = false;

            if (photoListView.SelectedItems.Count > 0)
            {
                photoListView.SelectedItems[firstListViewItemIndex].BeginEdit();
            }
        }
        
        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: windows default
        * return type: void
        * purpose: calls workhorse function
        *********************************************************************************************/
        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewPhoto();
        }


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: none
        * return type: void
        * purpose: calls backend function
        *********************************************************************************************/
        private void viewPhoto()
        {
            if (photoListView.SelectedItems.Count > 0)
            {
                string photoUid = photoListView.SelectedItems[firstListViewItemIndex].SubItems[listViewSubItemUidIndex].Text;

                bombaDeFotos.getPictureByGUID(photoInfoRetrieved, photoUid);
            }
        }




        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: an object of ErrorReport which will be used to check if backend was successful
        *   a complexPhoto object containing the info about a previously requested photo
        * return type: void
        * purpose: 
        *********************************************************************************************/
        public void photoInfoRetrieved(ErrorReport status, ComplexPhotoData thePhoto)
        {
            if (status.reportID == ErrorReport.SUCCESS)
            {

                    //check to see if photo exists.
                    if (File.Exists(thePhoto.path) == true)
                    {
                        PhotoViewWindow photoDisplayer = new PhotoViewWindow(this, thePhoto, photoListView.SelectedItems[firstListViewItemIndex].Text);
                        photoDisplayer.ShowDialog();
                    }
                    else
                    {
                        showError("Error: Photograph is not in the library.");
                    }
            }
            else if(status.reportID == ErrorReport.FAILURE)
            {
                showError("Error: Photograph missing.");
            }
        }


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: windows default
        * return type: void
        * purpose: closes form
        *********************************************************************************************/
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: windows default
        * return type: void
        * purpose: calls workhorse function
        *********************************************************************************************/
        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            albumListActivation();
        }






    }
}
