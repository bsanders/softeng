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
        public progressForm( int maxBarUnits)
        {
            InitializeComponent();

            importProgressBar.Maximum = maxBarUnits;
        }

        public void updateProgress(int amount)
        {
            importProgressBar.Step = amount;

            importProgressBar.PerformStep();
        }

        public void finished()
        {
            progressLabel.Text = "Pictures successfully imported!";
        }

    }
}
