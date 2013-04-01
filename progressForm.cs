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

        public progressForm( int maxBarUnits, PhotoBomb backend)
        {

            bombaDeFotos = backend;

            InitializeComponent();

            importProgressBar.Maximum = maxBarUnits;

            finishButton.Enabled = false;

            cancelButton.BringToFront();

            DialogResult = DialogResult.None;
        }

        public void updateProgress(int amount)
        {
            progressLabel.Text = "Pictures Importing ...";

            importProgressBar.Step = amount;

            importProgressBar.PerformStep();
        }

        public void finished()
        {
            progressLabel.Text = "Pictures successfully imported!";

            cancelButton.Enabled = false;
            finishButton.Enabled = true;
            finishButton.BringToFront();
        }

        private void finishButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
            
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            ErrorReport status= bombaDeFotos.cancelAddNewPicturesThread();

            Close();
        }

    }
}
