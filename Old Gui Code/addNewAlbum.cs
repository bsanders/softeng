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
        
        //-- contains the regex to check user inputs against
        private const string validInputKey = @"^[\w\d][\w\d ]{0,14}[\w\d]$";
        
        //-- the maximum length of an album name according to srs
        private const int userInputMaxSize = 16;
        
        //-- this class's error window object
        private ErrorDialogForm errorBox;

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: the Main Window of the program
        * return type: none
        * purpose: creates a new instance of addNewAlbum
        *********************************************************************************************/
        public addNewAlbum(mainGUI localMainWindowRef)
        {
            mainWindowRef = localMainWindowRef;

            InitializeComponent();
        }


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: none
        * return type: void
        * purpose: workhorse function that handles the finish button click
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
        * parameters: ErrorReport to check if back end successful 
        * return type: void
        * purpose: determines course of action depending on whether name is valid and unique
        *********************************************************************************************/
        public void albumNameAccepted(ErrorReport status)
        {
            if (status.reportID != ErrorReport.SUCCESS)
            {
                showError("Invalid album name.");

                finishButton.Enabled = true;
            }
            else
            {
                mainWindowRef.guiCreateThisAlbum(albumNameTextBox.Text, albumAdded);
            }
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: ErrorReport to check if back end successful 
        * return type: void
        * purpose: 
        *********************************************************************************************/
        public void albumAdded(ErrorReport status)
        {
            if (status.reportID != ErrorReport.SUCCESS)
            {
                // no error specified in srs.
                finishButton.Enabled = true;
            }
            else
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: windows default
        * return type: void
        * purpose: disables the finishbutton( to prevent click spam), and then calls a workhorse function
        *********************************************************************************************/
        private void finishButton_Click(object sender, EventArgs e)
        {
            finishButton.Enabled = false ;
            createTheNewAlbum();
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: string containing the error message to be displayed
        * return type: void
        * purpose: displays an error dialog
        *********************************************************************************************/
        private void showError(string theErrorMessage)
        {
            errorBox = new ErrorDialogForm(theErrorMessage);

            errorBox.ShowDialog();

            //MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK);
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: windows default
        * return type: void
        * purpose: close the form
        *********************************************************************************************/
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: windows default
        * return type: void
        * purpose: to enable/disable the finish button based on whether there is text in the text box
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
        * parameters: string containing the text to be checked
        * return type: bool that checks to see if input string is valid
        * purpose: checks to see if input string is valid
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
