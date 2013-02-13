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
    //------------------------------------------------------
        //xml parsing utils.
        XmlParser xmlParser;

        //the XML in memory for the albumbs
        XDocument _albumsXDocs;
        XDocument _picturesXDocs;

        //----------------------------------------------
        //init.
        public PhotoBomb()
        {
            _albumsXDocs = null;
            _picturesXDocs = null;

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
                _albumsXDocs = XDocument.Load(xmlPath);
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
            if (_albumsXDocs == null)
            {
                error.id = Error.FAILURE;
                error.description = "PhotoBomb:saveAlbumsXML:No Album to save.";
                guiCallback(error);
                return;
            }


            guiCallback(error);
        }
        //----------------------------------------------

        //This method returns a list of Album objects to
        //the parameter callback function.
        public void getUserAlbumbs(getAlbumbsCallback guiCallback)
        {
            Error error = new Error();
            List<UserAlbum> _albums = new List<UserAlbum>();

            try
            {
                List<XElement> search = xmlParser.searchForElement(_albumsXDocs, "album");
                //foreach()
            }
            catch
            {
                error.id = Error.FAILURE;
                error.description = "failed to ...";
            }
            //fill out albums list...

            guiCallback(error, _albums);
        }


    }//class


    //This is a data class that will be used
    //to send a calling gui a list of albums.
    //This class is a SINGLE element of that list.
    public class UserAlbum
    {
        String albumName;
        //add more information here...

        public UserAlbum()
        {
            albumName = "";
        }
    }


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