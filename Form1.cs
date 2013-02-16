﻿//Don't expect many comments on this page, its for
//testing the backend and will be nuked once we get
//the specs for the real gui.

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
            photoBomb.openAlbumsXML(new SoftwareEng.generic_callback(loadXML_Callback), xmlPathTE.Text);
        }


        //callback for above test button.
        public void loadXML_Callback(SoftwareEng.ErrorReport e)
        {
            if (e.id == SoftwareEng.ErrorReport.SUCCESS || e.id == SoftwareEng.ErrorReport.SUCCESS_WITH_WARNINGS)
            {
                output.AppendText("XML Loaded\n");
                if(e.id == SoftwareEng.ErrorReport.SUCCESS_WITH_WARNINGS)
                {
                    for (int i = 0; i < e.warnings.Count; ++i)
                    {
                        output.AppendText(e.warnings.ElementAt(i));
                    }
                }
            }

            else
            {
                output.AppendText("Error: " + e.description + "\n\n");
            }
        }

        //--------------------------------------------------------------------

        //test save button.
        private void button2_Click(object sender, EventArgs e)
        {
            photoBomb.saveAlbumsXML(new SoftwareEng.generic_callback(saveXML_Callback), xmlPathTE.Text);
        }//method


        //callback for the above test button
        public void saveXML_Callback(SoftwareEng.ErrorReport e)
        {
            if (e.id == SoftwareEng.ErrorReport.SUCCESS || e.id == SoftwareEng.ErrorReport.SUCCESS_WITH_WARNINGS)
            {
                output.AppendText("XML Saved\n");

                if (e.id == SoftwareEng.ErrorReport.SUCCESS_WITH_WARNINGS)
                {
                    output.AppendText("Warnings:\n");
                    for (int i = 0; i < e.warnings.Count; ++i)
                    {
                        output.AppendText(e.warnings.ElementAt(i));
                    }
                }
            }

            else
            {
                output.AppendText("Error: " + e.description + "\n\n");
            }
        }


        //--------------------------------------------------------------------


        private void loadAlbums_Click(object sender, EventArgs e)
        {
            photoBomb.getAllUserAlbumNames(new SoftwareEng.getAllUserAlbumNames_callback(loadAlbums_Callback));
        }


        public void loadAlbums_Callback(SoftwareEng.ErrorReport error, List<SoftwareEng.UserAlbum> _albums)
        {
            if (error.id == SoftwareEng.ErrorReport.SUCCESS || error.id == SoftwareEng.ErrorReport.SUCCESS_WITH_WARNINGS)
            {
                output.AppendText("Albums Found:\n");
                //output albums
                for (int i = 0; i < _albums.Count; ++i)
                {
                    output.AppendText(_albums.ElementAt(i).albumName + "\n");
                }
                //output warnings
                if (error.id == SoftwareEng.ErrorReport.SUCCESS_WITH_WARNINGS)
                {
                    output.AppendText("\nWarnings:\n");
                    for (int i = 0; i < error.warnings.Count; ++i)
                    {
                        output.AppendText(error.warnings.ElementAt(i) + "\n");
                    }
                }
            }

            else
            {
                output.AppendText("Error: " + error.description + "\n\n");
            }
        }

        //---------------------------------------------------------------------






    }//class




}//namespace

