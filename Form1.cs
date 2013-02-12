using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.Xml.Linq;

namespace TestApp
{
    public partial class Form1 : Form
    {

        SoftwareEng.PhotoBomb photoBomb;

        public Form1()
        {
            InitializeComponent();
            photoBomb = new SoftwareEng.PhotoBomb();
        }



        private void loadXML_Click(object sender, EventArgs e)
        {
            photoBomb.openAlbumbsXML("test.xml");
        }//form1()



        private void button2_Click(object sender, EventArgs e)
        {
            photoBomb.saveAlbumbsXML();
        }//method




    }//class




}//namespace

