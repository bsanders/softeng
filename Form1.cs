//Don't expect many comments on this page, its for
//testing the backend and will be nuked once we get
//the specs for the real gui.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.Xml.Linq;

namespace TestApp
{
    public partial class Form1 : Form
    {
        //This is the main backend of the program.
        TestApp.PhotoBomb photoBomb;

        //--------------------------------------------------------------------

        //init.
        public Form1()
        {
            InitializeComponent();
            photoBomb = new TestApp.PhotoBomb();
        }

        //--------------------------------------------------------------------

        //test button.
        private void loadXML_Click(object sender, EventArgs e)
        {
            photoBomb.openAlbumsXML(new TestApp.generic_callback(loadXML_Callback), xmlPathTE.Text);
        }


        //callback for above test button.
        public void loadXML_Callback(TestApp.ErrorReport e)
        {
            if (e.reportID == TestApp.ErrorReport.SUCCESS || e.reportID == TestApp.ErrorReport.SUCCESS_WITH_WARNINGS)
            {
                output.AppendText("XML Loaded\n");
                if(e.reportID == TestApp.ErrorReport.SUCCESS_WITH_WARNINGS)
                {
                    for (int i = 0; i < e.warnings.Count; ++i)
                    {
                        output.AppendText(e.warnings.ElementAt(i));
                    }
                }
            }

            else
            {
                output.AppendText("Error: " + e.description + "\n\n");
            }
        }

        //--------------------------------------------------------------------

        //test save button.
        private void button2_Click(object sender, EventArgs e)
        {
            photoBomb.saveAlbumsXML(new TestApp.generic_callback(saveXML_Callback), xmlPathTE.Text);
        }//method


        //callback for the above test button
        public void saveXML_Callback(TestApp.ErrorReport e)
        {
            if (e.reportID == TestApp.ErrorReport.SUCCESS || e.reportID == TestApp.ErrorReport.SUCCESS_WITH_WARNINGS)
            {
                output.AppendText("XML Saved\n");

                if (e.reportID == TestApp.ErrorReport.SUCCESS_WITH_WARNINGS)
                {
                    output.AppendText("Warnings:\n");
                    for (int i = 0; i < e.warnings.Count; ++i)
                    {
                        output.AppendText(e.warnings.ElementAt(i));
                    }
                }
            }

            else
            {
                output.AppendText("Error: " + e.description + "\n\n");
            }
        }


        //--------------------------------------------------------------------


        private void loadAlbums_Click(object sender, EventArgs e)
        {
            photoBomb.getAllUserAlbumNames(new TestApp.getAllUserAlbumNames_callback(loadAlbums_Callback));
        }


        public void loadAlbums_Callback(TestApp.ErrorReport error, List<TestApp.SimpleAlbumData> _albums)
        {
            if (error.reportID == TestApp.ErrorReport.SUCCESS || error.reportID == TestApp.ErrorReport.SUCCESS_WITH_WARNINGS)
            {
                output.AppendText("Albums Found:\n");
                //output albums
                for (int i = 0; i < _albums.Count; ++i)
                {
                    output.AppendText(_albums.ElementAt(i).albumName + ", UID: " + _albums.ElementAt(i).UID + "\n");
                }
                //output warnings
                if (error.reportID == TestApp.ErrorReport.SUCCESS_WITH_WARNINGS)
                {
                    output.AppendText("\nWarnings:\n");
                    for (int i = 0; i < error.warnings.Count; ++i)
                    {
                        output.AppendText(error.warnings.ElementAt(i) + "\n");
                    }
                }
            }

            else
            {
                output.AppendText("Error: " + error.description + "\n\n");
            }
        }

        //---------------------------------------------------------------------

        //load Album Pictures
        private void loadAlbumPictures_Click(object sender, EventArgs e)
        {
            int UID;
            try
            {
                int.TryParse(albumUID.Text, out UID);
                if (UID > 0 && UID < 9999999)
                {
                    photoBomb.getAllPhotosInAlbum(new TestApp.getAllPhotosInAlbum_callback(loadAlbumPictures_callback), UID);
                }
                else
                {
                    output.AppendText("The UID does not appear to be a valid number, try again.\n");
                }
            }
            catch
            {
                output.AppendText("The UID does not appear to be a valid number, try again.\n");
            }
            
        }

        public void loadAlbumPictures_callback(TestApp.ErrorReport error, List<TestApp.SimplePhotoData> _pictures)
        {
            
            if (error.reportID == TestApp.ErrorReport.FAILURE)
            {
                output.AppendText("FAILURE!!!\n" + error.description + "\n");
            }
            if (error.reportID == TestApp.ErrorReport.SUCCESS || error.reportID == TestApp.ErrorReport.SUCCESS_WITH_WARNINGS)
            {
                output.AppendText("Pictures in album:\n");
                for (int i = 0; i < _pictures.Count; ++i)
                {
                    output.AppendText(_pictures.ElementAt(i).pictureName + "\n");
                }
            }
        }


        //---------------------------------------------------------------------






    }//class




}//namespace

