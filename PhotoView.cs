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

        public PhotoViewWindow(mainGUI localMainWindowRef, ComplexPhotoData wantedPhoto, string myName)
        {
            mainWindowRef = localMainWindowRef;

            photoToDisplay = wantedPhoto;

            InitializeComponent();

            this.Text = myName;

            displayPhoto();

            //photoBox.LoadAsync(wantedPhoto.path);
        }

        private void displayPhoto()
        {
            
            photoBox.ImageLocation = photoToDisplay.path;

            photoBox.LoadAsync();
        }

        private void okPhotoViewButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
