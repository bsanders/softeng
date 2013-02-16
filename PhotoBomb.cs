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

        //The XML in memory for the albumbs.
        //Add new vars here if we get more xmls.
        XDocument _albumsXDocs;
        XDocument _picturesXDocs;

        //----------------------------------------------
        //By: Ryan Moe
        //Edited Last: 
        //
        //initialize.
        public PhotoBomb()
        {
            _albumsXDocs = null;
            _picturesXDocs = null;

            xmlParser = new XmlParser();
        }

        //----------------------------------------------
        //By: Ryan Moe
        //Edited Last: 
        //
        //open the xml document that represents the
        //user's albums in the program.
        //PARAM 1 = a callback (delegate) to a gui function (see PhotoBombDelegates.cs).
        //PARAM 2 = The path to the album xml, THIS MAY BE REMOVED.
        //
        //ERROR CONDITIONS
        //1) if the xml file does not exist, an error will be returned.
        //2) if the xml file does not contain VALID xml, error.
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
                error.description = "PhotoBomb.openAlbumsXML():failed to load the albums xml file: " + xmlPath;
                guiCallback(error);
                return;
            }

            //The loading of the xml was nominal, report back to the gui callback.
            error.description = "great success!";
            guiCallback(error);
        }

        //----------------------------------------------
        //By: Ryan Moe
        //Edited Last: 
        //
        //Save the album data to an xml file.
        //PARAM 1 = a gui callback (see PhotoBombDelegates.cs).
        //PARAM 2 = path to where you want to save the xml.
        public void saveAlbumsXML(generic_callback guiCallback, string xmlSavePath)
        {
            Error error = new Error();
            //error checking
            if (_albumsXDocs == null)
            {
                error.id = Error.FAILURE;
                error.description = "PhotoBomb:saveAlbumsXML:No Album loaded to save.";
                guiCallback(error);
                return;
            }


            guiCallback(error);
        }
        //----------------------------------------------
        //By: Ryan Moe
        //Edited Last: 
        //
        //This method returns a list of Album objects to the
        //callback given by the parameter.
        //PARAM 1 = a gui callback (see PhotoBombDelegates.cs).
        public void getAllUserAlbumNames(getAlbumbsCallback guiCallback)
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




    //-------------------------------------------------



    //BY: Ryan Moe
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