/**
 * This is the backend for the PhotoBomb program.
 * These functions are NOT to be used by a frontend (gui).
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SoftwareEng
{
    //This is a PARTIAL class,
    //it is the private part of the PhotoBomb class.
    partial class PhotoBomb
    {
        
        //xml parsing utils.
        private XmlParser xmlParser;

        //The XML in memory for the albumbs.
        //Add new vars here if we get more xmls.
        private XDocument _albumsXDocs;
        private XDocument _picturesXDocs;

        //-----------------------------------------------------------------
        //FUNCTIONS--------------------------------------------------------
        //-----------------------------------------------------------------

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

        //-----------------------------------------------------------------

        private void openAlbumsXML_backend(generic_callback guiCallback, string xmlPath){
            //use this to inform the calling gui of how things went.
            ErrorReport error = new ErrorReport();

            try
            {
                _albumsXDocs = XDocument.Load(xmlPath);
            }
            catch
            {
                error.reportID = ErrorReport.SHIT_JUST_GOT_REAL;
                error.description = "PhotoBomb.openAlbumsXML():failed to load the albums xml file: " + xmlPath;
                guiCallback(error);
                return;
            }

            //The loading of the xml was nominal, report back to the gui callback.
            error.description = "great success!";
            guiCallback(error);
        }

        //-----------------------------------------------------------------

        private void saveAlbumsXML_backend(generic_callback guiCallback, string xmlSavePath)
        {
            ErrorReport error = new ErrorReport();
            //error checking
            if (_albumsXDocs == null)
            {
                error.reportID = ErrorReport.FAILURE;
                error.description = "PhotoBomb:saveAlbumsXML:No Album loaded to save.";
                guiCallback(error);
                return;
            }


            guiCallback(error);
        }

        //-----------------------------------------------------------------

        private void getAllUserAlbumNames_backend(getAllUserAlbumNames_callback guiCallback)
        {
            ErrorReport error = new ErrorReport();

            //error checking
            if (_albumsXDocs == null)
            {
                error.reportID = ErrorReport.FAILURE;
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
                error.reportID = ErrorReport.FAILURE;//maybe every error should be SHIT_JUST_GOT_REAL?  Decisions, decisions...
                error.description = "PhotoBomb.getAllUserAlbumNames():Failed at finding albums in the database.";
                guiCallback(error, null);
                return;
            }

            //go through each album and get data from its children to add to the list.
            foreach (XElement elem in _albumSearch)
            {
                //this custom data class gets sent back to the gui.
                UserAlbum userAlbum = new UserAlbum();

                List<XElement> _nameSearch;
                try
                {
                    //get the name(s) of this album.
                    _nameSearch = xmlParser.searchForElements(elem, "name");
                }
                catch
                {
                    error.reportID = ErrorReport.SUCCESS_WITH_WARNINGS;
                    error.warnings.Add("PhotoBomb.getAllUserAlbumNames():Had an error finding the name of an album.");
                    continue;
                }
                //make sure we have at least one name for the album.
                if (_nameSearch.Count == 1)
                {
                    //get the value of the album name and add to list.
                    try
                    {
                        userAlbum.albumName = _nameSearch.ElementAt(0).Attribute("value").Value;
                    }
                    catch
                    {
                        error.reportID = ErrorReport.SUCCESS_WITH_WARNINGS;
                        error.warnings.Add("PhotoBomb.getAllUserAlbumNames():Had an error trying to get the name value from an album.");
                        continue;
                    }
                    _albumsToReturn.Add(userAlbum);
                }
                else if (_nameSearch.Count > 1)
                {
                    error.reportID = ErrorReport.SUCCESS_WITH_WARNINGS;
                    error.warnings.Add("PhotoBomb.getAllUserAlbumNames():Found an album with more than one name.");
                }
                else if (_nameSearch.Count == 0)
                {
                    error.reportID = ErrorReport.SUCCESS_WITH_WARNINGS;
                    error.warnings.Add("PhotoBomb.getAllUserAlbumNames():Found an album with no name.");
                }

            }//foreach

            guiCallback(error, _albumsToReturn);
        }

        //-----------------------------------------------------------------








    }//class
}
