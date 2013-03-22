using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace SoftwareEng
{
    public partial class aboutForm : Form
    {

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: none
        * return type: void
        * purpose: initializes object
        *********************************************************************************************/
        public aboutForm()
        {
            InitializeComponent();
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: windows default
        * return type: void
        * purpose: closes form
        *********************************************************************************************/
        private void okButton_Click(object sender, EventArgs e)
        {
//            SoftwareEng
            Close();
        }
    }
}
