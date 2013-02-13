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

        //--------------------------------------------------------------------

        //init.
        public Form1()
        {
            InitializeComponent();
            photoBomb = new SoftwareEng.PhotoBomb();
        }

        //--------------------------------------------------------------------

        //test button.
        private void loadXML_Click(object sender, EventArgs e)
        {
            photoBomb.openAlbumsXML(new SoftwareEng.generic_callback(loadXML_Callback), "test.xml");
        }//form1()

        //callback for above test button.
        public void loadXML_Callback(SoftwareEng.Error e)
        {
            if (e.id == SoftwareEng.Error.SUCCESS)
            {
                output.AppendText("XML Loaded\n");
            }
            else
            {
                output.AppendText("XML Not Loaded\n");
            }
        }

        //--------------------------------------------------------------------

        //test save button.
        private void button2_Click(object sender, EventArgs e)
        {
            photoBomb.saveAlbumsXML(new SoftwareEng.generic_callback(saveXML_Callback), "test.xml");
        }//method

        //callback for the above test button
        public void saveXML_Callback(SoftwareEng.Error e)
        {
            if(e.id == SoftwareEng.Error.SUCCESS){
                output.AppendText("XML Saved\n");
            }
            else
            {
                output.AppendText("XML Not Saved\n");
            }
        }


        //--------------------------------------------------------------------


        private void loadAlbums_Click(object sender, EventArgs e)
        {

        }


    }//class




}//namespace

