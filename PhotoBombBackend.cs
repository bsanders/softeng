/**
 * This is the backend for the PhotoBomb program.
 * These functions are NOT to be used by a frontend (gui).
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

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
        private string libraryPath;

        //The XML in memory for the albumbs.
        //Add new vars here if we get more xmls.
        private XDocument _albumsDatabase;
        private string albumsDatabasePath;
        private XDocument _picturesDatabase;
        private string picturesDatabasePath;

        //-----------------------------------------------------------------
        //FUNCTIONS--------------------------------------------------------
        //-----------------------------------------------------------------

        private void init(generic_callback guiCallback, string albumDatabasePathIn, string pictureDatabasePathIn, string libraryPathIn)
        {
            ErrorReport errorReport = new ErrorReport();

            albumsDatabasePath = albumDatabasePathIn;
            picturesDatabasePath = pictureDatabasePathIn;
            libraryPath = libraryPathIn;

            xmlParser = new XmlParser();

            util_openAlbumsXML(errorReport);
            util_openPicturesXML(errorReport);

            guiCallback(errorReport);
        }

        //-----------------------------------------------------------------

        private void rebuildBackendOnFilesystem_backend(generic_callback guiCallback)
        {
            ErrorReport errorReport = new ErrorReport();

            //if the library folder existed, rename it.
            if (Directory.Exists(libraryPath))
            {
                try
                {
                    Directory.Move(libraryPath, (libraryPath + "_backup"));
                }
                catch
                {
                    errorReport.reportID = ErrorReport.FAILURE;
                    errorReport.description = "Couldn't backup (rename) the old library folder.  If you have a library backup already, please remove it.";
                    guiCallback(errorReport);
                    return;
                }
            }//if

            //now make a new library folder
            try
            {
                Directory.CreateDirectory(libraryPath);
            }
            catch
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "Unable to create the new library folder.";
                guiCallback(errorReport);
                return;
            }

            //make the new database xml files
            XDocument initDB = new XDocument();
            XElement root = new XElement("database");
            initDB.Add(root);
            try
            {
                initDB.Save(albumsDatabasePath);
                initDB.Save(picturesDatabasePath);
            }
            catch
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "Unable to create the new database files.";
                guiCallback(errorReport);
                return;
            }

            //Load the new databases into memory.
            util_openAlbumsXML(errorReport);
            util_openPicturesXML(errorReport);
            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                guiCallback(errorReport);
                return;
            }

            saveAlbumsXML_backend(null);
            savePicturesXML_backend(null);

            guiCallback(errorReport);
        }

        //-----------------------------------------------------------------

        //By: Ryan Moe
        //Edited Last:
        private void reopenAlbumsXML_backend(generic_callback guiCallback)
        {
            //use this to inform the calling gui of how things went.
            ErrorReport error = new ErrorReport();

            util_openAlbumsXML(error);

            guiCallback(error);
        }

        //-----------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        private void saveAlbumsXML_backend(generic_callback guiCallback)
        {
            ErrorReport error = new ErrorReport();

            //make sure the album database is valid.
            if (!util_checkAlbumDatabase(error))
            {
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
            if (!util_checkPicturesDatabase(error))
            {
                guiCallback(error);
                return;
            }

            _picturesDatabase.Save(picturesDatabasePath);
            if (guiCallback != null)
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
            if (!util_checkAlbumDatabase(error))
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
            if (!util_checkAlbumDatabase(error))
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
                    pic.picturesNameInAlbum = (string)subElement.Element("name").Value;
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

            XElement picElement = util_getComplexPictureByUID(error, uid);

            //if the picture finding function reported success.
            if (error.reportID == ErrorReport.SUCCESS || error.reportID == ErrorReport.SUCCESS_WITH_WARNINGS)
            {
                ComplexPhotoData photo = new ComplexPhotoData();
                //ComplexPhotoData MOE MARKER MOE MARKER MOE MARKER MOE MARKER!!!!!!
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
        private void addNewPicture_backend(generic_callback guiCallback, String photoUserPath, String photoExtension, int albumUID, String pictureNameInAlbum)
        {
            ErrorReport errorReport = new ErrorReport();
            ComplexPhotoData newPicture = new ComplexPhotoData();

            //get a unique ID for this photo and update its 
            //data object to reflect this new UID.
            newPicture.UID = util_getNewPicUID();
            //error checking
            if (newPicture.UID == -1)
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "Failed to get a UID for a new picture.";
                guiCallback(errorReport);
                return;
            }

            //Change me if you want to start naming the pictures differently in the library.
            String picNameInLibrary = newPicture.UID.ToString() + photoExtension;

            //Change me if you want the default album name to be different.
            if (pictureNameInAlbum == "")
            {
                pictureNameInAlbum = "Image " + newPicture.UID.ToString();
            }

            //Move picture and get a new path for the picture in our storage.
            newPicture.path = util_copyPicToLibrary(errorReport, photoUserPath, picNameInLibrary);
            //error checking
            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                guiCallback(errorReport);
                return;
            }

            newPicture.extension = photoExtension;

            util_addPicToPicDatabase(errorReport, newPicture);

            //if adding to the picture database failed
            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                guiCallback(errorReport);
                return;
            }

            util_addPicToAlbumDatabase(errorReport, newPicture, albumUID, pictureNameInAlbum);

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
            int uid = util_getNewAlbumUID(errorReport);
            albumData.UID = uid;

            util_addAlbumToAlbumDatabase(errorReport, albumData);

            //if adding to the album database failed
            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                guiCallback(errorReport);
                return;
            }

            saveAlbumsXML_backend(null);

            guiCallback(errorReport);

        }


        //-------------------------------------------------------------

        private void addExistingPictureToAlbum_backend(generic_callback guiCallback, int pictureUID, int albumUID, String SimplePhotoData)
        {
            ErrorReport errorReport = new ErrorReport();

            XElement picture = util_getComplexPictureByUID(errorReport, pictureUID);
            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                guiCallback(errorReport);
                return;
            }

            util_addPicToAlbumDatabase(errorReport, null, albumUID, SimplePhotoData);
            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                guiCallback(errorReport);
                return;
            }

            guiCallback(errorReport);
        }//method

        //--------------------------------------------------------------

        private void checkIfAlbumNameIsUnique_backend(generic_callback guiCallback, String albumName)
        {
            ErrorReport errorReport = new ErrorReport();
            Boolean nameUnique = util_checkAlbumNameIsUnique(albumName);

            if (!nameUnique)
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "Album name is not unique.";
                guiCallback(errorReport);
                return;
            }

            guiCallback(errorReport);
        }

        //--------------------------------------------------------------

        private void changePhotoNameByUID_backend(generic_callback guiCallback, int albumUID, int photoUID, String newName)
        {
            ErrorReport errorReport = new ErrorReport();

            XElement album = util_getAlbumByUID(errorReport, albumUID);

            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                guiCallback(errorReport);
                return;
            }

            XElement photo = util_getPhotoFromAlbumElemByUID(errorReport, album, photoUID);

            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                guiCallback(errorReport);
                return;
            }

            util_changePhotoNameInAlbumPhotoElem(errorReport, photo, newName);

            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                guiCallback(errorReport);
                return;
            }

            saveAlbumsXML_backend(null);
        }







    }//class
}
