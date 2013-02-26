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
        private guiCreateAlbumDelegate form1Delegate;

        private const int userInputMaxSize = 16; 

        /************************************************************
        * 
        ************************************************************/
        public addNewAlbum()
        {
            InitializeComponent();
        }

        public addNewAlbum(guiCreateAlbumDelegate form1DelegateParam)
        {
            form1Delegate = form1DelegateParam;

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
            if (stringChecker(newAlbumMaskedTextBox.Text, userInputMaxSize) == false)
            {
                showError("Invalid album name.");
            }
            else
            {
                form1Delegate(newAlbumMaskedTextBox.Text);
            }
        }

        private void finishButton_Click(object sender, EventArgs e)
        {

            createTheNewAlbum();
        }

        /************************************************************
        * 
        ************************************************************/
        private bool stringChecker(string target, int maxSize)
        {
            if ((target.Length >= maxSize) || (target.Length < 1))
            {
                return false;
            }
            else
            {
                ;
            }
            return true;
        }

        /************************************************************
        * 
        ************************************************************/
        private void showError(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Deez Nutz 2!", MessageBoxButtons.OK);
        }
    }
}
