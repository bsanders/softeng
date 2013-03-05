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
    public partial class PhotoViewWindow : Form
    {
        private mainGUI mainWindowRef;

        private ComplexPhotoData photoToDisplay;

        /*********************************************************************************************
        * parameters: a reference to the main GUI window, 
        * return type:
        * purpose: 
        *********************************************************************************************/
        public PhotoViewWindow(mainGUI localMainWindowRef, ComplexPhotoData wantedPhoto, string myName)
        {
            mainWindowRef = localMainWindowRef;

            photoToDisplay = wantedPhoto;

            InitializeComponent();

            this.Text = myName;

            displayPhoto();

            //photoBox.LoadAsync(wantedPhoto.path);
        }

        /*********************************************************************************************
        * parameters:
        * return type:
        * purpose: 
        *********************************************************************************************/
        private void displayPhoto()
        {
            
            photoBox.ImageLocation = photoToDisplay.path;

            photoBox.LoadAsync();
        }

        /*********************************************************************************************
        * parameters:
        * return type:
        * purpose: 
        *********************************************************************************************/
        private void okPhotoViewButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
