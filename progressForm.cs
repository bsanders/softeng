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
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        public progressForm( int maxBarUnits, PhotoBomb backend)
        {

            bombaDeFotos = backend;

            InitializeComponent();

            importProgressBar.Maximum = maxBarUnits;

            finishButton.Enabled = false;

            cancelButton.BringToFront();

            DialogResult = DialogResult.None;
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        public void updateProgress(int amount)
        {
            progressLabel.Text = "Pictures Importing ...";

            importProgressBar.Step = amount;

            importProgressBar.PerformStep();
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
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
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        private void finishButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
            
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            ErrorReport status= bombaDeFotos.cancelAddNewPicturesThread();

            Close();
        }
    }
}
