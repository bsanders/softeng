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
        private PhotoBomb bombaDeFotos;

        private const int userInputMaxSize = 16; 

        /************************************************************
        * 
        ************************************************************/
        public addNewAlbum()
        {
            InitializeComponent();
        }

        public addNewAlbum(PhotoBomb photobombParam)
        {
            bombaDeFotos = photobombParam;

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
                ;
            }
        }

        private void finishButton_Click(object sender, EventArgs e)
        {

            createTheNewAlbum();
            this.Close();
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
