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
        //This is the main backend of the program.
        SoftwareEng.PhotoBomb photoBomb;


        //init.
        public Form1()
        {
            InitializeComponent();
            photoBomb = new SoftwareEng.PhotoBomb();
        }


        //test button.
        private void loadXML_Click(object sender, EventArgs e)
        {
            photoBomb.openAlbumsXML(new SoftwareEng.openAlbumsXML_Callback(loadXML_Callback), "test.xml");
        }//form1()

        //callback for above test button.
        public void loadXML_Callback(SoftwareEng.Error e)
        {
            output.AppendText(e.description);
        }


        //test button.
        private void button2_Click(object sender, EventArgs e)
        {
            photoBomb.saveAlbumbsXML("test.xml");
        }//method




    }//class




}//namespace

