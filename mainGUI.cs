using System;
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
            //ListViewItem.ListViewSubItem[] itemHolderSubitems;
            ListViewItem itemHolder= null;

            albumListView.Items.Clear();

            //itemHolderSubitems= new ListViewItem.ListViewSubItem[]{new ListViewItem.ListViewSubItem(itemHolder, "Add New Album")};

            itemHolder = new ListViewItem("Add New Album", defaultAlbumImageListIndex);
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
            ;
        }

        /************************************************************
        * 
        ************************************************************/
        private void guiAddNewAlbum()
        {
            ;


        }


        /************************************************************
        * used as delegate and passed to addNewAlbum form
        ************************************************************/
        public void guiNewAlbumNamed(string userInput)
        {
            //bombaDeFotos.addNewAlbum(

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


        public static string label = "8===========================D-----------------";
    }
}
