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

        //------------------------------------------------------

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
                    error.reportID = ErrorReport.SUCCESS_WITH_WARNINGS;
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
    }
}
