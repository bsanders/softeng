﻿using System;
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
        // true enables additional output to help test the program
        private const bool debugMode = false;

        // switch that enables threading on program
        private const bool threadingMode = false;

        // didn't know what to call it, so I named it the literal spanish translation]
        public SoftwareEng.PhotoBomb bombaDeFotos;

        //stores the albumImageList index of the default image for albums
        private const short defaultAlbumImageListIndex = 0;

        //used to specify the UID of the "add new album" icon
        private const int addAlbumID = 0;

        

        //--first selected item in a List View will have an index of zero, 
        //--this is necessary even if  listView multiselect is disabled
        private const int firstListViewItemIndex = 0;

        //--second subitem of first ListView item will be album uid
        private const int listViewSubItemUidIndex = 1;

        private int albumChosenbyUser;

        private string photoNameTempBackup = "";

        private progressForm pictureImportProgress;


        /************************************************************
         * constructor
         ************************************************************/
        public mainGUI()
        {
            InitializeComponent();

            //for now the gui will determine filepaths(set to same folder as exe) in case it is ever made a user choice
            bombaDeFotos = new PhotoBomb(guiGenericErrorFunction, "albumRC1.xml", "photoRC1.xml", "photo library");

            //ensures that the album list is visible 
            albumListView.BringToFront();

            //true tells the function to refresh list
            populateAlbumView(true);

            
            createNewAlbumToolStripMenuItem.Enabled = true;
            aboutToolStripMenuItem.Enabled = true;

            albumChosenbyUser = addAlbumID;
        }

        /************************************************************
         * Finished
         ************************************************************/
        private void guiGenericErrorFunction(ErrorReport status)
        {
            if (status.reportID != ErrorReport.SUCCESS)
            {
                string oops;

                if (status.description != null)
                {
                    oops = status.description;
                }
                else
                {
                    oops = "Back end failed. ";
                }
                showError(oops);
            }
        }

        //-----------------------------------------------------------

        private void guiConstructorCallback(ErrorReport status)
        {
            if (status.reportID != ErrorReport.SUCCESS)
            {
                bombaDeFotos.rebuildBackendOnFilesystem(new generic_callback(guiGenericErrorFunction));
            }
        }

        /************************************************************
         * Finished
         ************************************************************/
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
                if (threadingMode == true)
                {
                    backgroundAlbumListLoader.RunWorkerAsync();
                }
                else
                {
                    bombaDeFotos.getAllUserAlbumNames(new getAllUserAlbumNames_callback(guiAlbumsRetrieved));
                }
            }
        }

        private void backgroundAlbumListLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            bombaDeFotos.getAllUserAlbumNames(new getAllUserAlbumNames_callback(guiAlbumsRetrieved));
        }

        /************************************************************
         * Finished
         ************************************************************/
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
                        //MessageBox.Show("Exception!", "Super Error", MessageBoxButtons.OK);
                    }
                }

                //--resumes drawing of albumListView
                albumListView.EndUpdate();
            }
            else if (status.reportID == ErrorReport.FAILURE)
            {
                showError(status.description);
            }
        }

        private void backgroundAlbumListDisplayer_DoWork(object sender, DoWorkEventArgs e)
        {

        }


        /************************************************************
        * temporary function to show an error
        ************************************************************/
        private void showError(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Error!", MessageBoxButtons.OK);
        }



        /************************************************************
         * Finished
         ************************************************************/
        private void albumListView_ItemActivate(object sender, EventArgs e)
        {
            albumListActivation();
        }


        /************************************************************
        * Finished -needs more testing
        ************************************************************/
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


        /************************************************************
        * Finished -needs more testing
        ************************************************************/
        public void guiPopulatePhotoListView(bool refreshView)
        {
            photoListView.Clear();

            if (refreshView == true)
            {
                if (threadingMode == true)
                {
                    backgroundPhotoListLoader.RunWorkerAsync();
                }
                else
                {
                    bombaDeFotos.getAllPhotosInAlbum(new getAllPhotosInAlbum_callback(guiPhotosInAlbumRetrieved), albumChosenbyUser);
                }
            }
        }


        private void backgroundPhotoListLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            bombaDeFotos.getAllPhotosInAlbum(new getAllPhotosInAlbum_callback(guiPhotosInAlbumRetrieved), albumChosenbyUser);
        }


        /************************************************************
        * Finished -needs more testing
        ************************************************************/
        public void guiPhotosInAlbumRetrieved(ErrorReport status, List<SimplePhotoData> albumContents)
        {
            photoListView.BringToFront();
            addPhotosToExistingAlbumToolStripMenuItem.Enabled = true;
            mainFormBackbutton.Enabled = true;

            if (debugMode == true && albumListView.SelectedItems.Count > 0)
            {
                statusLabel.Text = albumListView.SelectedItems[0].SubItems[listViewSubItemUidIndex].Text;
            }

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
                        MessageBox.Show("Exception!", "Super Error", MessageBoxButtons.OK);
                    }
                }
                photoListView.EndUpdate();
            }
            else if (status.reportID == ErrorReport.FAILURE)
            {
                showError(status.description);
            }
        }


        /************************************************************
        * Finished
        ************************************************************/
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


        /************************************************************
        * unfinished --used as delegate
        ************************************************************/
        public void guiCheckAlbumNameIsUnique(string userInput, generic_callback addNewAlbumDelegate)
        {
            bombaDeFotos.checkIfAlbumNameIsUnique(addNewAlbumDelegate, userInput);
        }

        /************************************************************
        * Finished --needs testing
        ************************************************************/
        public void guiCreateThisAlbum(string userInput, generic_callback addNewAlbumDelegate)
        {
            SimpleAlbumData theNewAlbum = new SimpleAlbumData();

            theNewAlbum.albumName = userInput;

            bombaDeFotos.addNewAlbum(addNewAlbumDelegate, theNewAlbum);
        }

        /************************************************************
        * Finished, needs testing
        ************************************************************/
        private void addPicturesToAlbum(int albumId)
        {
            if (threadingMode == true)
            {
                backgroundPictureImporter.RunWorkerAsync();
            }
            else
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


                //pictureImportProgress.ShowDialog();
                bombaDeFotos.addNewPictures(guiPictureAdded, fullFileNames, photoExtensions, albumChosenbyUser, null, new ProgressChangedEventHandler(guiUpdateImportProgress), 1);
                pictureImportProgress.ShowDialog();


                /*
                foreach (string picFile in photoOpenFileDialog.FileNames)
                {
                    if (pictureImportProgress.DialogResult != DialogResult.Cancel)
                    {
                        bombaDeFotos.addNewPicture(new generic_callback(guiPictureAdded), picFile, ".jpg", albumId, "");
                        pictureImportProgress.updateProgress(1);
                    }
                    else
                    {
                        break;
                    }
                }
                */
            }
        }

        public void guiUpdateImportProgress(object sender, ProgressChangedEventArgs e)
        {
            pictureImportProgress.updateProgress(1);
        }


        /************************************************************
        * Finished, needs testing
        ************************************************************/
        private void backgroundImportPictureWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (photoOpenFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            progressForm photoImportProgress;


            pictureImportProgress = new progressForm(photoOpenFileDialog.FileNames.Length, null);

            //ComplexPhotoData newPicture = new ComplexPhotoData();

            pictureImportProgress.Show();



            foreach (string picFile in photoOpenFileDialog.FileNames)
            {
                bombaDeFotos.addNewPicture(new generic_callback(guiPictureAdded), picFile, ".jpg", albumChosenbyUser, "");

                pictureImportProgress.updateProgress(1);
            }

            pictureImportProgress.finished();
        }

        /************************************************************
         * unfinished
         ************************************************************/
        private void photoListView_ItemActivate(object sender, EventArgs e)
        {
            viewPhoto();


            if (debugMode == true)
            {
                if (albumListView.SelectedItems.Count > 0)
                {
                    statusLabel.Text = albumListView.SelectedItems[0].SubItems[listViewSubItemUidIndex].Text;
                }
                else
                {
                    statusLabel.Text = "-1";
                }
            }
        }


        /************************************************************
        * Finished
        ************************************************************/
        public void guiPictureAdded(ErrorReport status)
        {
            if (status.reportID == ErrorReport.FAILURE)
            {
                showError(status.description);
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

        /************************************************************
        * 
        ************************************************************/
        private void addPhotosToExistingAlbumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addPicturesToAlbum(albumChosenbyUser);
        }

        /************************************************************
        * Finished
        ************************************************************/
        private void createNewAlbumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            guiShowCreateAlbumWindow();
        }

        /************************************************************
        * Finished
        ************************************************************/
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("   Created by PhotoBombers Studio, LLC   ", "About", MessageBoxButtons.OK);
            aboutToolStripMenuItem.Enabled = false;

            aboutForm aboutBox = new aboutForm();

            aboutBox.ShowDialog();

            aboutToolStripMenuItem.Enabled = true;
        }

        /************************************************************
        * Finished
        ************************************************************/
        private void mainFormBackbutton_Click(object sender, EventArgs e)
        {
            mainFormBackbutton.Enabled = false;
            albumListView.BringToFront();
            albumChosenbyUser = 0;
            createNewAlbumToolStripMenuItem.Enabled = true;
            addPhotosToExistingAlbumToolStripMenuItem.Enabled = false;

            //albumListView.SelectedItems.Clear();


            if (albumListView.SelectedItems.Count > 0 && debugMode == true)
            {
                //statusLabel.Text = albumListView.SelectedItems[0].SubItems[1].Text;
            }
            else if (debugMode == true)
            {
                //statusLabel.Text="-1";
            }

        }

        /************************************************************
        * unfinished
        ************************************************************/
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
                //albumListView.ContextMenuStrip = addAlbumContextMenuStrip;
                albumListView.ContextMenuStrip = null;
            }

        }

        /************************************************************
        * Finished --needs testing
        ************************************************************/
        private void addNewAlbumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            guiShowCreateAlbumWindow();
        }

        private void photoListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (photoListViewItemRenameCheck(e.Label) == false)
            {
                e.CancelEdit = true;
                showError("Invalid photo name.");
                return;
            }
            int selectedItemUid = Convert.ToInt32(photoListView.Items[e.Item].SubItems[listViewSubItemUidIndex].Text);

            bombaDeFotos.changePhotoNameByUID(photoNameChanged, albumChosenbyUser, selectedItemUid, e.Label);
            renameToolStripMenuItem.Enabled = true;
        }

        public void photoNameChanged(ErrorReport status)
        {
            ;
        }

        private bool photoListViewItemRenameCheck(string newName)
        {
            //string validInputKey = @"^[\w\d][\w\d ]{0,30}[\w\d]$";

            RegexStringValidator inputChecker = new RegexStringValidator(@"^[\w\d][\w\d ]{0,30}[\w\d]$");

            try
            {
                inputChecker.Validate(newName);
                return true;
            }
            catch (ArgumentException)
            {
                if (debugMode == true)
                {
                    statusLabel.Text = "Bam! Check Not finished";
                }
                return false;
            }
        }

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
                statusLabel.Text = photoNameTempBackup;
            }
        }

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
        //-------------------------------------
        private void viewPhoto()
        {
            if (photoListView.SelectedItems.Count > 0)
            {
                int photoUid = Convert.ToInt32(photoListView.SelectedItems[firstListViewItemIndex].SubItems[listViewSubItemUidIndex].Text);

                //int albumUid = Convert.ToInt32(albumListView.SelectedItems[firstListViewItemIndex].SubItems[listViewSubItemUidIndex].Text);

                bombaDeFotos.getPictureByUID(photoInfoRetrieved, photoUid);
            }
        }
        //-------------------------------------
        public void photoInfoRetrieved(ErrorReport status, ComplexPhotoData thePhoto)
        {
            if (status.reportID == ErrorReport.SUCCESS)
            {
                //ComplexPhotoData toPhotoView= new ComplexPhotoData;
                if (threadingMode == true)
                {
                    backgroundPhotoLoader.RunWorkerAsync(thePhoto);
                }
                else
                {
                    //check to see if photo exists.
                    //if exists
                    PhotoViewWindow photoDisplayer = new PhotoViewWindow(this, thePhoto, photoListView.SelectedItems[firstListViewItemIndex].Text);
                    photoDisplayer.ShowDialog();
                    //else
                    //show error window.
                    
                }
            }
            else if(status.reportID == ErrorReport.FAILURE)
            {
                showError(status.description);
            }
        }

        private void backgroundPhotoLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            ComplexPhotoData thePhoto = (ComplexPhotoData)e.Argument;

            PhotoViewWindow photoDisplayer = new PhotoViewWindow(this, thePhoto, photoListView.SelectedItems[firstListViewItemIndex].Text);

            photoDisplayer.ShowDialog();
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }



        private void backgroundImportPictureWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //true tells the function to refresh list
            guiPopulatePhotoListView(true);
        }

        private void backgroundAlbumListLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            albumListActivation();
        }






    }
}
