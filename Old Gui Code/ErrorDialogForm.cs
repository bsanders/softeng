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
    public partial class ErrorDialogForm : Form
    {

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: a string that carries the error message
        * return type: none
        * purpose: constructor, initializes the object
        *********************************************************************************************/
        public ErrorDialogForm(string errorMessage)
        {
            InitializeComponent();

            errorLabel.Text = errorMessage;
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: unused, an object and EventArgs
        * return type: void
        * purpose: exits the form
        *********************************************************************************************/
        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
