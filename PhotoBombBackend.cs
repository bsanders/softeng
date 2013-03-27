/**
 * This is the backend for the PhotoBomb program.
 * These functions are NOT to be used by a frontend (gui).
 * 
 * These functions should be written in a "conductor" sort of
 * way, in that they should try and call as many small functions
 * as they can other classes (like the utils class).
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.ComponentModel;
using System.Threading;
using System.Reflection;

namespace SoftwareEng
{

    //This is a PARTIAL class,
    //it is the private part of the PhotoBomb class.
    public partial class PhotoBomb
    {

        //xml parsing utils.
        //private XmlParser xmlParser;

        //path to the pictures folder we put all the pictures
        //tracked by the database.
        private string libraryPath;

        //The XML in memory for the albumbs.
        //Add new vars here if we get more xmls.
        private XElement _albumsDatabase;
        private string albumsDatabasePath;

        private XElement _picturesDatabase;
        private string picturesDatabasePath;

        private const int UID_MAX_SIZE = 2000000000;


        //-----------------------------------------------------------------
        //FUNCTIONS--------------------------------------------------------
        //-----------------------------------------------------------------

        //By: Ryan Moe
        //Edited Last:
        private void init_backend(generic_callback guiCallback, string albumDatabasePathIn, string pictureDatabasePathIn, string libraryPathIn)
        {
            ErrorReport errorReport = new ErrorReport();

            //keep the paths to databases and library.
            albumsDatabasePath = albumDatabasePathIn;
            picturesDatabasePath = pictureDatabasePathIn;
            libraryPath = libraryPathIn;

            //this might be depricated with the current backend design,
            //think about moving it into the utils class...
            //xmlParser = new XmlParser();

            //try to open the databases.
            // BS: These functions are being slated for merging together
            // Ooops.  If these functions fail, but the third check succeeds, it overwrites errorReport
            util_openAlbumsXML(errorReport);
            util_openPicturesXML(errorReport);

            //check the library directory.
            util_checkLibraryDirectory(errorReport);

            guiCallback(errorReport);
        }

        //-----------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        // I understand what this function does, but not entirely when/why it does it
        private void rebuildBackendOnFilesystem_backend(generic_callback guiCallback)
        {
            ErrorReport errorReport = new ErrorReport();

            //if the library folder existed, rename it.
            if (Directory.Exists(libraryPath))
            {
                //if a backup already exists, throw error.
                try
                {
                    Directory.Move(libraryPath, (libraryPath + Properties.Settings.Default.PhotoLibraryBackupName));
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
            XElement root = new XElement(Properties.Settings.Default.XMLRootElement);
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
            // BS: These functions are being slated for merging together
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

        //BS: Commenting this function out: its never used 03/23/13
        //-----------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //private void reopenAlbumsXML_backend(generic_callback guiCallback)
        //{
        //    //use this to inform the calling gui of how things went.
        //    ErrorReport error = new ErrorReport();

        //    util_openAlbumsXML(error);

        //    guiCallback(error);
        //}

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

            _albumsDatabase.Document.Save(albumsDatabasePath);

            if (guiCallback != null)
                guiCallback(error);
        }

        //BS: Commenting this function out: its never used 03/23/13
        //-----------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //private void reopenPicturesXML_backend(generic_callback guiCallback)
        //{
        //    //use this to inform the calling gui of how things went.
        //    ErrorReport error = new ErrorReport();

        //    try
        //    {
        //        _picturesDatabase = XDocument.Load(picturesDatabasePath);
        //    }
        //    catch
        //    {
        //        error.reportID = ErrorReport.FAILURE;
        //        error.description = "PhotoBomb.openPicturesXML():failed to load the albums xml file: " + picturesDatabasePath;
        //        guiCallback(error);
        //        return;
        //    }

        //    //The loading of the xml was nominal, report back to the gui callback.
        //    guiCallback(error);
        //}

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
            
            _picturesDatabase.Document.Save(picturesDatabasePath);
            if (guiCallback != null)
                guiCallback(error);
        }

        //-----------------------------------------------------------------
        // By: Bill Sanders, based on Ryan Moe's earlier function
        // last edited: 3/24/13
        /// <summary>
        /// Retrieves a list of all albums in the albums.xml file, sent back via the callback.
        /// </summary>
        /// <param name="guiCallback">The callback to send the data back to the GUI</param>
        private void getAllAlbums_backend(getAllAlbumNames_callback guiCallback)
        {
            ErrorReport error = new ErrorReport();

            // Ensure the database is valid before proceeding
            if (!util_checkAlbumDatabase(error))
            {
                error.reportID = ErrorReport.FAILURE;
                error.description = "The album database was determined to be not valid.";
                guiCallback(error, null);
                return;
            }

            // This list will hold all the data the GUI needs for all albums in XML
            List<SimpleAlbumData> _albumsToReturn = new List<SimpleAlbumData>();
            
            // An object to enumerate over the album XML nodes
            IEnumerable<XElement> _albumSearchIE;
            try
            {
                // get all the albums
                _albumSearchIE = (from c in _albumsDatabase.Elements() select c);
            }
            catch
            {
                error.reportID = ErrorReport.FAILURE;
                error.description = "PhotoBomb.getAllAlbumsByID_backend():Failed at finding albums in the database.";
                guiCallback(error, null);
                return;
            }
            foreach (XElement thisAlbum in _albumSearchIE)
            {
                SimpleAlbumData userAlbum = new SimpleAlbumData();

                userAlbum.UID = (int)thisAlbum.Attribute("uid");
                try
                {
                    // Throws an exception if there is not exactly one albumName for a given album.
                    userAlbum.albumName = thisAlbum.Descendants("albumName").Single().Value;
                }
                catch
                {
                    // This is ugly.  If we're upgrading to .net 4.5 anyway we can replace all error code with a tracking class:
                    // http://msdn.microsoft.com/en-us/library/system.runtime.compilerservices.callermembernameattribute.aspx
                    error.reportID = ErrorReport.SUCCESS_WITH_WARNINGS;
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("{0}.{1} : {2}",
                        this.GetType().Name,
                        MethodBase.GetCurrentMethod().Name,
                        "Found an album with more than one name, or invalid name."
                        );
                    error.warnings.Add(sb.ToString());
                    continue; // We'll keep going if one album is invalid
                }

                _albumsToReturn.Add(userAlbum);
            }
            guiCallback(error, _albumsToReturn);
        }

        // 3/24/2013, BS: Commenting this function out,
        // having replaced it with the one above: getAllAlbums_backend()
        //By: Ryan Moe
        //Edited Last:
        //NOTE: This function is up for replacement by new logic that uses
        //the select/from/where style of code.
        //This is not in the style of code this file wants.
        //private void getAllUserAlbumNames_backend(getAllUserAlbumNames_callback guiCallback)
        //{
        //    ErrorReport error = new ErrorReport();

        //    //if the database is NOT valid.
        //    if (!util_checkAlbumDatabase(error))
        //    {
        //        error.reportID = ErrorReport.FAILURE;
        //        error.description = "The album database was determined to be not valid.";
        //        guiCallback(error, null);
        //        return;
        //    }

        //    //the list of all albums to return to the gui.
        //    List<SimpleAlbumData> _albumsToReturn = new List<SimpleAlbumData>();
            
        //    //get all the albums AND their children.
        //    List<XElement> _albumSearch;
        //    try
        //    {
        //        _albumSearch = xmlParser.searchForElements(_albumsDatabase.Element(Properties.Settings.Default.XMLRootElement), "album");
        //    }
        //    catch
        //    {
        //        error.reportID = ErrorReport.FAILURE;//maybe every error should be SHIT_JUST_GOT_REAL?  Decisions, decisions...
        //        error.description = "PhotoBomb.getAllUserAlbumNames():Failed at finding albums in the database.";
        //        guiCallback(error, null);
        //        return;
        //    }
        //    //go through each album and get data from its children to add to the list.
        //    foreach (XElement thisAlbum in _albumSearch)
        //    {
        //        //this custom data class gets sent back to the gui.
        //        SimpleAlbumData userAlbum = new SimpleAlbumData();

        //        List<XElement> _nameSearch;
        //        try
        //        {
        //            //get the name(s) of this album.
        //            _nameSearch = xmlParser.searchForElements(thisAlbum, "albumName");
        //        }
        //        catch
        //        {
        //            error.reportID = ErrorReport.SUCCESS_WITH_WARNINGS;
        //            error.warnings.Add("PhotoBomb.getAllUserAlbumNames():Had an error finding the name of an album.");
        //            continue;
        //        }
        //        //make sure we have at least one name for the album.
        //        if (_nameSearch.Count == 1)
        //        {
        //            //get the value of the album name and add to list.
        //            try
        //            {
        //                userAlbum.albumName = _nameSearch.ElementAt(0).Value;
        //                userAlbum.UID = (int)thisAlbum.Attribute("uid");
        //            }
        //            catch
        //            {
        //                error.reportID = ErrorReport.SUCCESS_WITH_WARNINGS;
        //                error.warnings.Add("PhotoBomb.getAllUserAlbumNames():Had an error trying to get either the name or uid value from an album.");
        //                continue;
        //            }
        //            _albumsToReturn.Add(userAlbum);
        //        }
        //        else if (_nameSearch.Count > 1)
        //        {
        //            error.reportID = ErrorReport.SUCCESS_WITH_WARNINGS;
        //            error.warnings.Add("PhotoBomb.getAllUserAlbumNames():Found an album with more than one name.");
        //        }
        //        else if (_nameSearch.Count == 0)
        //        {
        //            error.reportID = ErrorReport.SUCCESS_WITH_WARNINGS;
        //            error.warnings.Add("PhotoBomb.getAllUserAlbumNames():Found an album with no name.");
        //        }

        //    }//foreach

        //    guiCallback(error, _albumsToReturn);
        //}


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
                specificAlbum = (from c in _albumsDatabase.Elements()
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

            //get the picture from the picture database.
            XElement picElement = util_getComplexPictureByUID(error, uid);

            //if the picture finding function reported success.
            if (error.reportID == ErrorReport.SUCCESS || error.reportID == ErrorReport.SUCCESS_WITH_WARNINGS)
            {
                ComplexPhotoData photo = new ComplexPhotoData();
                //ComplexPhotoData MOE MARKER MOE MARKER MOE MARKER MOE MARKER!!!!!!
                try
                {
                    photo.hash = StringToByteArray((string)picElement.Attribute("sha1"));
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
            //picture failed to be found.
            else
            {
                guiCallback(error, null);
                return;
            }
        }//method

        // BS: Is this function at all used given that we don't add photos one at a time?
        //-------------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        private void addNewPicture_backend(generic_callback guiCallback,
            String photoUserPath,
            String photoExtension,
            int albumUID,
            String pictureNameInAlbum)
        {
            ErrorReport errorReport = new ErrorReport();
            ComplexPhotoData newPicture = new ComplexPhotoData();

            newPicture.hash = util_getHashOfFile(photoUserPath);

            //get a unique ID for this photo and update its 
            //data object to reflect this new UID.
            newPicture.UID = util_getNewPicUID(1);
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
                pictureNameInAlbum = Properties.Settings.Default.DefaultImageName + " " + newPicture.UID.ToString();
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

            //add to disk.
            savePicturesXML_backend(null);
            saveAlbumsXML_backend(null);

            guiCallback(errorReport);
        }


        //-------------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //NOTE: this is an overloaded function call FOR BACKEND USE ONLY.
        //      It does not have a gui callback and instead returns the
        //      Error report directly, for use in the backend.
        private ErrorReport addNewPicture_backend(ErrorReport errorReport,
            String photoUserPath, 
            String photoExtension, 
            int albumUID, 
            String pictureNameInAlbum, 
            int searchStartingPoint)
        {
            ComplexPhotoData newPicture = new ComplexPhotoData();

            // Compute the hash for this picture, and then check to make sure it is unique
            newPicture.hash = util_getHashOfFile(photoUserPath);
            if (!util_checkPhotoIsUniqueToAlbum(albumUID, ByteArrayToString(newPicture.hash)))
            {
                errorReport.reportID = ErrorReport.SUCCESS_WITH_WARNINGS;
                errorReport.description = "Picture is not unique.";
                errorReport.warnings.Add("Picture is not unique: " + photoUserPath);
                return errorReport;
            }

            //get a unique ID for this photo and update its 
            //data object to reflect this new UID.
            newPicture.UID = util_getNewPicUID(searchStartingPoint);
            //error checking
            if (newPicture.UID == -1)
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "Failed to get a UID for a new picture.";
                return errorReport;
            }

            //Change me if you want to start naming the pictures differently in the library.
            String picNameInLibrary = newPicture.UID.ToString() + photoExtension;

            //Change me if you want the default album name to be different.
            if (pictureNameInAlbum == "")
            {
                pictureNameInAlbum = Properties.Settings.Default.DefaultImageName + " " + newPicture.UID.ToString();
            }

            //Move picture and get a new path for the picture in our storage.
            newPicture.path = util_copyPicToLibrary(errorReport, photoUserPath, picNameInLibrary);
            //error checking
            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                return errorReport;
            }

            newPicture.extension = photoExtension;

            util_addPicToPicDatabase(errorReport, newPicture);

            //if adding to the picture database failed
            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                return errorReport;
            }

            util_addPicToAlbumDatabase(errorReport, newPicture, albumUID, pictureNameInAlbum);

            //if adding to the album database failed
            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                return errorReport;
            }

            //save to disk.
            savePicturesXML_backend(null);
            saveAlbumsXML_backend(null);

            return errorReport;
        }




        //-------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        private void addNewAlbum_backend(generic_callback guiCallback, SimpleAlbumData albumData)
        {
            ErrorReport errorReport = new ErrorReport();

            //get a new uid for the new album.
            int uid = util_getNewAlbumUID(errorReport);
            albumData.UID = uid;

            //add the album to the memory database.
            util_addAlbumToAlbumDatabase(errorReport, albumData);

            //if adding to the album database failed
            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                guiCallback(errorReport);
                return;
            }

            //save to disk.
            saveAlbumsXML_backend(null);

            guiCallback(errorReport);

        }


        //-------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //UNTESTED/UNFINISHED
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
        //By: Ryan Moe
        //Edited Last:
        private void checkIfAlbumNameIsUnique_backend(generic_callback guiCallback, String albumName)
        {
            ErrorReport errorReport = new ErrorReport();

            //get uniqueness
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
        //By: Ryan Moe
        //Edited Last:
        private void changePhotoNameByUID_backend(generic_callback guiCallback, int albumUID, int photoUID, String newName)
        {
            ErrorReport errorReport = new ErrorReport();

            //get the album that has the name of the photo we are changing.
            XElement album = util_getAlbumByUID(errorReport, albumUID);

            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                guiCallback(errorReport);
                return;
            }

            //Get the photo from the album.
            XElement photo = util_getPhotoFromAlbumElemByUID(errorReport, album, photoUID);

            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                guiCallback(errorReport);
                return;
            }

            //change the photo's name.
            util_changePhotoNameInAlbumPhotoElem(errorReport, photo, newName);

            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                guiCallback(errorReport);
                return;
            }

            saveAlbumsXML_backend(null);
        }

        //---------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        private void addNewPictures_backend(generic_callback guiCallback, List<String> photoUserPath, List<String> photoExtension, int albumUID, List<String> pictureNameInAlbum, ProgressChangedEventHandler updateCallback, int updateAmount)
        {
            addPhotosThread = new BackgroundWorker();

            //transfer parameters into a data class to pass
            //into the photo thread.
            addPhotosThreadData data = new addPhotosThreadData();
            data.errorReport = new ErrorReport();
            data.guiCallback = guiCallback;
            data.photoUserPath = photoUserPath;
            data.photoExtension = photoExtension;
            data.albumUID = albumUID;
            data.pictureNameInAlbum = pictureNameInAlbum;
            data.updateAmount = updateAmount;

            //setup the worker.
            addPhotosThread.WorkerReportsProgress = true;
            addPhotosThread.WorkerSupportsCancellation = true;
            addPhotosThread.DoWork += new DoWorkEventHandler(addPhotosThread_DoWork);
            addPhotosThread.ProgressChanged += updateCallback;
            addPhotosThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(addPhotosThread_RunWorkerCompleted);
            addPhotosThread.RunWorkerAsync(data);
        }

        //------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        private ErrorReport cancelAddNewPicturesThread_backend()
        {
            ErrorReport error = new ErrorReport();
            try
            {
                addPhotosThread.CancelAsync();
            }
            catch
            {
                error.reportID = ErrorReport.FAILURE;
                error.description = "Failed to stop the thread.";
            }
            return error;
        }
    }//class

}