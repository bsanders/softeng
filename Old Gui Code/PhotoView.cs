using System;
using System.Windows;
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
        * Author: Alejandro Sosa
        * parameters: reference to main window, and a complexphotodata containing info of the photo
        *   to be displayed
        * return type: none
        * purpose: creates a new instance of PhotoViewWindow object
        *********************************************************************************************/
        public PhotoViewWindow(mainGUI localMainWindowRef, ComplexPhotoData wantedPhoto, string myName)
        {
            mainWindowRef = localMainWindowRef;

            photoToDisplay = wantedPhoto;

            InitializeComponent();

            this.Text = myName;

            int appropriateHeight = (int)((SystemParameters.PrimaryScreenHeight)/3)*2;

            int appropriateWidth= (int)((SystemParameters.PrimaryScreenWidth)/3)*2;

            photoboxPanel1.MaximumSize = new System.Drawing.Size(appropriateWidth, appropriateHeight);
            
            //displayPhoto();

            photoBox.LoadAsync(wantedPhoto.path);
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: none
        * return type: void
        * purpose: obsolete
        *********************************************************************************************/
        private void displayPhoto()
        {
            
            photoBox.ImageLocation = photoToDisplay.path;

            photoBox.LoadAsync();
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: windows default
        * return type: void
        * purpose: closes form
        *********************************************************************************************/
        private void okPhotoViewButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
