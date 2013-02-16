/**
 * This is the main program for the PhotoBomb backend.
 * These are the function calls for the GUI to call.
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SoftwareEng
{
    partial class PhotoBomb
    {

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
            ErrorReport error = new ErrorReport();

            try
            {
                _albumsXDocs = XDocument.Load(xmlPath);
            }
            catch
            {
                error.id = ErrorReport.SHIT_JUST_GOT_REAL;
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
            ErrorReport error = new ErrorReport();
            //error checking
            if (_albumsXDocs == null)
            {
                error.id = ErrorReport.FAILURE;
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
        public void getAllUserAlbumNames(getAllUserAlbumNames_callback guiCallback)
        {
            ErrorReport error = new ErrorReport();

            //error checking
            if (_albumsXDocs == null)
            {
                error.id = ErrorReport.FAILURE;
                error.description = "PhotoBomb.getAllUserAlbumNames():Albums xml has not been loaded yet!";
                guiCallback(error, null);
                return;
            }

            //the list of all albums to return to the gui.
            List<UserAlbum> _albumsToReturn = new List<UserAlbum>();

            //get all the albums AND their children.
            List<XElement> _albumSearch;
            try
            {  
                _albumSearch = xmlParser.searchForElements(_albumsXDocs, "album");
            }
            catch
            {
                error.id = ErrorReport.FAILURE;//maybe every error should be SHIT_JUST_GOT_REAL?  Decisions, decisions...
                error.description = "PhotoBomb.getAllUserAlbumNames():Failed at finding albums in the database.";
                guiCallback(error, null);
                return;
            }

            //go through each album and get data from its children to add to the list.
            foreach (XElement elem in _albumSearch) 
            {
                //this gets sent back to the gui.
                UserAlbum userAlbum = new UserAlbum();
                    
                //need this inner try because if this fails to find the name it would go
                //to the outer catch and not continue the foreach loop looking for albums.
                List<XElement> _nameSearch;
                try
                {
                    //get the name(s) of this album.
                    _nameSearch = xmlParser.searchForElements(elem, "name");
                }
                catch
                {
                    error.id = ErrorReport.SUCCESS_WITH_WARNINGS;
                    error.warnings.Add("PhotoBomb.getAllUserAlbumNames():Had an error finding the name of an album.");
                    continue;
                }
                //make sure we have at least one name for the album.
                if (_nameSearch.Count == 1)
                {
                    //get the value of the album name and add to list.
                    //userAlbum.albumName = _nameSearch.ElementAt(0).Value;
                    try
                    {
                        userAlbum.albumName = _nameSearch.ElementAt(0).Attribute("value").Value;
                    }
                    catch
                    {
                        error.id = ErrorReport.SUCCESS_WITH_WARNINGS;
                        error.warnings.Add("PhotoBomb.getAllUserAlbumNames():Had an error trying to get the name value from an album.");
                        continue;
                    }
                    _albumsToReturn.Add(userAlbum);
                }
                else if(_nameSearch.Count > 1)
                {
                    error.id = ErrorReport.SUCCESS_WITH_WARNINGS;
                    error.warnings.Add("PhotoBomb.getAllUserAlbumNames():Found an album with more than one name.");
                }
                else if (_nameSearch.Count == 0)
                {
                    error.id = ErrorReport.SUCCESS_WITH_WARNINGS;
                    error.warnings.Add("PhotoBomb.getAllUserAlbumNames():Found an album with no name.");
                }

            }//foreach

            guiCallback(error, _albumsToReturn);
        }


    }//class




    //-------------------------------------------------



    //BY: Ryan Moe
    //This is a data class that will be used
    //to send a calling gui a list of albums.
    //This class is a SINGLE element of that list.
    public class UserAlbum
    {
        public String albumName;
        public int UID;
        //add more information here if needed...

        //initialize vars.
        public UserAlbum()
        {
            albumName = "";
            UID = -1;//indicates UID not set.
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