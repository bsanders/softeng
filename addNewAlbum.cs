using System;
using System.Configuration;
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

        private ErrorDialogForm errorBox;

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        public addNewAlbum(mainGUI localMainWindowRef)
        {
            mainWindowRef = localMainWindowRef;

            InitializeComponent();
        }


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        private void createTheNewAlbum()
        {
            if (stringChecker(albumNameTextBox.Text) == false)
            {
                showError("Invalid album name.");

                finishButton.Enabled = true;
            }
            else
            {
                mainWindowRef.guiCheckAlbumNameIsUnique(albumNameTextBox.Text, albumNameAccepted);
            }
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        public void albumNameAccepted(ErrorReport status)
        {
            if (status.reportID != ErrorReport.SUCCESS)
            {
                showError(status.description);

                finishButton.Enabled = true;
            }
            else
            {
                mainWindowRef.guiCreateThisAlbum(albumNameTextBox.Text, albumAdded);
            }
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        public void albumAdded(ErrorReport status)
        {
            if (status.reportID != ErrorReport.SUCCESS)
            {
                showError(status.description);

                finishButton.Enabled = true;
            }
            else
            {
                DialogResult = DialogResult.OK;
                //MessageBox.Show("New Album Successfully Created", "New Album", MessageBoxButtons.OK);
                this.Close();
            }
        }


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        private void finishButton_Click(object sender, EventArgs e)
        {
            finishButton.Enabled = false ;
            createTheNewAlbum();
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        private void showError(string theErrorMessage)
        {
            errorBox = new ErrorDialogForm(theErrorMessage);

            errorBox.ShowDialog();

            //MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK);
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
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


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        private bool stringChecker(string target)
        {
            RegexStringValidator inputChecker = new RegexStringValidator(validInputKey);

            try
            {
                inputChecker.Validate(target);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }       
    }
}
