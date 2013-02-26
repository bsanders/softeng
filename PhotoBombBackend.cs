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

        //path to the pictures folder we put all the pictures
        //tracked by the database.
        private string picturePath;

        //The XML in memory for the albumbs.
        //Add new vars here if we get more xmls.
        private XDocument _albumsDatabase;
            private string albumsDatabasePath;
        private XDocument _picturesDatabase;
            private string picturesDatabasePath;

        //-----------------------------------------------------------------
        //FUNCTIONS--------------------------------------------------------
        //-----------------------------------------------------------------

        private void init(generic_callback guiCallback, string albumDatabasePathIn, string pictureDatabasePathIn, string pictureFolderPathIn)
        {
            ErrorReport errorReport = new ErrorReport();

            albumsDatabasePath = albumDatabasePathIn;
            picturesDatabasePath = pictureDatabasePathIn;
            picturePath = pictureFolderPathIn;

            xmlParser = new XmlParser();

            openAlbumsXML(errorReport);
            openPicturesXML(errorReport);

            guiCallback(errorReport);
        }

        //-----------------------------------------------------------------

        //By: Ryan Moe
        //Edited Last:
        private void reopenAlbumsXML_backend(generic_callback guiCallback){
            //use this to inform the calling gui of how things went.
            ErrorReport error = new ErrorReport();

            openAlbumsXML(error);

            guiCallback(error);
        }

        //-----------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        private void saveAlbumsXML_backend(generic_callback guiCallback)
        {
            ErrorReport error = new ErrorReport();

            //make sure the album database is valid.
            if (!checkDatabaseIntegrity(_albumsDatabase, error))
            {
                //make an error message here.

                guiCallback(error);
                return;
            }

            _albumsDatabase.Save(albumsDatabasePath);

            if (guiCallback != null)
                guiCallback(error);
        }

        //-----------------------------------------------------------------

        //By: Ryan Moe
        //Edited Last:
        private void reopenPicturesXML_backend(generic_callback guiCallback)
        {
            //use this to inform the calling gui of how things went.
            ErrorReport error = new ErrorReport();

            try
            {
                _picturesDatabase = XDocument.Load(picturesDatabasePath);
            }
            catch
            {
                error.reportID = ErrorReport.FAILURE;
                error.description = "PhotoBomb.openPicturesXML():failed to load the albums xml file: " + picturesDatabasePath;
                guiCallback(error);
                return;
            }

            //The loading of the xml was nominal, report back to the gui callback.
            error.description = "great success!";
            guiCallback(error);
        }

        //-----------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        private void savePicturesXML_backend(generic_callback guiCallback)
        {
            ErrorReport error = new ErrorReport();

            //if the database is NOT valid.
            if (!checkDatabaseIntegrity(_picturesDatabase, error))
            {
                //make an error message here.

                guiCallback(error);
                return;
            }

            _picturesDatabase.Save(picturesDatabasePath);
            if(guiCallback != null)
                guiCallback(error);
        }

        //-----------------------------------------------------------------


        //By: Ryan Moe
        //Edited Last:
        //NOTE: This function is up for replacement by new logic that uses
        //the select/from/where style of code.
        private void getAllUserAlbumNames_backend(getAllUserAlbumNames_callback guiCallback)
        {
            ErrorReport error = new ErrorReport();

            //if the database is NOT valid.
            if (!checkDatabaseIntegrity(_albumsDatabase, error))
            {
                error.reportID = ErrorReport.FAILURE;
                error.description = "The album database was determined to be not valid.";
                guiCallback(error, null);
                return;
            }

            //the list of all albums to return to the gui.
            List<SimpleAlbumData> _albumsToReturn = new List<SimpleAlbumData>();

            //get all the albums AND their children.
            List<XElement> _albumSearch;
            try
            {
                _albumSearch = xmlParser.searchForElements(_albumsDatabase.Element("database"), "album");
            }
            catch
            {
                error.reportID = ErrorReport.FAILURE;//maybe every error should be SHIT_JUST_GOT_REAL?  Decisions, decisions...
                error.description = "PhotoBomb.getAllUserAlbumNames():Failed at finding albums in the database.";
                guiCallback(error, null);
                return;
            }
            //go through each album and get data from its children to add to the list.
            foreach (XElement thisAlbum in _albumSearch)
            {
                //this custom data class gets sent back to the gui.
                SimpleAlbumData userAlbum = new SimpleAlbumData();

                List<XElement> _nameSearch;
                try
                {
                    //get the name(s) of this album.
                    _nameSearch = xmlParser.searchForElements(thisAlbum, "albumName");
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
                        userAlbum.albumName = _nameSearch.ElementAt(0).Value;
                        userAlbum.UID = (int)thisAlbum.Attribute("uid");
                    }
                    catch
                    {
                        error.reportID = ErrorReport.SUCCESS_WITH_WARNINGS;
                        error.warnings.Add("PhotoBomb.getAllUserAlbumNames():Had an error trying to get either the name or uid value from an album.");
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
        //By: Ryan Moe
        //Edited Last:
        private void getAllPhotosInAlbum_backend(getAllPhotosInAlbum_callback guiCallback, int AlbumUID)
        {
            ErrorReport error = new ErrorReport();

            //make sure the album database is valid.
            if (!checkDatabaseIntegrity(_albumsDatabase, error))
            {
                guiCallback(error, null);
                return;
            }

            //Try searching for the album with the uid specified.
            XElement specificAlbum;
            try
            {
                //for(from) every c in the database's children (all albums),
                //see if it's attribute uid is the one we want,
                //and if so return the first instance of a match.
                specificAlbum = (from c in _albumsDatabase.Element("database").Elements()
                                 where (int)c.Attribute("uid") == AlbumUID
                                 select c).Single();//NOTE: this will throw error if more than one OR none at all.
            }
            catch
            {
                error.reportID = ErrorReport.FAILURE;
                error.description = "PhotoBomb.getAllPhotosInAlbum():Failed to find the album specified.";
                guiCallback(error, null);
                return;
            }
    
            //Now lets get all the picture data from
            //the album and fill out the picture object list.
            List<SimplePhotoData> _list = new List<SimplePhotoData>();
            foreach (XElement subElement in specificAlbum.Element("albumPhotos").Elements("picture"))
            {
                SimplePhotoData pic = new SimplePhotoData();
                try
                {
                    pic.UID = (int)subElement.Attribute("uid");
                    pic.pictureName = (string)subElement.Element("name").Value;
                    _list.Add(pic);
                }
                catch
                {
                    error.reportID = ErrorReport.SUCCESS_WITH_WARNINGS;
                    error.warnings.Add("PhotoBomb.getAllPhotosInAlbum():A Picture in the album is missing either a name or an id.");
                }
            }//foreach

            guiCallback(error, _list);
        }//method

        //----------------------------------------------------------------------

        //By: Ryan Moe
        //Edited Last:
        private void getPictureByUID_backend(getPhotoByUID_callback guiCallback, int uid)
        {
            ErrorReport error = new ErrorReport();

            XElement picElement = getPictureElementByUID(error, uid);

            //if the picture finding function reported success.
            if (error.reportID == ErrorReport.SUCCESS || error.reportID == ErrorReport.SUCCESS_WITH_WARNINGS)
            {
                ComplexPhotoData photo = new ComplexPhotoData();
                try
                {
                    photo.UID = (int)picElement.Attribute("uid");
                    photo.path = (string)picElement.Element("filePath").Value;
                }
                catch
                {
                    error.reportID = ErrorReport.FAILURE;
                    error.description = "PhotoBomb.getPictureByUID():Photo info could not be loaded.";
                    guiCallback(error, null);
                    return;
                }
                //Success!
                guiCallback(error, photo);
                return;
            }
            else
            {
                guiCallback(error, null);
                return;
            }
        }//method
        

        //-------------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        private void addNewPicture_backend(generic_callback guiCallback, ComplexPhotoData newPicture, int albumUID)
        {
            ErrorReport errorReport = new ErrorReport();

            //get a unique ID for this photo and update its 
            //data object to reflect this new UID.
            int newUID = getNewPictureUID_slow();
            newPicture.UID = newUID;

            addPictureToPictureDatabase(errorReport, newPicture);

            //if adding to the picture database failed
            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                guiCallback(errorReport);
                return;
            }

            addPictureToAlbumDatabase(errorReport, newPicture, albumUID);

            //if adding to the album database failed
            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                guiCallback(errorReport);
                return;
            }

            savePicturesXML_backend(null);
            saveAlbumsXML_backend(null);

            guiCallback(errorReport);
        }




        //-------------------------------------------------------------

        private void addNewAlbum_backend(generic_callback guiCallback, SimpleAlbumData albumData)
        {
            ErrorReport errorReport = new ErrorReport();
            int uid = getNewAlbumUID_slow(errorReport);
            albumData.UID = uid;

            addAlbumToAlbumDatabase(errorReport, albumData);

            saveAlbumsXML_backend(null);

            guiCallback(errorReport);

        }


        //-------------------------------------------------------------

        private void addExistingPictureToAlbum_backend(generic_callback guiCallback, int pictureUID, int albumUID)
        {
            ErrorReport errorReport = new ErrorReport();
            this.getPictureElementByUID(errorReport, pictureUID);
            //get pictureData
            //addPictureToAlbumDatabase(errorReport, pictureData, albumUID);
        }






    }//class
}
