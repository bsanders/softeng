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

        //----------------------------------------------
        //init.
        public PhotoBomb()
        {
            xmlParser = new XmlParser();
        }

        //----------------------------------------------
        //open the xml document that represents the
        //albumbs in the program.
        public void openAlbumsXML(openAlbumsXML_Callback guiCallback, string path)
        {
            //use this to inform the calling gui of how things went.
            Error error = new Error();

            try
            {
                _albums = XDocument.Load(path);
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
            return; 
        }

        //----------------------------------------------
        //save the xml document
        public void saveAlbumbsXML(string path)
        {


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