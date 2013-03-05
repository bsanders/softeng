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


        private progressForm pictureImportProgress;


        //--for showing errors 
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
            String libraryPath = System.IO.Path.Combine(Environment.CurrentDirectory, "photo library");
            bombaDeFotos = new PhotoBomb();
            bombaDeFotos.init(guiConstructorCallback, "albumRC1.xml", "photoRC1.xml", libraryPath);


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
        * parameters: 
        * return type: 
        * purpose: 
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
        * parameters: 
        * return type: 
        * purpose: 
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
        * parameters: 
        * return type: 
        * purpose: 
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
        * parameters: 
        * return type: 
        * purpose: 
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
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        private void albumListView_ItemActivate(object sender, EventArgs e)
        {
            albumListActivation();
        }


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        private void albumListActivation()
        {
            if (albumListView.SelectedItems.Count > 0)
            {
                int selectedAlbumID = Convert.ToInt32(albumListView.SelectedItems[firstListViewItemIndex].SubItems[listViewSubItemUidIndex].Text);

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
        * parameters: 
        * return type: 
        * purpose: 
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
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        public void guiPhotosInAlbumRetrieved(ErrorReport status, List<SimplePhotoData> albumContents)
        {
            photoListView.BringToFront();
            addPhotosToExistingAlbumToolStripMenuItem.Enabled = true;
            mainFormBackbutton.Enabled = true;

            if (status.reportID == ErrorReport.SUCCESS)
            {
                photoListView.BeginUpdate();
                ListViewItem itemHolder = null;
                ListViewItem.ListViewSubItem[] itemHolderSubitems;

                foreach (SimplePhotoData singlePhoto in albumContents)
                {
                    try
                    {
                        itemHolderSubitems = new ListViewItem.ListViewSubItem[]{
                            new ListViewItem.ListViewSubItem(itemHolder, singlePhoto.picturesNameInAlbum),
                            new ListViewItem.ListViewSubItem(itemHolder, singlePhoto.UID.ToString() )
                            };

                        itemHolder = new ListViewItem(itemHolderSubitems, defaultAlbumImageListIndex);
                        photoListView.Items.Add(itemHolder);
                    }
                    catch (Exception)
                    {
                        showError("Error: Album missing.");
                    }
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
        * parameters: 
        * return type: 
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
        * return type: 
        * purpose: 
        *********************************************************************************************/
        public void guiCheckAlbumNameIsUnique(string userInput, generic_callback addNewAlbumDelegate)
        {
            bombaDeFotos.checkIfAlbumNameIsUnique(addNewAlbumDelegate, userInput);
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
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
        * parameters: 
        * return type: 
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

                foreach (string picFile in photoOpenFileDialog.FileNames)
                {
                    fullFileNames.Add(picFile);

                    photoExtensions.Add(".jpg");
                }

                progressForm photoImportProgress;

                pictureImportProgress = new progressForm(photoOpenFileDialog.FileNames.Length, bombaDeFotos);

                

                ComplexPhotoData newPicture = new ComplexPhotoData();


                bombaDeFotos.addNewPictures(guiPictureAdded, fullFileNames, photoExtensions, albumChosenbyUser, null, new ProgressChangedEventHandler(guiUpdateImportProgress), 1);
                pictureImportProgress.ShowDialog();
        }

        public void guiUpdateImportProgress(object sender, ProgressChangedEventArgs e)
        {
            pictureImportProgress.updateProgress(1);
        }



        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        private void photoListView_ItemActivate(object sender, EventArgs e)
        {
            viewPhoto();
        }


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        public void guiPictureAdded(ErrorReport status)
        {
            if (status.reportID == ErrorReport.FAILURE)
            {
                //showError(status.description);
            }
            else if (status.reportID == ErrorReport.SUCCESS_WITH_WARNINGS)
            {
                //showError("Import picture warning");
            }
            else
            {
                if (pictureImportProgress.DialogResult != DialogResult.Cancel)
                {
                    pictureImportProgress.finished();
                }
                guiPopulatePhotoListView(true);
            }
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        private void addPhotosToExistingAlbumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addPicturesToAlbum(albumChosenbyUser);
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        private void createNewAlbumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            guiShowCreateAlbumWindow();
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
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
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        private void mainFormBackbutton_Click(object sender, EventArgs e)
        {
            backButtonActivate();
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
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
        * parameters: 
        * return type: 
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
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        private void addNewAlbumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            guiShowCreateAlbumWindow();
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
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
            int selectedItemUid = Convert.ToInt32(photoListView.Items[e.Item].SubItems[listViewSubItemUidIndex].Text);

            bombaDeFotos.changePhotoNameByUID(photoNameChanged, albumChosenbyUser, selectedItemUid, e.Label);
            renameToolStripMenuItem.Enabled = true;
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        public void photoNameChanged(ErrorReport status)
        {
            ;
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
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
        * parameters: 
        * return type: 
        * purpose: 
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
        * parameters: 
        * return type: 
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

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewPhoto();
        }


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        private void viewPhoto()
        {
            if (photoListView.SelectedItems.Count > 0)
            {
                int photoUid = Convert.ToInt32(photoListView.SelectedItems[firstListViewItemIndex].SubItems[listViewSubItemUidIndex].Text);

                bombaDeFotos.getPictureByUID(photoInfoRetrieved, photoUid);
            }
        }




        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
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
                        showError("Error: Photograph missing.");
                    }
            }
            else if(status.reportID == ErrorReport.FAILURE)
            {
                showError("Error: Photograph missing.");
            }
        }


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            albumListActivation();
        }






    }
}
