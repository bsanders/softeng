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

        private const short defaultAlbumImageListIndex = 0;

        /************************************************************
         * 
         ************************************************************/
        public mainGUI()
        {
            InitializeComponent();

            //for now the gui will determine filepaths in case it is ever made a user choice
            bombaDeFotos = new PhotoBomb(guiGenericErrorFunction, "albumRC1.xml", "photoRC1.xml", "photo library");

            //createNewAlbumToolStripMenuItem.Enabled = true;
            
            populateAlbumView(false);

            createNewAlbumToolStripMenuItem.Enabled = true;
            aboutToolStripMenuItem.Enabled = true;
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

            itemHolderSubitems= new ListViewItem.ListViewSubItem[]{
                new ListViewItem.ListViewSubItem(itemHolder, "Add New Album"),
                new ListViewItem.ListViewSubItem(itemHolder, "0")};

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
                ListViewItem itemHolder = null;
                ListViewItem.ListViewSubItem[] itemHolderSubitems;

                foreach (SimpleAlbumData singleAlbum in albumsRetrieved)
                {
                    try
                    {
                        itemHolderSubitems = new ListViewItem.ListViewSubItem[]{
                        new ListViewItem.ListViewSubItem(itemHolder, singleAlbum.albumName),
                        new ListViewItem.ListViewSubItem(itemHolder, singleAlbum.UID.ToString() )};

                        itemHolder = new ListViewItem(itemHolderSubitems, defaultAlbumImageListIndex);
                        albumListView.Items.Add(itemHolder);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Exception!", "Super Deez Nutz", MessageBoxButtons.OK);
                    }
                }
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
            MessageBox.Show(errorMessage, "Deez Nutz!", MessageBoxButtons.OK);
        }




        /************************************************************
         * 
         ************************************************************/
        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ;
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
            //first item will have an index of zero, necessary even if multiselect is disabled
            const int firstItem = 0;

            //second subitem will be album uid
            const int uid = 1;

            int selectedAlbumID = Convert.ToInt32(albumListView.SelectedItems[firstItem].SubItems[uid].Text);

            if (selectedAlbumID >= 1)
            {
                bombaDeFotos.getAllPhotosInAlbum(new getAllPhotosInAlbum_callback(guiPhotosInAlbumRetrieved), selectedAlbumID);
            }
            else
            {
                guiAddNewAlbum();
            }
        }

        /************************************************************
        * 
        ************************************************************/
        public void guiPhotosInAlbumRetrieved(ErrorReport status, List<SimplePhotoData> albumContents)
        {
            photoListView.BringToFront();
            addPhotosToExistingAlbumToolStripMenuItem.Enabled = true;
            mainFormBackbutton.Enabled = true;

            if (status.reportID == ErrorReport.SUCCESS)
            {
                ListViewItem itemHolder = null;
                ListViewItem.ListViewSubItem[] itemHolderSubitems;

                foreach (SimplePhotoData singlePhoto in albumContents)
                {
                    try
                    {
                        itemHolderSubitems = new ListViewItem.ListViewSubItem[]{
                        new ListViewItem.ListViewSubItem(itemHolder, singlePhoto.pictureName),
                        new ListViewItem.ListViewSubItem(itemHolder, singlePhoto.UID.ToString() )};

                        itemHolder = new ListViewItem(itemHolderSubitems, defaultAlbumImageListIndex);
                        albumListView.Items.Add(itemHolder);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Exception!", "Super Deez Nutz", MessageBoxButtons.OK);
                    }
                }
            }
            else
            {
                showError("Error Loading Pictures");
            }
        }


        /************************************************************
        * 
        ************************************************************/
        private void guiAddNewAlbum()
        {
            PhotoBomb photobombRef = bombaDeFotos;

            addNewAlbum createAlbumDialog = new addNewAlbum();

            createAlbumDialog.ShowDialog();
        }


        /************************************************************
        * used as delegate
        ************************************************************/
        public void guiNewAlbumNamed(string userInput)
        {
            showError(label);
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
                showError(label);
                return;
            }
            ComplexPhotoData newPicture= new ComplexPhotoData();
            foreach (string picFile in photoOpenFileDialog.FileNames)
            {
                newPicture.path= picFile;
                newPicture.extension = ".jpeg";
                bombaDeFotos.addNewPicture(new generic_callback(guiPictureAdded), newPicture, albumId);

            }
        }

        /************************************************************
        * 
        ************************************************************/
        public void guiPictureAdded(ErrorReport status)
        {
            if (status.reportID != ErrorReport.SUCCESS)
            {
                showError("Failed to import a picture");
            }
        }



        public static string label = "8===============================D-------- ({})";

        /************************************************************
        * 
        ************************************************************/
        private void addPhotosToExistingAlbumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showError("Functionailty not yet finished, LOL");
        }

        /************************************************************
        * 
        ************************************************************/
        private void createNewAlbumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            guiAddNewAlbum();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("   Created by PhotoBombers Studio, LLC   ", "About", MessageBoxButtons.OK);
        }

        private void mainFormBackbutton_Click(object sender, EventArgs e)
        {
            albumListView.BringToFront();
            mainFormBackbutton.Enabled = false;
        }
    }
}
