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
    public partial class addNewAlbum : Form
    {
        private mainGUI mainWindowRef;

        private const int userInputMaxSize = 16; 

        /************************************************************
        * 
        ************************************************************/
        public addNewAlbum()
        {
            InitializeComponent();
        }

        
        public addNewAlbum(mainGUI localMainWindowRef)
        {
            mainWindowRef = localMainWindowRef;

            InitializeComponent();
        }
         
        
        /************************************************************
         * 
         ************************************************************/


        /************************************************************
        * 
        ************************************************************/
        private void createTheNewAlbum()
        {
            if (stringChecker(newAlbumMaskedTextBox.Text) == false)
            {
                showError("Invalid album name.");
            }
            else
            {
                mainWindowRef.guiNewAlbumNamed(albumNameTextBox.Text, albumNameAccepted);
            }
        }

        /************************************************************
        * 
        ************************************************************/
        public void albumNameAccepted(ErrorReport status)
        {
            if (status.reportID != ErrorReport.SUCCESS)
            {
                showError("Functionailty not yet finished, LOL");
            }
        }



        /************************************************************
        * 
        ************************************************************/
        private void finishButton_Click(object sender, EventArgs e)
        {

            createTheNewAlbum();
            //this.Close();
        }



        /************************************************************
        * 
        ************************************************************/
        private void showError(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK);
        }


        /************************************************************
        * 
        ************************************************************/
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        /************************************************************
        * 
        ************************************************************/
        private void albumNameTextBox_TextChanged(object sender, EventArgs e)
        {
            finishButton.Enabled = true;
        }


        /************************************************************
        * 
        ************************************************************/
        private bool stringChecker(string target)
        {


            return true;

            /*
            albumNameTextBox.Enabled = false;
            for (int i = 0; i < albumNameTextBox.Text.Length; i++)
            {
                


            }
            */
        }

        private void newAlbumMaskedTextBox_TextChanged(object sender, EventArgs e)
        {
            finishButton.Enabled = true;
        }

        private void newAlbumMaskedTextBox_Enter(object sender, EventArgs e)
        {

        }        
    }
}
