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

namespace SoftwareEng
{
    public partial class Form1 : Form
    {
        //This is the main backend of the program.
        SoftwareEng.PhotoBomb photoBomb;

        //--------------------------------------------------------------------

        //init.
        public Form1()
        {
            InitializeComponent();
            photoBomb = new SoftwareEng.PhotoBomb(new generic_callback(photoBombConstructor_callback), "test.xml", "test2.xml", "");
        }

        public void photoBombConstructor_callback(ErrorReport e)
        {

        }

        //--------------------------------------------------------------------

        //test button.
        private void loadXML_Click(object sender, EventArgs e)
        {
            photoBomb.reopenAlbumsXML(new SoftwareEng.generic_callback(loadXML_Callback));
            photoBomb.reopenPicturesXML(new SoftwareEng.generic_callback(loadXML_Callback));
        }


        //callback for above test button.
        public void loadXML_Callback(SoftwareEng.ErrorReport e)
        {
            if (e.reportID == SoftwareEng.ErrorReport.SUCCESS || e.reportID == SoftwareEng.ErrorReport.SUCCESS_WITH_WARNINGS)
            {
                output.AppendText("XML Loaded\n");
                if(e.reportID == SoftwareEng.ErrorReport.SUCCESS_WITH_WARNINGS)
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
            photoBomb.saveAlbumsXML(new SoftwareEng.generic_callback(saveXML_Callback));
            photoBomb.savePicturesXML(new SoftwareEng.generic_callback(saveXML_Callback));
        }//method


        //callback for the above test button
        public void saveXML_Callback(SoftwareEng.ErrorReport e)
        {
            if (e.reportID == SoftwareEng.ErrorReport.SUCCESS || e.reportID == SoftwareEng.ErrorReport.SUCCESS_WITH_WARNINGS)
            {
                output.AppendText("XML Saved\n");

                if (e.reportID == SoftwareEng.ErrorReport.SUCCESS_WITH_WARNINGS)
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
            photoBomb.getAllUserAlbumNames(new SoftwareEng.getAllUserAlbumNames_callback(loadAlbums_Callback));
        }


        public void loadAlbums_Callback(SoftwareEng.ErrorReport error, List<SoftwareEng.SimpleAlbumData> _albums)
        {
            if (error.reportID == SoftwareEng.ErrorReport.SUCCESS || error.reportID == SoftwareEng.ErrorReport.SUCCESS_WITH_WARNINGS)
            {
                output.AppendText("Albums Found:\n");
                //output albums
                for (int i = 0; i < _albums.Count; ++i)
                {
                    output.AppendText(_albums.ElementAt(i).albumName + ", UID: " + _albums.ElementAt(i).UID + "\n");
                }
                //output warnings
                if (error.reportID == SoftwareEng.ErrorReport.SUCCESS_WITH_WARNINGS)
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

        //load all Album Pictures
        private void loadAlbumPictures_Click(object sender, EventArgs e)
        {
            int UID;
            try
            {
                int.TryParse(uidTE.Text, out UID);
                if (UID > 0 && UID < 9999999)
                {
                    photoBomb.getAllPhotosInAlbum(new SoftwareEng.getAllPhotosInAlbum_callback(loadAlbumPictures_callback), UID);
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

        public void loadAlbumPictures_callback(SoftwareEng.ErrorReport error, List<SoftwareEng.SimplePhotoData> _pictures)
        {
            
            if (error.reportID == SoftwareEng.ErrorReport.FAILURE)
            {
                output.AppendText("FAILURE!!!\n" + error.description + "\n");
            }
            if (error.reportID == SoftwareEng.ErrorReport.SUCCESS || error.reportID == SoftwareEng.ErrorReport.SUCCESS_WITH_WARNINGS)
            {
                output.AppendText("Pictures in album:\n");
                for (int i = 0; i < _pictures.Count; ++i)
                {
                    output.AppendText(_pictures.ElementAt(i).pictureName + ", UID: " + _pictures.ElementAt(i).UID + "\n");
                }
            }
        }

        //---------------------------------------------------------------------
        //Get a picture's complex data
        private void getPictureByUIDButton_Click(object sender, EventArgs e)
        {
            int UID;
            try
            {
                int.TryParse(uidTE.Text, out UID);
            }
            catch
            {
                output.AppendText("The UID does not appear to be a valid number, try again.\n");
                return;
            }


            if (UID > 0 && UID < 9999999)
            {
                photoBomb.getPictureByUID(new SoftwareEng.getPhotoByUID_callback(getPictureByUID_callback), UID);
            }
            else
            {
                output.AppendText("The UID does not appear to be a valid number, try again.\n");
            }

        }


        public void getPictureByUID_callback(SoftwareEng.ErrorReport error, ComplexPhotoData pictureInfo)
        {
            if (error.reportID == SoftwareEng.ErrorReport.FAILURE)
            {
                output.AppendText("Failed to get the picture: " + error.description + "\n");
            }
            else
            {
                output.AppendText("Picture UID: " + pictureInfo.UID + ", path: " + pictureInfo.path + "\n");
            }
        }


        //---------------------------------------------------------------------
        //add a picture button
        private void button1_Click(object sender, EventArgs e)
        {
            ComplexPhotoData data = new ComplexPhotoData();
            data.UID = 999;//this doesn't matter as the backend assignes the real UID.
            data.picturesAlbumName = "myNewPicture";
            data.path = "newPic.jpg";
            data.extension = ".jpg";
            photoBomb.addNewPicture(new SoftwareEng.generic_callback(addPictureButton_callback) , data, 1);

        }

        public void addPictureButton_callback(SoftwareEng.ErrorReport error)
        {
            if (error.reportID == ErrorReport.FAILURE)
            {
                output.AppendText("Failure reported by the backend: " + error.description + "\n");
            }
            else
            {
                output.AppendText("Picture Added\n");
            }
        }


        //---------------------------------------------------------------------
        //add an album
        private void addAlbumButton_Click(object sender, EventArgs e)
        {
            SimpleAlbumData data = new SimpleAlbumData();
            data.albumName = "newAlbum";
            photoBomb.addNewAlbum(new SoftwareEng.generic_callback(addAlbumButton_callback), data);
        }

        public void addAlbumButton_callback(SoftwareEng.ErrorReport error)
        {
            if (error.reportID == ErrorReport.FAILURE)
            {
                output.AppendText("Failure reported by the backend: " + error.description + "\n");
            }
            else
            {
                output.AppendText("Album Added\n");
            }
        }

        //---------------------------------------------------------------------






    }//class




}//namespace

