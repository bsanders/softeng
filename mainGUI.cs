using System;
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
        private const bool debugMode = true;

        private SoftwareEng.PhotoBomb bombaDeFotos;

        //stores the albumImageList index of the default image for albums
        private const short defaultAlbumImageListIndex = 0;

        //used to specify the UID of the "add new album" icon
        private const int addAlbumID = 0;

        //--first selected item in a List View will have an index of zero, 
        //--this is necessary even if  listView multiselect is disabled
        private const int firstListViewItemIndex = 0;

        //--second subitem of first ListView item will be album uid
        private const int albumListViewSubItemUidIndex = 1;

        private int albumChosenbyUser;

        /************************************************************
         * constructor
         ************************************************************/
        public mainGUI()
        {
            InitializeComponent();

            //for now the gui will determine filepaths in case it is ever made a user choice
            bombaDeFotos = new PhotoBomb(guiGenericErrorFunction, "albumRC1.xml", "photoRC1.xml", "photo library");
            
            //display all albums 
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

        /************************************************************
         * Finished
         ************************************************************/
        private void populateAlbumView(bool refreshView)
        {
            ListViewItem.ListViewSubItem[] itemHolderSubitems;
            ListViewItem itemHolder= null;

            albumListView.Items.Clear();

            //--code from here to the if statement is to regenerate the 
            //--the "add new album icon" as it's not an album
            itemHolderSubitems= new ListViewItem.ListViewSubItem[]{
                new ListViewItem.ListViewSubItem(itemHolder, "Add New Album"),
                new ListViewItem.ListViewSubItem(itemHolder, addAlbumID.ToString() )
                };

            itemHolder = new ListViewItem(itemHolderSubitems, defaultAlbumImageListIndex);
            albumListView.Items.Add(itemHolder);

            if(refreshView == true)
            {
                bombaDeFotos.getAllUserAlbumNames(new getAllUserAlbumNames_callback(guiAlbumsRetrieved));
            }
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
                        MessageBox.Show("Exception!", "Super Error", MessageBoxButtons.OK);
                    }
                }

                //--resumes drawing of albumListView
                albumListView.EndUpdate();
            }
            else if(status.reportID == ErrorReport.FAILURE)
            {
                showError(status.description);
            }
        }

        /************************************************************
        * temp
        ************************************************************/
        private void showError(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Error!", MessageBoxButtons.OK);
        }

        /************************************************************
         * unfinished
         ************************************************************/
        private void photoListView_ItemActivate(object sender, EventArgs e)
        {
            statusLabel.Text = albumListView.SelectedItems[0].SubItems[1].Text;
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
            createNewAlbumToolStripMenuItem.Enabled = false;

            int selectedAlbumID = Convert.ToInt32(albumListView.SelectedItems[firstListViewItemIndex].SubItems[albumListViewSubItemUidIndex].Text);

            if (selectedAlbumID > addAlbumID)
            {
                albumChosenbyUser = selectedAlbumID;

                //true tells the function to refresh list
                guiPopulatePhotoListView(true);

                albumChosenbyUser = selectedAlbumID;
            }
            else
            {
                guiShowCreateAlbumWindow();
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
                bombaDeFotos.getAllPhotosInAlbum(new getAllPhotosInAlbum_callback(guiPhotosInAlbumRetrieved), albumChosenbyUser);
            }
        }


        /************************************************************
        * Finished -needs more testing
        ************************************************************/
        public void guiPhotosInAlbumRetrieved(ErrorReport status, List<SimplePhotoData> albumContents)
        {
            photoListView.BringToFront();
            addPhotosToExistingAlbumToolStripMenuItem.Enabled = true;
            mainFormBackbutton.Enabled = true;

            if (debugMode == true)
            {
                statusLabel.Text = albumListView.SelectedItems[0].SubItems[1].Text;
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
            else if(status.reportID == ErrorReport.FAILURE)
            {
                showError(status.description);
            }
        }


        /************************************************************
        * Finished
        ************************************************************/
        private void guiShowCreateAlbumWindow()
        {
            addNewAlbum createAlbumDialog = new addNewAlbum(this);

            createAlbumDialog.ShowDialog();

            if (createAlbumDialog.DialogResult == DialogResult.OK)
            {
                createAlbumDialog.Close();

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
        * unfinished
        ************************************************************/
        private void addPicturesToAlbum(int albumId)
        {
            if (photoOpenFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            ComplexPhotoData newPicture= new ComplexPhotoData();
            foreach (string picFile in photoOpenFileDialog.FileNames)
            {
                newPicture.path= picFile;
                newPicture.extension = ".jpg";
                //bombaDeFotos.addNewPicture(new generic_callback(guiPictureAdded), newPicture, albumId, "");
            }
            //true tells the function to refresh list
            guiPopulatePhotoListView(true);
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
                showError("Import picture warning");
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
            MessageBox.Show("   Created by PhotoBombers Studio, LLC   ", "About", MessageBoxButtons.OK);
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


            if (debugMode == true)
            {
                statusLabel.Text = albumListView.SelectedItems[0].SubItems[1].Text;
            }
        }

        /************************************************************
        * unfinished --needs testing
        ************************************************************/
        private void albumListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            //albumChosenbyUser = Convert.ToInt32(albumListView.SelectedItems[firstListViewItemIndex].SubItems[albumListViewSubItemUidIndex].Text);
        }

        /************************************************************
        * Finished --needs testing
        ************************************************************/
        private void addNewAlbumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            guiShowCreateAlbumWindow();
        }
    }
}
