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
        private SoftwareEng.PhotoBomb bombaDeFotos;

        //stores the albumImageList index of the default image for albums
        private const short defaultAlbumImageListIndex = 0;

        //used to specify the UID of the "add new album" icon
        private const int addAlbumID = 0;

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
            populateAlbumView(false);

            createNewAlbumToolStripMenuItem.Enabled = true;
            aboutToolStripMenuItem.Enabled = true;

            albumChosenbyUser = addAlbumID;
        }

        /************************************************************
         * 
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
         * 
         ************************************************************/
        public void guiLoadXml(ErrorReport status)
        {
            if (status.reportID == ErrorReport.SUCCESS)
            {
                ;
            }
            else
            {
                ;
            }

        }

        /************************************************************
         * 
         ************************************************************/
        private void populateAlbumView(bool resetView)
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

            if(resetView == false)
            {
                bombaDeFotos.getAllUserAlbumNames(new getAllUserAlbumNames_callback(guiAlbumsRetrieved));
            }
        }

        /************************************************************
         * 
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
            else
            {
                showError("Error Loading Albums");
            }
        }

        /************************************************************
        * 
        ************************************************************/
        private void showError(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Error!", MessageBoxButtons.OK);
        }

        /************************************************************
         * 
         ************************************************************/
        private void photoListView_ItemActivate(object sender, EventArgs e)
        {
            ;
        }

        /************************************************************
         * 
         ************************************************************/
        private void albumListView_ItemActivate(object sender, EventArgs e)
        {
            albumListActivation();   
        }


        /************************************************************
        * 
        ************************************************************/
        private void albumListActivation()
        {
            //--first selected item will have an index of zero, 
            //--this isnecessary even if multiselect is disabled
            const int firstItem = 0;

            //--second subitem will be album uid
            const int uid = 1;

            int selectedAlbumID = Convert.ToInt32(albumListView.SelectedItems[firstItem].SubItems[uid].Text);

            if (selectedAlbumID > addAlbumID)
            {
                bombaDeFotos.getAllPhotosInAlbum(new getAllPhotosInAlbum_callback(guiPhotosInAlbumRetrieved), selectedAlbumID);
                albumChosenbyUser = selectedAlbumID;
            }
            else
            {
                guiShowCreateAlbumWindow();
            }
        }
        

        /************************************************************
        * 
        ************************************************************/
        public void guiPopulatePhotoListView(bool resetView)
        {
            photoListView.Clear();

            if (resetView == false)
            {
                bombaDeFotos.getAllPhotosInAlbum(new getAllPhotosInAlbum_callback(guiPhotosInAlbumRetrieved), albumChosenbyUser);
            }
        }


        /************************************************************
        * 
        ************************************************************/
        public void guiPhotosInAlbumRetrieved(ErrorReport status, List<SimplePhotoData> albumContents)
        {
            photoListView.BringToFront();
            addPhotosToExistingAlbumToolStripMenuItem.Enabled = true;
            ;
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
                        albumListView.Items.Add(itemHolder);
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
        * 
        ************************************************************/
        private void guiShowCreateAlbumWindow()
        {
            addNewAlbum createAlbumDialog = new addNewAlbum(this);

            createAlbumDialog.ShowDialog();
        }


        /************************************************************
        * used as delegate
        ************************************************************/
        public void guiCheckAlbumNameIsUnique(string userInput, generic_callback createAlbumDelegate)
        {
            ErrorReport errorStatus= new ErrorReport();

            //bombaDeFotos.checkIfAlbumNameIsUnique();

            createAlbumDelegate(errorStatus);
           
        }

        public void guiAlbumNameChecked(ErrorReport errorStatus)
        {
            //createAlbumDelegate(errorStatus);

        }

        public void guiAddAlbum()
        {
            ;
        }





        /************************************************************
        * used for generic_callback after calling photobomb object
        ************************************************************/
        public void guiAlbumAdded(ErrorReport status)
        {
            if (status.reportID == ErrorReport.SUCCESS)
            {
                statusLabel.Text = "Album Successfully Created!";
            }
            else
            {
                if (status.description != null)
                {
                    showError(status.description);
                }
            }
        }


        /************************************************************
        * 
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
                bombaDeFotos.addNewPicture(new generic_callback(guiPictureAdded), newPicture, albumId, "");
            }
        }

        /************************************************************
        * 
        ************************************************************/
        public void guiPictureAdded(ErrorReport status)
        {
            if (status.reportID == ErrorReport.FAILURE)
            {
                showError(status.description);
            }
            else if (status.reportID == ErrorReport.SUCCESS_WITH_WARNINGS)
            {
                showError("import picture warning");
            }
        }

        /************************************************************
        * 
        ************************************************************/
        private void addPhotosToExistingAlbumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addPicturesToAlbum(albumChosenbyUser);

            //showError("Functionailty not yet finished, LOL");
        }

        /************************************************************
        * 
        ************************************************************/
        private void createNewAlbumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            guiShowCreateAlbumWindow();
        }

        /************************************************************
        * 
        ************************************************************/
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("   Created by PhotoBombers Studio, LLC   ", "About", MessageBoxButtons.OK);
        }

        /************************************************************
        * 
        ************************************************************/
        private void mainFormBackbutton_Click(object sender, EventArgs e)
        {
            albumListView.BringToFront();
            mainFormBackbutton.Enabled = false;
        }
    }
}
