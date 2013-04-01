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
    public partial class progressForm : Form
    {
        public PhotoBomb bombaDeFotos;


        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: int that specifies total sections on the progress bar, and a reference to the backed
        * return type: none
        * purpose: initializes progressForm object
        *********************************************************************************************/
        public progressForm( int maxBarUnits, PhotoBomb backend)
        {

            bombaDeFotos = backend;

            InitializeComponent();

            //-- set progress bar sections to maxBarUnits
            importProgressBar.Maximum = maxBarUnits;

            finishButton.Enabled = false;

            //--cancel button and finish button are in the same spot, so make sure cencel button
            //--remains on top
            cancelButton.BringToFront();

            DialogResult = DialogResult.None;
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: number of blocks to increment progress bar
        * return type: void
        * purpose: updates progress bar
        *********************************************************************************************/
        public void updateProgress(int amount)
        {
            progressLabel.Text = "Pictures Importing ...";

            importProgressBar.Step = amount;

            importProgressBar.PerformStep();
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: none
        * return type: void
        * purpose: call this once backend is done  
        *********************************************************************************************/
        public void finished()
        {
            progressLabel.Text = "Pictures successfully imported!";

            cancelButton.Enabled = false;
            finishButton.Enabled = true;
            finishButton.BringToFront();
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: windows default
        * return type: void
        * purpose: simply closes the form
        *********************************************************************************************/
        private void finishButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
            
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: windows default
        * return type: void
        * purpose: calls the cancel function for the backend before closing form
        *********************************************************************************************/
        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            ErrorReport status= bombaDeFotos.cancelAddNewPicturesThread();

            Close();
        }
    }
}
