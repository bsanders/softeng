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
    public partial class aboutForm : Form
    {

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        public aboutForm()
        {
            InitializeComponent();
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: 
        * return type: 
        * purpose: 
        *********************************************************************************************/
        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
