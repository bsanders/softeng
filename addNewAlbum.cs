using System;
//using System.Configuration;
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

        private const string validInputKey = @"^[\w\d][\w\d ]{0,14}[\w\d]$";

        private const int userInputMaxSize = 16; 

        /************************************************************
        * constructors
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
            if (stringChecker(albumNameTextBox.Text) == false)
            {
                showError("Invalid album name.");
            }
            else
            {
                mainWindowRef.guiCheckAlbumNameIsUnique(albumNameTextBox.Text, albumNameAccepted);
            }
        }

        /************************************************************
        * 
        ************************************************************/
        public void albumNameAccepted(ErrorReport status)
        {
            if (status.reportID != ErrorReport.SUCCESS)
            {
                showError("Album name not unique");
            }
            else
            {
                ;
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
            if (albumNameTextBox.Text.Length > 0)
            {
                finishButton.Enabled = true;
            }
            else
            {
                finishButton.Enabled = false;
            }
        }


        /************************************************************
        * 
        ************************************************************/
        private bool stringChecker(string target)
        {
            //RegexStringValidator a;

            return true;

            /*
            albumNameTextBox.Enabled = false;
            for (int i = 0; i < albumNameTextBox.Text.Length; i++)
            {
                


            }
            */
        }       
    }
}
