/**
 * This is the backend for the PhotoBomb program.
 * These functions are NOT to be used by a frontend (gui).
 * 
 * These functions should be written in a "conductor" sort of
 * way, in that they should try and call as many small functions
 * as they can other classes (like the utils class).
 * 
 * *******************************************************************************
 * Changelog:
 * 4/1/13 Ryan Causey: changing the remove/add album backend functions to manipulate
 *                     the observableCollection of albums as this is one-way data bound
 *                     and will automatically update the GUI.
 * 4/4/13 Ryan Causey: changed the existing rebuild database function and added supporting functions to
 *                     take all the existing photos and consolidate them into a single backup album.
 *                     Fixed a bug with rebuildBackend function that caused two recovery albums to appear.
 *                     Fixed a bug where backend would not initialize properly on first program start.
 * 4/5/13 Ryan Causey: Updating addNewPicture and addNewPictures functions to work with new GUI.
 *                     Handled an error case where an unhandled exception would be thrown in saveAlbums/PicturesXML
 * 4/6/13 Ryan Causey: Edited the removePhoto backend function to update the observable collection.
 *                     Fixed using the wrong UID to find the element in the photos collection.
 **/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        /******************************************************************************
         * added for data binding
        ******************************************************************************/
        //observable collection for albums
        private ObservableCollection<SimpleAlbumData> _albumsCollection;

        //observable collection for pictures
        private ObservableCollection<ComplexPhotoData> _photosCollection;

        private List<ComplexPhotoData> _photosClipboard;

        //-----------------------------------------------------------------
        //FUNCTIONS--------------------------------------------------------
        //-----------------------------------------------------------------

        //By: Ryan Moe
        //Edited Last: 3/31/13
        //Edited By: Ryan Causey
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

            //the list of all albums to return to the gui.
            _albumsCollection = new ObservableCollection<SimpleAlbumData>();

            //the list of photographs to be used to populate the album view
            _photosCollection = new ObservableCollection<ComplexPhotoData>();

            // initialize a list for the clipboard
            _photosClipboard = new List<ComplexPhotoData>();

            guiCallback(errorReport);
        }

        //-----------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last By: Ryan Causey
        //Edited Last Date: 4/4/13
        /// <summary>
        /// This function blows out the existing database(normally due to it being corrupted)
        /// and creates a new one, consolidating all the photos into a Recovery Album.
        /// </summary>
        /// <param name="guiCallback">GUI supplied callback</param>
        private void rebuildBackendOnFilesystem_backend(generic_callback guiCallback)
        {
            ErrorReport errorReport = new ErrorReport();
            bool recover = true;

            //if the library folder does not exist, we are not recovering because there is nothing to recover. Sorry =(
            if (!Directory.Exists(libraryPath))
            {
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
                recover = false;
            }

            //make the new database xml files
            XDocument initDB = new XDocument();
            XElement root = new XElement(Settings.XMLRootElement);
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

            createDefaultXML(errorReport);

            // Check the XML creation for bugs
            if (errorReport.reportID == ErrorReport.FAILURE)
            {
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

            // Not needed, createDefaultXML() saves these.
            //saveAlbumsXML_backend(null);
            //savePicturesXML_backend(null);

            //build the backup album and add all the photos to that album if we can
            if (recover)
            {
                addPhotoBackup(errorReport, buildBackupAlbum(errorReport));
            }

            guiCallback(errorReport);
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/4/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// will clear the existing album collection and create a new Recovery Album
        /// and add it to the previously cleared collection.
        /// </summary>
        /// <param name="errorReport">ErrorReport object to be updated</param>
        /// <returns>ErrorReport</returns>
        private int buildBackupAlbum(ErrorReport errorReport)
        {
            //empty the existing album collection
            _albumsCollection.Clear();

            //create the backup album
            SimpleAlbumData backupAlbum = new SimpleAlbumData();

            //set the name
            backupAlbum.albumName = Settings.PhotoLibraryBackupName;

            //get a new uid for the new album.
            backupAlbum.UID = util_getNextUID(_albumsDatabase, "album", "uid", 1);

            //add the album to the memory database.
            util_addAlbumToAlbumDB(errorReport, backupAlbum);

            //if adding to the album database failed
            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                return -1;
            }

            //save to disk.
            saveAlbumsXML_backend(null);

            return backupAlbum.UID;
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/4/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// Adds all the photos from the library into the *hopefully* already created
        /// Recovery Album.
        /// </summary>
        /// <param name="errorReport">ErrorReport object to be updated</param>
        /// <param name="albumUID">UID of the Recovery Album</param>
        /// <returns>ErrorReport</returns>
        private ErrorReport addPhotoBackup(ErrorReport errorReport, int albumUID)
        {
            if (albumUID == -1)
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "Failed to create recovery album";
                return errorReport;
            }

            //set up a directory info object from the path
            DirectoryInfo libraryDir = new DirectoryInfo(libraryPath);

            //set up an array of files
            foreach (FileInfo fi in libraryDir.GetFiles())
            {
                ComplexPhotoData newPicture = new ComplexPhotoData();

                // Compute the hash for this picture, and then check to make sure it is unique
                newPicture.hash = util_getHashOfFile(fi.FullName);
                if (!util_checkPhotoIsUniqueToAlbum(albumUID, ByteArrayToString(newPicture.hash)))
                {
                    errorReport.reportID = ErrorReport.SUCCESS_WITH_WARNINGS;
                    errorReport.description = "Picture is not unique.";
                    errorReport.warnings.Add("Picture is not unique: " + fi.FullName);
                    return errorReport;
                }

                //probably should check that the file extension is supported...
                newPicture.extension = fi.Extension;

                //get a unique ID for this photo and update its 
                //data object to reflect this new UID.
                newPicture.UID = util_getNextUID(_picturesDatabase, "picture", "uid", 1);
                // error checking the call
                if (!util_checkIDIsValid(newPicture.UID))
                {
                    errorReport.reportID = ErrorReport.FAILURE;
                    errorReport.description = "Failed to get a UID for a new picture.";
                    return errorReport;
                }

                //Change me if you want to start naming the pictures differently in the library.
                String picNameInLibrary = newPicture.UID.ToString() + fi.Extension;

                // Get the refcount (will get zero if the pic is brand new) and increment it.
                newPicture.refCount = util_getPhotoRefCount(ByteArrayToString(newPicture.hash));
                newPicture.refCount++;
                // if this is a new picture, we add it to the db
                if (newPicture.refCount == 1)
                {
                    //rename the file
                    fi.MoveTo(Path.Combine(libraryPath, picNameInLibrary));

                    newPicture.fullPath = fi.FullName;

                    util_addPicToPhotoDB(errorReport, newPicture);
                }
                else
                {
                    // Otherwise, incremented the refcount, change the xml object in memory and it'll be saved shortly.
                    XElement thisPic = util_getPhotoDBNode(errorReport, ByteArrayToString(newPicture.hash));
                    thisPic.Attribute("refCount").Value = newPicture.refCount.ToString();
                }

                //if adding to the picture database failed
                if (errorReport.reportID == ErrorReport.FAILURE)
                {
                    return errorReport;
                }

                util_addPicToAlbumDB(errorReport, newPicture, albumUID);

                //if adding to the album database failed
                if (errorReport.reportID == ErrorReport.FAILURE)
                {
                    return errorReport;
                }

                //save to disk.
                savePicturesXML_backend(null);
                saveAlbumsXML_backend(null);

                //when we have the photos collection implemented need to update it here and
                //if necessary blow it out up above.
            }

            return errorReport;
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

            try
            {
                _albumsDatabase.Document.Save(albumsDatabasePath);

            }
            catch (DirectoryNotFoundException)
            {
                error.reportID = ErrorReport.FAILURE;
                error.description = "Library folder not found.";
                guiCallback(error);
                return;
            }
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
            if (!util_checkPhotoDBIntegrity(error))
            {
                guiCallback(error);
                return;
            }

            try
            {
                _picturesDatabase.Document.Save(picturesDatabasePath);

            }
            catch (DirectoryNotFoundException)
            {
                error.reportID = ErrorReport.FAILURE;
                error.description = "Library folder not found.";
                guiCallback(error);
                return;
            }

            if (guiCallback != null)
                guiCallback(error);
        }

        //-----------------------------------------------------------------
        // By: Bill Sanders, based on Ryan Moe's earlier function
        // last edited: 4/6/13
        // Last Edited By: Ryan Causey
        /// <summary>
        /// Retrieves a list of all albums in the albums.xml file, sent back via the callback.
        /// </summary>
        /// <param name="guiCallback">The callback to send the data back to the GUI</param>
        private void getAllAlbums_backend(getAllAlbumNames_callback guiCallback)
        {
            ErrorReport error = new ErrorReport();

            //clear the collection as we are refreshing it
            _albumsCollection.Clear();

            // Ensure the database is valid before proceeding
            if (!util_checkAlbumDatabase(error))
            {
                error.reportID = ErrorReport.FAILURE;
                error.description = "The album database was determined to be not valid.";
                guiCallback(error, null);
                return;
            }

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

                // Get the thumbnail path...
                userAlbum.thumbnailPath = thisAlbum.Element("thumbnailPath").Value;
                // check to see if the file still exists, it may also be empty string, which is ok?

                if ((userAlbum.thumbnailPath != string.Empty) && (!File.Exists(userAlbum.thumbnailPath)))
                {
                    // check to see if this is going to be the first photo in an otherwise empty library...
                    XElement firstPhoto = (from c in thisAlbum.Descendants("picture") select c).FirstOrDefault();

                    util_setAlbumThumbnail(thisAlbum, util_getComplexPhotoData(error, firstPhoto, userAlbum.UID));
                }

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

                _albumsCollection.Add(userAlbum);
            }
            ReadOnlyObservableCollection<SimpleAlbumData> readOnlyAlbumList = new ReadOnlyObservableCollection<SimpleAlbumData>(_albumsCollection);
            guiCallback(error, readOnlyAlbumList);
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
        //By: Bill Sanders
        //Edited Date: 4/7/13
        // TODO: threading?
        /// <summary>
        /// Copies all of the albums in the specified album to the clipboard.
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="AlbumUID">The unique ID of the album to get the photos from</param>
        private void sendSelectedPhotosToClipboard_backend(sendAllPhotosInAlbum_callback guiCallback, int AlbumUID)
        {
            ErrorReport error = new ErrorReport();

            _photosClipboard.Clear();
            //make sure the album database is valid.
            if (!util_checkAlbumDatabase(error))
            {
                guiCallback(error, null);
                return;
            }

            //Try searching for the album with the uid specified.
            XElement specificAlbum = util_getAlbum(error, AlbumUID);

            foreach (XElement subElement in specificAlbum.Element("albumPhotos").Elements("picture"))
            {
                ComplexPhotoData pic = new ComplexPhotoData();
                try
                {
                    //bills new swanky function here
                    _photosClipboard.Add(util_getComplexPhotoData(error, subElement, AlbumUID));
                }
                catch
                {
                    error.reportID = ErrorReport.SUCCESS_WITH_WARNINGS;
                    error.warnings.Add("PhotoBomb.getAllPhotosInAlbum():A Picture in the album is missing either a name or an id.");
                }
            }//foreach

            List<ComplexPhotoData> picturesToGui = new List<ComplexPhotoData>(_photosCollection);
            guiCallback(error, picturesToGui);
        }

        //-----------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last: Ryan Causey
        //Edited Date: 4/4/13
        private void getAllPhotosInAlbum_backend(getAllPhotosInAlbum_callback guiCallback, int AlbumUID)
        {
            ErrorReport error = new ErrorReport();

            //first thing to do is clear the old data out of the photoCollection
            _photosCollection.Clear();

            //make sure the album database is valid.
            if (!util_checkAlbumDatabase(error))
            {
                guiCallback(error, null);
                return;
            }

            //Try searching for the album with the uid specified.
            XElement specificAlbum = util_getAlbum(error, AlbumUID);
            // Commenting out the below in favor of a util class. - BillSanders
            //try
            //{
            //    //for(from) every c in the database's children (all albums),
            //    //see if it's attribute uid is the one we want,
            //    //and if so return the first instance of a match.
            //    specificAlbum = (from c in _albumsDatabase.Elements()
            //                     where (int)c.Attribute("uid") == AlbumUID
            //                     select c).Single();//NOTE: this will throw error if more than one OR none at all.
            //}
            //catch
            //{
            //    error.reportID = ErrorReport.FAILURE;
            //    error.description = "PhotoBomb.getAllPhotosInAlbum():Failed to find the album specified.";
            //    guiCallback(error, null);
            //    return;
            //}

            //Now lets get all the picture data from
            //the album and fill out the picture object list.
            foreach (XElement subElement in specificAlbum.Element("albumPhotos").Elements("picture"))
            {
                ComplexPhotoData pic = new ComplexPhotoData();
                try
                {
                    //bills new swanky function here
                    _photosCollection.Add(util_getComplexPhotoData(error, subElement, AlbumUID));
                }
                catch
                {
                    error.reportID = ErrorReport.SUCCESS_WITH_WARNINGS;
                    error.warnings.Add("PhotoBomb.getAllPhotosInAlbum():A Picture in the album is missing either a name or an id.");
                }
            }//foreach

            ReadOnlyObservableCollection<ComplexPhotoData> picturesToGui = new ReadOnlyObservableCollection<ComplexPhotoData>(_photosCollection);
            guiCallback(error, picturesToGui);
        }//method

        //----------------------------------------------------------------------

        //By: Ryan Moe
        //Edited Last:
        private void getPhoto_backend(getPhotoByUID_callback guiCallback, int idInAlbum, int albumUID)
        {
            ErrorReport error = new ErrorReport();

            // To get a photo from the photoDB knowing only an albumID and its ID in that album
            // We have to first retrieve the album...
            XElement albumNode = util_getAlbum(error, albumUID);
            // ... then with that, we can get the picture element from that album...
            XElement albumPicElement = util_getAlbumDBPhotoNode(error, albumNode, idInAlbum);
            // ... which we use to get the hash of the photo to do a lookup in the PhotoDB!
            XElement picElement = util_getPhotoDBNode(error, (string)albumPicElement.Attribute("sha1").Value);

            //if the picture finding function reported success.
            if (error.reportID == ErrorReport.SUCCESS || error.reportID == ErrorReport.SUCCESS_WITH_WARNINGS)
            {
                ComplexPhotoData photo = new ComplexPhotoData();

                try
                {
                    photo.hash = StringToByteArray((string)picElement.Attribute("sha1"));
                    photo.UID = (int)picElement.Attribute("uid");
                    photo.refCount = (int)picElement.Attribute("refCount");
                    photo.fullPath = (string)picElement.Element("filePath").Value;
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
        // Added by Bill 4/1/13: This function is used only by frontend to add a single photo at a time
        // Commenting it out
        // This function doesn't take that param but starts hardcoded at 1
        // So I added a default parameter of 1 to the below function.
        //-------------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //private void addNewPicture_backend(generic_callback guiCallback,
        //    String photoUserPath,
        //    String photoExtension,
        //    int albumUID,
        //    String pictureNameInAlbum)
        //{
        //    ErrorReport errorReport = new ErrorReport();
        //    ComplexPhotoData newPicture = new ComplexPhotoData();

        //    newPicture.hash = util_getHashOfFile(photoUserPath);

        //    // get an ID number for the picture.
        //    // Note that a photo may have two different ID numbers in albumsdb and picturesdb
        //    newPicture.UID = util_getNextUID(_picturesDatabase, "picture", 1);
        //    // error checking the call
        //    if (!util_checkUIDIsValid(newPicture.UID))
        //    {
        //        errorReport.reportID = ErrorReport.FAILURE;
        //        errorReport.description = "Failed to get a UID for a new picture.";
        //        guiCallback(errorReport);
        //        return;
        //    }

        //    //Change me if you want to start naming the pictures differently in the library.
        //    String picNameInLibrary = newPicture.UID.ToString() + photoExtension;

        //    //Change me if you want the default image name to be different.
        //    if (pictureNameInAlbum == "")
        //    {
        //        pictureNameInAlbum = Properties.Settings.Default.DefaultImageName + " " + newPicture.UID.ToString();
        //    }

        //    //Move picture and get a new path for the picture in our storage.
        //    newPicture.path = util_copyPicToLibrary(errorReport, photoUserPath, picNameInLibrary);
        //    //error checking
        //    if (errorReport.reportID == ErrorReport.FAILURE)
        //    {
        //        guiCallback(errorReport);
        //        return;
        //    }

        //    newPicture.extension = photoExtension;

        //    util_addPicToPhotoDB(errorReport, newPicture);

        //    //if adding to the picture database failed
        //    if (errorReport.reportID == ErrorReport.FAILURE)
        //    {
        //        guiCallback(errorReport);
        //        return;
        //    }

        //    util_addPicToAlbumDB(errorReport, newPicture, albumUID, pictureNameInAlbum);

        //    //if adding to the album database failed
        //    if (errorReport.reportID == ErrorReport.FAILURE)
        //    {
        //        guiCallback(errorReport);
        //        return;
        //    }

        //    //add to disk.
        //    savePicturesXML_backend(null);
        //    saveAlbumsXML_backend(null);

        //    guiCallback(errorReport);
        //}


        //-------------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last By: Ryan Causey
        //Edited Last Date: 4/5/13
        //NOTE: this is an overloaded function call FOR BACKEND USE ONLY.
        //      It does not have a gui callback and instead returns the
        //      Error report directly, for use in the backend.
        /// <summary>
        /// Create a photo object and its metadata, then adds it to both databases and the library.
        /// </summary>
        /// <param name="errorReport">An error report</param>
        /// <param name="photoUserPath">The path where the photo is originally from</param>
        /// <param name="photoExtension">The extension of the photo's filename</param>
        /// <param name="albumUID">The ID of the album to add this picture to</param>
        /// <param name="pictureNameInAlbum">The name the picture will have in the album</param>
        /// <param name="searchStartingPoint">Where to start looking for a new UID; defaults to 1</param>
        /// <returns></returns>
        private ErrorReport addNewPicture_backend(ErrorReport errorReport,
            String photoUserPath,
            String photoExtension,
            int albumUID,
            int searchStartingPoint = 1)
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

            // Get the refcount (will get zero if the pic is brand new) and increment it.
            newPicture.refCount = util_getPhotoRefCount(ByteArrayToString(newPicture.hash));
            newPicture.refCount++;


            newPicture.extension = photoExtension;

            // if this is a new picture, we add it to the db
            if (newPicture.refCount == 1)
            {
                //get a unique ID for this photo and update its 
                //data object to reflect this new UID.
                newPicture.UID = util_getNextUID(_picturesDatabase, "picture", "uid", searchStartingPoint);
                // error checking the call
                if (!util_checkIDIsValid(newPicture.UID))
                {
                    errorReport.reportID = ErrorReport.FAILURE;
                    errorReport.description = "Failed to get a UID for a new picture.";
                    return errorReport;
                }

                //Change me if you want to start naming the pictures differently in the library.
                String picNameInLibrary = newPicture.UID.ToString() + photoExtension;

                newPicture.fullPath = util_copyPhotoToLibrary(errorReport, photoUserPath, picNameInLibrary);
                //error checking
                if (errorReport.reportID == ErrorReport.FAILURE)
                {
                    return errorReport;
                }
                util_addPicToPhotoDB(errorReport, newPicture);
                //Move picture and get a new path for the picture in our storage.
                //generate the thumbnails and get their path.
                newPicture.lgThumbPath = util_generateThumbnail(errorReport, newPicture.fullPath, picNameInLibrary, Settings.lrgThumbSize);
            }
            else
            {
                // Otherwise, incremented the refcount, change the xml object in memory and it'll be saved shortly.
                XElement thisPic = util_getPhotoDBNode(errorReport, ByteArrayToString(newPicture.hash));
                thisPic.Attribute("refCount").Value = newPicture.refCount.ToString();
                // fetch the uid and put it in the complex photo data, we'll need it later
                newPicture.UID = (int)thisPic.Attribute("uid");
            }

            //if adding to the picture database failed
            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                return errorReport;
            }

            util_addPicToAlbumDB(errorReport, newPicture, albumUID);

            //if adding to the album database failed
            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                return errorReport;
            }

            //save to disk.
            savePicturesXML_backend(null);
            saveAlbumsXML_backend(null);

            //update the photosCollection
            //_photosCollection.Add(newPicture);

            return errorReport;
        }

        //-------------------------------------------------------------------
        //By: Bill Sanders
        //Edited Last By: Ryan Causey
        //Edited Last Date: 4/6/13
        /// <summary>
        /// Removes the specified photo from the specified album
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="idInAlbum">The photo's ID</param>
        /// <param name="albumUID">The album's UID</param>
        private ErrorReport removePictureFromAlbum_backend(generic_callback guiCallback, int idInAlbum, int albumUID)
        {
            ErrorReport errorReport = new ErrorReport();

            XElement thisAlbum = util_getAlbum(errorReport, albumUID);

            // First get the instance of the photo (from the album DB!)
            XElement thisPicture = util_getAlbumDBPhotoNode(errorReport, thisAlbum, idInAlbum);

            // check to see if we're removing the first photo in the album.
            XElement firstPhotoInAlbum = (from c in thisAlbum.Descendants("picture") select c).FirstOrDefault();

            // if we are deleting the first...
            if ((int)firstPhotoInAlbum.Attribute("idInAlbum") == idInAlbum)
            {
                // if this is the first *and* last photo, set the thumbnailpath to empty string
                if (thisAlbum.Descendants("picture").Count() == 1)
                {
                    thisAlbum.Element("thumbnailPath").Value = "";
                }
                else
                {
                    // Otherwise, we'll have to actually add the new thumbnail.

                    XElement secondPhotoInAlbum = null;
                    try
                    {
                        // Get the second element (soon to be first) and set it to be the thumbnail
                        secondPhotoInAlbum = thisAlbum.Descendants("picture").ElementAt(1); // (0-indexed)
                        // Set the thumbnail.
                        util_setAlbumThumbnail(thisAlbum, util_getComplexPhotoData(errorReport, secondPhotoInAlbum, albumUID));
                    }
                    catch (Exception ex)
                    {
                        if (ex is ArgumentNullException ||
                            ex is ArgumentOutOfRangeException)
                        {
                            // Don't stop removing this photo though...
                            errorReport.reportID = ErrorReport.FAILURE;
                            errorReport.warnings.Add("Failed to get second photo in the album, though count() returned >1.");
                        }
                        else
                        {
                            // Otherwise I'm not sure what the exception was, so re-throw it.
                            throw;
                        }
                    }
                }
            }

            // Now delete that node
            errorReport = removePictureElement_backend(null, thisPicture);

            //copying bills swanky code
            //get the photo to remove
            var photoToRemove = _photosCollection.FirstOrDefault(photo => photo.idInAlbum == idInAlbum);
            //and remove it
            _photosCollection.Remove(photoToRemove);

            /*
             * commenting out because possibly more efficient code above.
            //Now update the collection(linear searches are the best! <_< NO FUTURE!)
            for (int i = 0; i < _photosCollection.Count; ++i)
            {
                if (_photosCollection[i].UID == idInAlbum)
                {
                    _photosCollection.RemoveAt(i);
                    break;
                }
            }*/

            return errorReport;
        }

        //-------------------------------------------------------------------
        //By: Bill Sanders
        //Edited Last: 3/28/13
        /// <summary>
        /// Removes the specified photo from the specified album
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="pictureElement"></param>
        /// <returns></returns>
        private ErrorReport removePictureElement_backend(generic_callback guiCallback, XElement pictureElement)
        {
            ErrorReport errorReport = new ErrorReport();

            //error prone code here if XElement returned is null. An unhandled exception was raised here while I was testing program -Ryan Causey
            XElement picFromPicsDB = util_getPhotoDBNode(errorReport, (string)pictureElement.Attribute("sha1"));
            // added error handling code, but how is this call even happening on a photo that doesn't exist..?
            // a db integrity issue?

            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                return errorReport;
            }

            // From the photo database, get the number of references remaining to this photo.
            int refCount = (int)picFromPicsDB.Attribute("refCount");

            // Delete this instance of the photo from the in-memory xml database
            try
            {
                // delete this instance of the picture from the album db, and decrement the refCounter
                pictureElement.Remove();
                refCount--;

                if (refCount == 0)
                {
                    // This was the last reference to the picture, delete it from the photoDB and the filesystem
                    removePictureFromPicsDB_backend(null, picFromPicsDB);
                }
                else
                {
                    // The photo is still in the albumDB somewhere, so just update xml with new refcount
                    picFromPicsDB.Attribute("refCount").Value = refCount.ToString();
                }
                // TODO: move these two calls out of here for efficiency in removing multiple files!
                saveAlbumsXML_backend(null);
                savePicturesXML_backend(null);
            }
            catch // the photo mysteriously disappeared (from the xml!) before removing it..?
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "Failed to remove the photo instance from the xml database";
            }
            return errorReport;
        }

        //-------------------------------------------------------------------
        //By: Bill Sanders
        //Edited Last: 4/6/13
        //Edited By: Ryan Causey
        /// <summary>
        /// Removes the specified photo from the PhotoDB as well as the filesystem
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="pictureElement"></param>
        /// <returns></returns>
        private ErrorReport removePictureFromPicsDB_backend(generic_callback guiCallback, XElement pictureElement)
        {
            ErrorReport errorReport = new ErrorReport();

            // TODO: implement deleting thumbnails, too.
            // First try to delete it from the filesystem
            try
            {
                File.Delete(pictureElement.Element("filePath").Value);
                //since we are using this large thumbnail in the program as the image for the picture tile
                //(tested and it is NOT because the album is using the first photo's large thumbnail as its thumb)
                //this throws a System.IO.IOException because it cannot access the file to delete it as it is in use.
                //then the function skips over the rest of the delete, which can lead to dangling items in the xml
                //causing a unhandled exception later on if more deletes are tried.
                File.Delete(pictureElement.Element("lgThumbPath").Value);
            }
            catch
            {
                // the path was probably wrong...
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "Failed to delete the photo file or a thumbnail from the filesystem";
                return errorReport;
            }
            // Now delete this instance of the photo from the in-memory photo database
            // If we've gotten here, we've already deleted it from the albums database
            try
            {
                pictureElement.Remove();
                // TODO: move these calls out of here for efficiency in removing multiple files!
                saveAlbumsXML_backend(null);
                savePicturesXML_backend(null);
            }
            catch // the photo mysteriously disappeared (from the xml!) before removing it..?
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "Failed to remove the photo instance from the xml database";
                return errorReport;
            }
            return errorReport;
        }

        //-------------------------------------------------------------------
        //By: Bill Sanders
        //Edited Last: 4/3/13
        //Edited Last By: Ryan Causey
        /// <summary>
        /// Removes the specified album
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="albumUID">The album's UID</param>
        private ErrorReport removeAlbum_backend(generic_callback guiCallback, int albumUID)
        {
            ErrorReport errorReport = new ErrorReport();

            //error prone code here if there is no album with that UID. An unhandled exception was raised here during testing. -Ryan Causey
            XElement specificAlbum = util_getAlbum(errorReport, albumUID);

            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                return errorReport;
            }

            // linq returns a lazy evaluated ienumberable, which foreach apparently doesn't like, so we convert to a list.
            List<XElement> pictureElements = specificAlbum.Element("albumPhotos").Elements("picture").ToList();

            foreach (XElement subElement in pictureElements)
            {
                // remove the picture element
                removePictureElement_backend(null, subElement);
            }

            // now delete the album itself.
            // Since we've deleted every photo in the album, we can just remove the node.
            specificAlbum.Remove();

            // now sync to the disk
            saveAlbumsXML_backend(null);
            savePicturesXML_backend(null);

            // Searches through the albumsCollection and finds the first album with a matching UID
            var albumToRemove = _albumsCollection.FirstOrDefault(album => album.UID == albumUID);
            // ... and then deletes it.
            _albumsCollection.Remove(albumToRemove);
            // _albumsCollection is an ObservableCollection and should be updated automatically

            // Commenting this out, as the above should be more efficient.
            ////need to update _albumsCollection observable collection by removing the album with this UID
            //for (int i = 0; i < _albumsCollection.Count; ++i)
            //{
            //    //if this is the album we are looking for
            //    if (_albumsCollection[i].UID == albumUID)
            //    {
            //        //remove it from the observableCollection
            //        _albumsCollection.RemoveAt(i);
            //        //end the loop as there are no more albums to remove
            //        break;
            //    }
            //}

            return errorReport;
        }

        //-------------------------------------------------------------------
        //By: Bill Sanders
        //Edited Last: 4/6/13
        /// <summary>
        /// Renames the specified album
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="albumUID">The album's UID</param>
        /// <param name="newName">The new name of the album</param>
        private void renameAlbum_backend(generic_callback guiCallback, int albumUID, string newName)
        {
            ErrorReport errorReport = new ErrorReport();

            // get the album that we are changing.
            XElement albumElem = util_getAlbum(errorReport, albumUID);

            if (errorReport.reportID == ErrorReport.FAILURE)
            {

                guiCallback(errorReport);
                return;
            }

            // change the album's name.
            util_renameAlbum(errorReport, albumElem, newName);

            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                guiCallback(errorReport);
                return;
            }

            saveAlbumsXML_backend(null);

            // Searches through the albumsCollection and finds the first album with a matching UID
            var albumToRename = _albumsCollection.FirstOrDefault(album => album.UID == albumUID);
            // ... and then renames it.
            albumToRename.albumName = newName;

            return;
        }

        //By: Bill Sanders
        //Edited Last: 4/6/13
        /// <summary>
        /// This function renames the specified image in this album.
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="albumUID">The UID of the album the photo is in</param>
        /// <param name="idInAlbum">The id of the photo in this album</param>
        /// <param name="newName">The new name of the photo</param>
        private void renamePhoto_backend(generic_callback guiCallback, int albumUID, int idInAlbum, string newName)
        {
            ErrorReport errorReport = new ErrorReport();

            // get the photo node that we are working on.
            XElement photoElem = util_getAlbumDBPhotoNode(albumUID, idInAlbum);
            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                guiCallback(errorReport);
                return;
            }

            // change the photo's name.
            util_renamePhoto(errorReport, photoElem, newName);
            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                guiCallback(errorReport);
                return;
            }

            saveAlbumsXML_backend(null);

            // Searches through the photosCollection and finds the first photo with a matching id
            var photoToRename = _photosCollection.FirstOrDefault(picture => picture.idInAlbum == idInAlbum);
            // ... and then renames it.
            photoToRename.name = newName;

            return;
        }

        //By: Bill Sanders
        //Edited Last: 4/6/13
        /// <summary>
        /// This function sets the specified caption of the specified image in this album.
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="albumUID">The UID of the album the photo is in</param>
        /// <param name="idInAlbum">The id of the photo in this album</param>
        /// <param name="newCaption">The new name of the photo</param>
        private void setPhotoCaption_backend(generic_callback guiCallback, int albumUID, int idInAlbum, string newCaption)
        {
            ErrorReport errorReport = new ErrorReport();

            // get the photo node that we are working on.
            XElement photoElem = util_getAlbumDBPhotoNode(albumUID, idInAlbum);
            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                guiCallback(errorReport);
                return;
            }

            // ensure the caption is valid, before setting it
            if (!util_checkCaptionIsValid(newCaption))
            {
                guiCallback(errorReport);
                return;
            }

            // change the photo's caption.
            util_setPhotoCaption(errorReport, photoElem, newCaption);
            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                guiCallback(errorReport);
                return;
            }

            saveAlbumsXML_backend(null);

            // Searches through the photosCollection and finds the first photo with a matching id
            var photoToRecaption = _photosCollection.FirstOrDefault(picture => picture.idInAlbum == idInAlbum);
            // ... and then changes its caption.
            photoToRecaption.caption = newCaption;

            return;
        }


        //-------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last By: Ryan Causey
        //Edited Last Date: 4/1/13
        private void addNewAlbum_backend(generic_callback guiCallback, SimpleAlbumData albumData)
        {
            ErrorReport errorReport = new ErrorReport();

            //get a new uid for the new album.
            albumData.UID = util_getNextUID(_albumsDatabase, "album", "uid", 1);

            //add the album to the memory database.
            util_addAlbumToAlbumDB(errorReport, albumData);

            //if adding to the album database failed
            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                guiCallback(errorReport);
                return;
            }

            //save to disk.
            saveAlbumsXML_backend(null);

            //need to update the _albumsCollection observableCollection to reflect this addition in the GUI
            _albumsCollection.Add(albumData); //adds to end of collection

            guiCallback(errorReport);

        }

        //-------------------------------------------------------------
        //By: Bill Sanders
        //Edited Last By: 
        //Edited Last Date: 4/7/13
        /// <summary>
        /// Adds a set of photos that already exist in one album to another album
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="photoObj">A List of ComplexPhotoData objects which contain all the information about a photo</param>
        /// <param name="albumUID">The unique ID of the album to copy the photo into</param>
        private void addExistingPhotosToAlbum_backend(addNewPictures_callback guiCallback, List<ComplexPhotoData> photoList, int albumUID)
        {
            ErrorReport errorReport = new ErrorReport();

            foreach (ComplexPhotoData photoObj in photoList)
            {
                // Get the refcount (will get zero if the pic is brand new) and increment it.
                photoObj.refCount = util_getPhotoRefCount(ByteArrayToString(photoObj.hash));
                photoObj.refCount++;

                // Otherwise, incremented the refcount, change the xml object in memory and it'll be saved shortly.
                XElement thisPic = util_getPhotoDBNode(errorReport, ByteArrayToString(photoObj.hash));
                thisPic.Attribute("refCount").Value = photoObj.refCount.ToString();

                // Currently spec says not to carry captions forawrd when copying photos
                photoObj.caption = "";

                util_addPicToAlbumDB(errorReport, photoObj, albumUID);

                //if adding to the album database failed
                if (errorReport.reportID == ErrorReport.FAILURE)
                {
                    guiCallback(errorReport, albumUID);
                    //save to disk.
                    savePicturesXML_backend(null);
                    saveAlbumsXML_backend(null);
                    break;
                }
            }

            //save to disk.
            savePicturesXML_backend(null);
            saveAlbumsXML_backend(null);

            guiCallback(errorReport, albumUID);
            // You're on your own to update the GUI with the new pictures!
        }

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
        //By: Bill Sanders
        //Edited Last:
        private void checkIfPhotoNameIsUnique_backend(generic_callback guiCallback, String photoName, int albumUID)
        {
            ErrorReport errorReport = new ErrorReport();

            //get uniqueness
            Boolean nameUnique = util_checkPhotoNameIsUniqueToAlbum(photoName, util_getAlbum(errorReport, albumUID));

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
        private void changePhotoNameByUID_backend(generic_callback guiCallback, int albumUID, int idInAlbum, String newName)
        {
            ErrorReport errorReport = new ErrorReport();

            //get the album that has the name of the photo we are changing.
            XElement album = util_getAlbum(errorReport, albumUID);

            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                guiCallback(errorReport);
                return;
            }

            //Get the photo from the album.
            XElement photoElem = util_getAlbumDBPhotoNode(errorReport, album, idInAlbum);

            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                guiCallback(errorReport);
                return;
            }

            //change the photo's name.
            util_renamePhoto(errorReport, photoElem, newName);

            if (errorReport.reportID == ErrorReport.FAILURE)
            {
                guiCallback(errorReport);
                return;
            }

            saveAlbumsXML_backend(null);
        }

        //---------------------------------------------
        //By: Ryan Moe
        //Edited Last By: Ryan Causey
        //Edited Last Date: 4/5/13
        private void addNewPictures_backend(addNewPictures_callback guiCallback, List<String> photoUserPath, List<String> photoExtension, int albumUID, List<String> pictureNameInAlbum, ProgressChangedEventHandler updateCallback, int updateAmount)
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
            //data.photoCollection = _photosCollection;

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
                error.description = errorStrings.stopImportFailure;
            }
            return error;
        }
    }//class

}