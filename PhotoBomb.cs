/**
 * This is the main backend program for PhotoBomb.
 * There will be function calls in here for the gui
 * to interact with the backend.
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SoftwareEng
{
    class PhotoBomb
    {
        //xml parsing utils.
        XmlParser xmlParser;

        //the XML in memory for the albumbs
        XDocument _albums;
        XDocument _pictures;

        //----------------------------------------------
        //init.
        public PhotoBomb()
        {
            _albums = null;
            _pictures = null;

            xmlParser = new XmlParser();
        }

        //----------------------------------------------
        //open the xml document that represents the
        //albumbs in the program.
        public void openAlbumsXML(generic_callback guiCallback, string xmlPath)
        {
            //use this to inform the calling gui of how things went.
            Error error = new Error();

            try
            {
                _albums = XDocument.Load(xmlPath);
            }
            catch
            {
                error.id = Error.SHIT_JUST_GOT_REAL;
                error.description = "PhotoBomb:openAlbumsXML:failed to load the albums xml file.";
                guiCallback(error);
                return;
            }

            error.description = "great success!";
            guiCallback(error);
        }

        //----------------------------------------------
        //save the xml document
        public void saveAlbumsXML(generic_callback guiCallback, string xmlSavePath)
        {
            Error error = new Error();
            //error checking
            if (_albums == null)
            {
                error.id = Error.FAILURE;
                error.description = "PhotoBomb:saveAlbumsXML:No Album to save.";
                guiCallback(error);
                return;
            }


            guiCallback(error);
        }
        //----------------------------------------------



    }//class
}//namespace






/*
             *  XDocument anotherDoc = XDocument.Load("test.xml");
            List<XElement> _list = searchForElement(anotherDoc, "picture");

            for (int i = 0; i < _list.Count; ++i)
            {

            }

            //output.AppendText(_list.Count.ToString());
            //output.Text = _list[0].ToString();
             */