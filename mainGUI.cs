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

        public mainGUI()
        {
            InitializeComponent();

            bombaDeFotos = new PhotoBomb("", "", "");

            bombaDeFotos.openAlbumsXML(new generic_callback(guiLoadXml_Delegate));

            bombaDeFotos.openPicturesXML(new generic_callback(guiLoadXml_Delegate));

        }


        public void guiLoadXml_Delegate(ErrorReport status)
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

        private void populateAlbumListView(bool resetView)
        {
            ListViewItem.ListViewSubItem[] itemHolderSubitems;
            ListViewItem itemHolder;

            albumListView.Items.Clear();





            bombaDeFotos.getAllUserAlbumNames(new getAllUserAlbumNames_callback(guiAlbumsRetrieved_Delegate));
        }

        public void guiAlbumsRetrieved_Delegate(ErrorReport status, List<SimpleAlbumData> albumsRetrieved)
        {
            

            if (status.reportID == ErrorReport.SUCCESS)
            {
                foreach (SimpleAlbumData singleAlbum in albumsRetrieved)
                {
                    //albumListView.Items.Add;
                }
            }
            else
            {
                ;
            }

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ;
        }

        private void photoListView_ItemActivate(object sender, EventArgs e)
        {
            ;
        }

        private void albumListView_ItemActivate(object sender, EventArgs e)
        {
            ;
        }

        private void guiAddNewAlbum()
        {
            ;


        }
    }
}
