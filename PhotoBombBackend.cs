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
 * 4/8/13 Ryan Causey: Removed a call to the validate caption utility function.
 * Julian Nguyen (4/28/13)
 * Change a lot the fun(). A lot of the fun() with _backend was change to public. 
 * Julian Nguyen(4/30/13)
 * ErrorReports constants numbers removed and replaced with ReportStatus enums.
 * Julian Nguyen(5/1/13)
 * setErrorReportToFAILURE() replaced setting an an ErrorReport to FAILURE and it's description.
 * Fun() with "Picture" in the name were changed to "Image"
 * 
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
using System.Drawing;
using System.Drawing.Imaging;

namespace SoftwareEng
{

    //This is a PARTIAL class,
    //it is the private part of the PhotoBomb class.
    public partial class PhotoBomb
    {
        // A handy shortcut to the settings class...
        Properties.Settings Settings = Properties.Settings.Default;

        // Xml parsing utils.
        private PhotoBomb_Xml _photoBomb_xml;

        private ImageManipulation _imageManipulation;
       
        //path to the images folder we put all the images.
        //tracked by the database.
        private string _imagelibraryDirPath;

        //The XML in memory for the albumbs.
        //Add new vars here if we get more xmls.
        private XElement _albumsRootXml;
        private string _albumsXmlPath;

        private XElement _imagesRootXml;
        private string _imageXmlPath;

        // TODO: What is this for? 
        private const int UID_MAX_SIZE = 2000000000;

        /******************************************************************************
         * added for data binding
        ******************************************************************************/
        //observable collection for albums
        private ObservableCollection<SimpleAlbumData> _albumsCollection;

        //observable collection for pictures
        private ObservableCollection<ComplexPhotoData> _imagesCollection;

        private List<ComplexPhotoData> _imagesClipboard;


        /// By Julian Nguyen
        /// Edited: Julian Nguyen(5/1/13)
        /// <summary>
        /// 
        /// </summary>
        public PhotoBomb()
        {
 

            _photoBomb_xml = new PhotoBomb_Xml();
            _imageManipulation = new ImageManipulation();


            //the list of all albums to return to the gui.
            _albumsCollection = new ObservableCollection<SimpleAlbumData>();

            //the list of photographs to be used to populate the album view
            _imagesCollection = new ObservableCollection<ComplexPhotoData>();

            // initialize a list for the clipboard
            _imagesClipboard = new List<ComplexPhotoData>();

            _albumsXmlPath = String.Empty;
            _imageXmlPath = String.Empty; 
            _imagelibraryDirPath = String.Empty;
        }



        /// By: Ryan Moe
        /// Edited: Julian Nguyen(4/28/13)
        /// <summary>
        /// This will setup the data base. Files and all. 
        /// 
        /// </summary>
        /// <param name="albumXmlPathIn"></param>
        /// <param name="imageXmlPathIn"></param>
        /// <param name="imagelibraryDirPathIn"></param>
        public ErrorReport init_backend(string albumXmlPathIn, string imageXmlPathIn, string imagelibraryDirPathIn)
        {
            ErrorReport errorReport = new ErrorReport();

            //keep the paths to databases and library.
            _albumsXmlPath = albumXmlPathIn;
            _imageXmlPath = imageXmlPathIn;
            _imagelibraryDirPath = imagelibraryDirPathIn;


            // Try to open the databases. 
            if (!_photoBomb_xml.loadXmlRootElement(_albumsXmlPath, out _albumsRootXml)
                || !_photoBomb_xml.loadXmlRootElement(_imageXmlPath, out _imagesRootXml))
            {
                setErrorReportToFAILURE("Failed to load the Album or Image xml.", ref errorReport);
            }
            //util_openAlbumsXML(errorReport);
            //util_openImagesXML(errorReport);

            //check the library directory.
            util_checkLibraryDirectory(errorReport);

            return errorReport;
        }

        /// By: Ryan Moe
        /// Edited: Julian Nguyen(4/28/13)
        /// <summary>
        /// This function blows out the existing database(normally due to it being corrupted)
        /// and creates a new one, consolidating all the photos into a Recovery Album.
        /// </summary>
        /// <returns>The error report of this action.</returns>
        public ErrorReport rebuildBackendOnFilesystem_backend()
        {
            ErrorReport errorReport = new ErrorReport();
            bool recover = true;

            //if the library folder does not exist, we are not recovering because there is nothing to recover. Sorry =(
            if (!Directory.Exists(_imagelibraryDirPath))
            {
                //now make a new library folder
                try
                {
                    Directory.CreateDirectory(_imagelibraryDirPath);
                }
                catch
                {
                    setErrorReportToFAILURE("Unable to create the new library folder.", ref errorReport);

                    return errorReport;
                }
                recover = false;
            }

            createDefaultXML(errorReport);

            // Check the XML creation for bugs
            if (errorReport.reportStatus == ReportStatus.FAILURE)
            {
                return errorReport;
            }

            //Load the new databases into memory.
            // BS: These functions are being slated for merging together
            _photoBomb_xml.loadXmlRootElement(_albumsXmlPath, out _albumsRootXml);
            _photoBomb_xml.loadXmlRootElement(_albumsXmlPath, out _imagesRootXml);
            //util_openAlbumsXML(errorReport);
            //util_openImagesXML(errorReport);

            if (errorReport.reportStatus == ReportStatus.FAILURE)
            {
                return errorReport;
            }

            // Not needed, createDefaultXML() saves these.
            //saveAlbumsXML_backend();
            //saveImagesXML_backend();

            //build the backup album and add all the photos to that album if we can
            if (recover)
            {
                addPhotoBackup(errorReport, buildBackupAlbum(errorReport));
            }
            else
            {
                SimpleAlbumData firstAlbum = new SimpleAlbumData();
                firstAlbum.albumName = "My First Album";
                addNewAlbum_backend(firstAlbum);
            }
            return errorReport;
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
        private Guid buildBackupAlbum(ErrorReport errorReport)
        {
            //empty the existing album collection
            _albumsCollection.Clear();

            //create the backup album
            SimpleAlbumData backupAlbum = new SimpleAlbumData();

            //set the name
            backupAlbum.albumName = Settings.PhotoLibraryBackupName;

            //get a new uid for the new album.
            backupAlbum.UID = Guid.NewGuid();
       
            try
            {
                _photoBomb_xml.addAlbumNodeToAlbumsXml(backupAlbum.UID, backupAlbum.thumbAlbumID, backupAlbum.thumbnailPath, backupAlbum.albumName, _albumsRootXml);

            }
            catch (NullReferenceException)
            {
                //if adding to the album database failed
                return Guid.Empty;
            }

            //save to disk.
            errorReport = saveAlbumsXML_backend();
            if (errorReport.reportStatus == ReportStatus.FAILURE)
                return Guid.Empty;

            return backupAlbum.UID;
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/4/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// By: Ryan Causey
        /// Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// Adds all the photos from the library into the *hopefully* already created
        /// Recovery Album.
        /// </summary>
        /// <param name="errorReport">ErrorReport object to be updated</param>
        /// <param name="albumUID">UID of the Recovery Album</param>
        /// <returns>The ErrorReport of this action. </returns>
        private ErrorReport addPhotoBackup(ErrorReport errorReport, Guid albumUID)
        {
            if (albumUID == Guid.Empty)
            {
                setErrorReportToFAILURE("Failed to create recovery album", ref errorReport);
                return errorReport;
            }

            // This method of moving the folder to a new path and copying it back into the library works too
            // I'm not yet able to tell which is faster --BS
            // If you use this method, be sure to uncomment out the directory.delete function at the end of the foreach
            // Note also that DirectoryInfo.*() does not guarantee any order to the files
            //String tmpDir = Path.Combine(libraryPath, "..", "backup");
            //Directory.Move(libraryPath, tmpDir);
            //Directory.CreateDirectory(libraryPath);
            //set up a directory info object from the path
            //FileInfo[] files = new DirectoryInfo(tmpDir).GetFiles();

            IEnumerable<FileInfo> libraryDir = new DirectoryInfo(_imagelibraryDirPath).EnumerateFiles();
            // The files in libraryDir will not be sorted...
            
            //so we'll sort them here by removing the extension and then sorting them numerically
            IEnumerable<FileInfo> sortedFiles = libraryDir.OrderBy(
                fi => int.Parse(Path.GetFileNameWithoutExtension(fi.Name))
                );

            //set up an array of files
            foreach (FileInfo fi in sortedFiles)    
            {
                ComplexPhotoData newPicture = new ComplexPhotoData();

                // Compute the hash for this picture, and then check to make sure it is unique
                newPicture.hash = util_getHashOfFile(fi.FullName);
                if (!util_isImageUniqueToAlbum(albumUID, ByteArrayToString(newPicture.hash)))
                {
                    errorReport.reportStatus = ReportStatus.SUCCESS_WITH_WARNINGS;
                    errorReport.description = "Picture is not unique.";
                    errorReport.warnings.Add("Picture is not unique: " + fi.FullName);
                    return errorReport;
                }

                //probably should check that the file extension is supported...
                newPicture.extension = fi.Extension;

                //get a unique ID for this photo and update its 
                //data object to reflect this new UID.
                newPicture.UID = util_getNextUID(_imagesRootXml, "picture", "uid", 1);
                // error checking the call
                if (!util_checkIDIsValid(newPicture.UID))
                {
                    setErrorReportToFAILURE("Failed to get a UID for a new picture.", ref errorReport);
                    return errorReport;
                }

                //Change me if you want to start naming the pictures differently in the library.
                String picNameInLibrary = newPicture.UID.ToString() + fi.Extension;

                // Get the refcount (will get zero if the pic is brand new) and increment it.
                newPicture.refCount = _photoBomb_xml.getPhotoRefCount(ByteArrayToString(newPicture.hash), _imagesRootXml);
                newPicture.refCount++;
                // if this is a new picture, we add it to the db
                if (newPicture.refCount == 1)
                {
                    //rename the file
                    fi.MoveTo(Path.Combine(_imagelibraryDirPath, picNameInLibrary));

                    newPicture.fullPath = fi.FullName;

                    util_addImageToImageDB(errorReport, newPicture);
                }
                else
                {
                    // Otherwise, incremented the refcount, change the xml object in memory and it'll be saved shortly.
                    XElement imageNode = null;
                    if (!_photoBomb_xml.getImageNodeFromImageXml(ByteArrayToString(newPicture.hash), _imagesRootXml, out imageNode))
                    {
                        setErrorReportToFAILURE("Failed to get the image node.", ref errorReport);
                        return errorReport;
                    }
                    imageNode.Attribute("refCount").Value = newPicture.refCount.ToString();
                }

                //if adding to the picture database failed
                if (errorReport.reportStatus == ReportStatus.FAILURE)
                {
                    return errorReport;
                }

                util_addImageToAlbumDB(errorReport, newPicture, albumUID);

                //if adding to the album database failed
                if (errorReport.reportStatus == ReportStatus.FAILURE)
                {
                    return errorReport;
                }

                //save to disk.
                saveImagesXML_backend();
                saveAlbumsXML_backend();

                //when we have the photos collection implemented need to update it here and
                //if necessary blow it out up above.
            }
            
            //Directory.Delete(tmpDir, true);
            return errorReport;
        }

        /// By Ryan Moe
        /// Edited: Julian Nguyen(4/28/13)
        /// <summary>
        /// This will save the album XML to file.
        /// </summary>
        /// <returns>The error report for this action.</returns>
        public ErrorReport saveAlbumsXML_backend()
        {
            ErrorReport error = new ErrorReport();

            //make sure the album database is valid.
            if (!util_checkAlbumDatabase(error))
            {
                return error;
            }

            try
            {
                _albumsRootXml.Document.Save(_albumsXmlPath);
            }
            catch (DirectoryNotFoundException)
            {
                setErrorReportToFAILURE("Library folder not found.", ref error);
            }
            return error;
        }

        /// By Ryan Moe
        /// Edited: Julian Nguyen(5/1/13)
        /// <summary>
        /// This will save the XML to File. 
        /// </summary>
        /// <returns>The error report of this action.</returns>
        public ErrorReport saveImagesXML_backend()
        {
            ErrorReport error = new ErrorReport();

            //if the database is NOT valid.
            if (!util_checkPhotoDBIntegrity(error))
            {
                return error;
            }

            try
            {
                _imagesRootXml.Document.Save(_imageXmlPath);

            }
            catch (DirectoryNotFoundException)
            {
                setErrorReportToFAILURE("Library folder not found.", ref error);
                return error;
            }
            return error;
        }


        /// By: Bill Sanders, based on Ryan Moe's earlier function.
        /// Edited: Julian Nguyen(5/1/13)
        /// <summary>
        /// Retrieves a list of all albums in the albums.xml file, sent back via the callback.
        /// </summary>
        /// <param name="readOnlyAlbumList">A passback list of SimpleAlbumData.</param>
        /// <returns>The error report of this action.</returns>
        public ErrorReport getAllAlbums_backend(out ReadOnlyObservableCollection<SimpleAlbumData> readOnlyAlbumList)
        {
            ErrorReport error = new ErrorReport();

            //clear the collection as we are refreshing it
            _albumsCollection.Clear();
            //Null for safely.
            readOnlyAlbumList = null;

            // Ensure the database is valid before proceeding
            if (!util_checkAlbumDatabase(error))
            {
                setErrorReportToFAILURE("The album database was determined to be not valid", ref error);
                return error;
            }

            // An object to enumerate over the album XML nodes
            IEnumerable<XElement> _albumSearchIE;
            try
            {
                // get all the albums
                _albumSearchIE = (from c in _albumsRootXml.Elements() select c);
            }
            catch
            {
                setErrorReportToFAILURE("PhotoBomb.getAllAlbums_backend():Failed at finding albums in the database.", ref error);
                return error;
            }
            foreach (XElement thisAlbum in _albumSearchIE)
            {
                SimpleAlbumData userAlbum = new SimpleAlbumData();

                userAlbum.UID = (Guid)thisAlbum.Attribute("uid");

                // Get the thumbnail path...
                userAlbum.thumbnailPath = thisAlbum.Element("thumbnailPath").Value;
                // An album may legally have an empty thumbnail path (if for example it is empty)...

                // Get the idInAlbum of the thumbnail
                try
                {
                    userAlbum.thumbAlbumID = Convert.ToInt32(thisAlbum.Element("thumbnailPath").Attribute("thumbAlbumID").Value);
                }
                catch
                {
                    userAlbum.thumbAlbumID = -1;
                }

                // Here we're going to check if we need to regenerate the album thumbnail
                // First check to make sure the album's thumbnail is NOT set to empty string (an empty album) AND
                // Check to see if the file specified by thumbnailPath does NOT exist
                if ((userAlbum.thumbnailPath != string.Empty) && (!File.Exists(userAlbum.thumbnailPath)))
                {
                    // if the thumbnail doesn't exist... does the image itself?

                    XElement thumbnailPhoto = _photoBomb_xml.getAlbumImageNodeFromAlbumXml(userAlbum.UID, userAlbum.thumbAlbumID, _albumsRootXml);
                    ComplexPhotoData albumImageNodej = _photoBomb_xml.getComplexPhotoDataFromAlbumImageNode(thumbnailPhoto, _imagesRootXml);

                    if (albumImageNodej == null)
                    {
                        setErrorReportToFAILURE("Cannot get complexPhotoData", ref error);
                        return error;
                    }

                    if (!File.Exists(albumImageNodej.fullPath))
                    {
                        // if the image does not exist, remove all references of it from the whole DB
                        util_purgePhotoFromDB(error, ByteArrayToString(albumImageNodej.hash));
                    }

                    // get the first photo in the album and set it as the thumbnail
                    XElement firstAlbumImageNode = (from c in thisAlbum.Descendants("picture") select c).FirstOrDefault();

                    if (firstAlbumImageNode != null)
                    {
                        // ... and set it to be the thumbnail (generating it, if necessary)
                        util_setAlbumThumbnail(thisAlbum, _photoBomb_xml.getComplexPhotoDataFromAlbumImageNode(firstAlbumImageNode, _imagesRootXml));
                    }
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
                    error.reportStatus = ReportStatus.SUCCESS_WITH_WARNINGS;
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
            readOnlyAlbumList = new ReadOnlyObservableCollection<SimpleAlbumData>(_albumsCollection);
            return error;
        }


        /// By: Bill Sanders
        /// Edited Date: Julian Nguyen(4/28/13)
        /// TODO: threading?
        /// <summary>
        /// Copies all of the albums in the specified album to the clipboard. 
        /// </summary>
        /// <param name="AlbumUID">The unique ID of the album to get the photos from</param>
        /// <param name="imagesToGUI">The pass back of the list of iamges.</param>
        /// <returns>The error report of this action.</returns>
        public ErrorReport sendSelectedImagesToClipboard_backend(Guid AlbumUID, out List<ComplexPhotoData> imagesToGUI)
        {
            ErrorReport error = new ErrorReport();
            imagesToGUI = null;

            _imagesClipboard.Clear();
            //make sure the album database is valid.
            if (!util_checkAlbumDatabase(error))
            {
                return error;
            }

            //Try searching for the album with the uid specified.
            XElement albumNode = null;
            if (!_photoBomb_xml.getAlbumNodeFromAlbumXml(AlbumUID, _albumsRootXml, out albumNode))
            {
                // TODO
                return error;
            }

            foreach (XElement albumImageNode in albumNode.Element("albumPhotos").Elements("picture"))
            {
                //ComplexPhotoData pic = new ComplexPhotoData();
                try
                {
                    //bills new swanky function here
                    _imagesClipboard.Add(_photoBomb_xml.getComplexPhotoDataFromAlbumImageNode(albumImageNode, _imagesRootXml));
                }
                catch
                {
                    error.reportStatus = ReportStatus.SUCCESS_WITH_WARNINGS;
                    error.warnings.Add("PhotoBomb.getAllPhotosInAlbum():A Picture in the album is missing either a name or an id.");
                }
            }//foreach

            imagesToGUI = new List<ComplexPhotoData>(_imagesCollection);
            return error;
        }

        /// By: Ryan Moe
        /// Edited: Julian Nguyen(4/28/13)
        /// <summary>
        /// This will get all the Images in an album.  
        /// </summary>
        /// <param name="AlbumUID">The id of the album to look up.</param>
        /// <param name="imagesOfAnAlbum">A passback List of all the images in an album.</param>
        /// <returns>The error roport for this action.</returns>
        public ErrorReport getAllImagesInAlbum_backend(Guid AlbumUID, out ReadOnlyObservableCollection<ComplexPhotoData> imagesOfAnAlbum)
        {
            ErrorReport error = new ErrorReport();

            //first thing to do is clear the old data out of the photoCollection
            _imagesCollection.Clear();

            //make sure the album database is valid.
            if (!util_checkAlbumDatabase(error))
            {
                imagesOfAnAlbum = null;
                return error;
            }

            //Try searching for the album with the uid specified.
            XElement albumNode = null;
            if (!_photoBomb_xml.getAlbumNodeFromAlbumXml(AlbumUID, _albumsRootXml, out albumNode))
            {
                imagesOfAnAlbum = null;
                setErrorReportToFAILURE("Failed to get the album node.", ref error);
                return error;
            }


            //Now lets get all the picture data from
            //the album and fill out the picture object list.
            foreach (XElement albumImageNode in albumNode.Element("albumPhotos").Elements("picture"))
            {
                try
                {
                    //bills new swanky function here
                    ComplexPhotoData imageData = _photoBomb_xml.getComplexPhotoDataFromAlbumImageNode(albumImageNode, _imagesRootXml);

         
                    try
                    {
                        System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                        PropertyItem[] pList = _imageManipulation.getImageProperty(imageData.fullPath);


                        int countToBreak = 2;
                        foreach (PropertyItem pImage in pList)
                        {

                            if (pImage.Id == 0x010F)
                            {
                                imageData.equipmentManufacturer = encoding.GetString(pImage.Value);
                                --countToBreak;
                            }
                            else if (pImage.Id == 0x0110)
                            {
                                imageData.equipmentModel = encoding.GetString(pImage.Value);
                                --countToBreak;
                            }

                            if (countToBreak == 0)
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        // TODO : Do something. 
                    }

                    _imagesCollection.Add(imageData);
                }
                catch
                {
                    error.reportStatus = ReportStatus.SUCCESS_WITH_WARNINGS;
                    error.warnings.Add("PhotoBomb.getAllPhotosInAlbum():A Picture in the album is missing either a name or an id.");
                }
            }//foreach

            imagesOfAnAlbum = new ReadOnlyObservableCollection<ComplexPhotoData>(_imagesCollection);
            return error;

        }//method



        /// By Ryan Moe
        /// Edited: Julian Nguyen(5/1/13)
        /// <summary>
        /// Will get an Image from an Album.
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="imageIDinAlbum">The ID of the image.</param>
        /// <param name="albumUID">The ID of the Album to look in. </param>
        /// <param name="imageData">The pass back of the image's ComplexPhotoData</param>
        /// <returns>The errorReport of this Action.</returns>
        public ErrorReport getImage_backend(int imageIDinAlbum, Guid albumUID, out ComplexPhotoData imageData)
        {
            ErrorReport error = new ErrorReport();
            imageData = null;

            // To get a photo from the photoDB knowing only an albumID and its ID in that album
            // We have to first retrieve the album...
            XElement albumNode = null;
            if (!_photoBomb_xml.getAlbumNodeFromAlbumXml(albumUID, _albumsRootXml, out albumNode))
            {
                setErrorReportToFAILURE("failed to find the albumNode.", ref error);
                return error;
            }

            // ... then with that, we can get the picture element from that album...
            XElement albumImageNode = null;
            if (!_photoBomb_xml.getAlbumImageNodeFromAlbumNode(albumNode, imageIDinAlbum, out albumImageNode))
            {
                setErrorReportToFAILURE("failed to find the albumImageNode.", ref error);
                return error;
            }


            // ... which we use to get the hash of the photo to do a lookup in the PhotoDB!
            XElement imageNode = null;
            if (!_photoBomb_xml.getImageNodeFromImageXml((string)albumImageNode.Attribute("sha1").Value, _imagesRootXml, out imageNode))
            {
                setErrorReportToFAILURE("failed to find the imageNode.", ref error);
                return error;
            }

            try
            {
                // TODO: fun!

                imageData = new ComplexPhotoData();
                imageData.hash = stringToByteArray((string)imageNode.Attribute("sha1"));
                imageData.UID = (int)imageNode.Attribute("uid");
                imageData.refCount = (int)imageNode.Attribute("refCount");
                imageData.fullPath = (string)imageNode.Element("filePath").Value;
            }
            catch
            {
                setErrorReportToFAILURE("PhotoBomb.getPictureByUID():Photo info could not be loaded.", ref error);
            }
            
            return error;
   
        }//method


        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageUserPath"></param>
        /// <param name="imageExtension"></param>
        /// <param name="albumUID"></param>
        /// <returns></returns>
        public ErrorReport addNewImage(String imageUserPath, String imageExtension, Guid albumUID)
        {
            ErrorReport errReport = new ErrorReport();
            addNewImage_backend(errReport, imageUserPath, imageExtension, albumUID);

            saveImagesXML_backend();
            saveAlbumsXML_backend();

            // This is not real!!
            ReadOnlyObservableCollection<ComplexPhotoData> imagesOfAnAlbum = null;
            getAllImagesInAlbum_backend(albumUID, out imagesOfAnAlbum);

            return errReport;
        }


        /// By: Ryan Moe
        /// Edited Julian Nguyen(5/1/13)
        /// NOTE: this is an overloaded function call FOR BACKEND USE ONLY.
        ///      It does not have a gui callback and instead returns the
        ///      Error report directly, for use in the backend.
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
        private ComplexPhotoData addNewImage_backend(ErrorReport errorReport,
            String photoUserPath,
            String photoExtension,
            Guid albumUID,
            int searchStartingPoint = 1)
        {
            ComplexPhotoData imageData = new ComplexPhotoData();

            // Compute the hash for this picture, and then check to make sure it is unique
            imageData.hash = util_getHashOfFile(photoUserPath);
            if (!util_isImageUniqueToAlbum(albumUID, ByteArrayToString(imageData.hash)))
            {
                errorReport.reportStatus = ReportStatus.SUCCESS_WITH_WARNINGS;
                errorReport.description = "Picture is not unique.";
                errorReport.warnings.Add("Picture is not unique: " + photoUserPath);
                return null;
            }

            // Get the refcount (will get zero if the pic is brand new) and increment it.
            imageData.refCount = _photoBomb_xml.getPhotoRefCount(ByteArrayToString(imageData.hash), _imagesRootXml);
            imageData.refCount++;


            imageData.extension = photoExtension;

            // if this is a new picture, we add it to the db
            if (imageData.refCount == 1)
            {
                //get a unique ID for this photo and update its 
                //data object to reflect this new UID.
                imageData.UID = util_getNextUID(_imagesRootXml, "picture", "uid", searchStartingPoint);
                // error checking the call
                if (!util_checkIDIsValid(imageData.UID))
                {
                    setErrorReportToFAILURE("Failed to get a UID for a new picture.", ref errorReport);
                    return null;
                }

                //Change me if you want to start naming the pictures differently in the library.
                String picNameInLibrary = imageData.UID.ToString() + photoExtension;

                imageData.fullPath = util_copyImageToLibrary(errorReport, photoUserPath, picNameInLibrary);
                //error checking
                if (errorReport.reportStatus == ReportStatus.FAILURE)
                {
                    return null;
                }
                util_addImageToImageDB(errorReport, imageData);
                //Move picture and get a new path for the picture in our storage.
                //generate the thumbnails and get their path.
                imageData.lgThumbPath = util_generateThumbnail(errorReport, imageData.fullPath, picNameInLibrary, Settings.lrgThumbSize);
            }
            else
            {
                // Otherwise, incremented the refcount, change the xml object in memory and it'll be saved shortly.
                XElement imageNode =_photoBomb_xml.getImageNodeFromImageXml(ByteArrayToString(imageData.hash), _imagesRootXml);


                // Assign this new refcount
                imageNode.Attribute("refCount").Value = imageData.refCount.ToString();

                // fetch the uid and put it in the complex photo data, we'll need it later too
                imageData.UID = (int)imageNode.Attribute("uid");

                // make sure the photo still exists on the filesystem
                if (!File.Exists(imageNode.Element("filePath").Value))
                {
                    // good thing we checked!  Copy it to the filesystem again.
                    imageData.fullPath = util_copyImageToLibrary(
                        errorReport,
                        imageNode.Element("filePath").Value,
                        imageData.UID.ToString() + photoExtension);
                }
                else
                {
                    imageData.fullPath = imageNode.Element("filePath").Value;
                }
            }

            //if adding to the picture database failed
            if (errorReport.reportStatus == ReportStatus.FAILURE)
            {
                return null;
            }

            // Now add it to the albums database
            //_photoBomb_xml.a

            util_addImageToAlbumDB(errorReport, imageData, albumUID);

            //if adding to the album database failed
            if (errorReport.reportStatus == ReportStatus.FAILURE)
            {
                return null;
            }

            //save to disk.
            saveImagesXML_backend();
            saveAlbumsXML_backend();

            //update the photosCollection
            //_imagesCollection.Add(imageData);

            return imageData;
        }


        /// By: Bill Sanders
        /// Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// Removes the specified photo from the specified album
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="inAlbumID">The photo's ID</param>
        /// <param name="albumUID">The album's UID</param>
        public ErrorReport removeImageFromAlbum_backend(int inAlbumID, Guid albumUID)
        {
            ErrorReport errorReport = new ErrorReport();


            XElement albumNode = null;
            if (!_photoBomb_xml.getAlbumNodeFromAlbumXml(albumUID, _albumsRootXml, out albumNode))
            {
                setErrorReportToFAILURE("Failed to find an album with that UID", ref errorReport);
                return errorReport;
            }

            // First get the instance of the photo (from the album DB!)
            XElement albumImageNode = null;
            if (!_photoBomb_xml.getAlbumImageNodeFromAlbumNode(albumNode, inAlbumID, out albumImageNode))
            {
                setErrorReportToFAILURE("Failed to find the albumImageNode.", ref errorReport);
                return errorReport;
            }

            // check to see if we're removing the first photo in the album.
            XElement firstPhotoInAlbum = (from c in albumNode.Descendants("picture") select c).FirstOrDefault();

            


            // if we are deleting the first...
            if ((int)firstPhotoInAlbum.Attribute("idInAlbum") == inAlbumID)
            {
                // if this is the first *and* last photo, set the thumbnailpath to empty string
                if (albumNode.Descendants("picture").Count() == 1)
                {
                    albumNode.Element("thumbnailPath").Value = String.Empty;
                    albumNode.Element("thumbnailPath").Attribute("thumbAlbumID").Value = "-1";
                }
                else
                {
                    // Otherwise, we'll have to actually add the new thumbnail.

                    XElement secondPhotoInAlbum = null;
                    try
                    {
                        // Get the second element (soon to be first) and set it to be the thumbnail
                        secondPhotoInAlbum = albumNode.Descendants("picture").ElementAt(1); // (0-indexed)
                        // Set the thumbnail.
                        util_setAlbumThumbnail(albumNode, _photoBomb_xml.getComplexPhotoDataFromAlbumImageNode(secondPhotoInAlbum, _imagesRootXml));

                    }
                    catch (Exception ex)
                    {
                        if (ex is ArgumentNullException ||
                            ex is ArgumentOutOfRangeException ||
                        ex is NullReferenceException)
                        {
                            // Don't stop removing this photo though...
                            setErrorReportToFAILURE("Failed to get second photo in the album, though count() returned >1.", ref errorReport);
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
            errorReport = removeImageFromImageXml(albumImageNode, false);

            //copying bills swanky code
            //get the photo to remove
            var photoToRemove = _imagesCollection.FirstOrDefault(photo => photo.idInAlbum == inAlbumID);
            //and remove it
            _imagesCollection.Remove(photoToRemove);

            saveAlbumsXML_backend();
            saveImagesXML_backend();

            return errorReport;
        }

        /// By: Bill Sanders
        /// Edited Last: Julian Nguyen(5/1/13)
        /// <summary>
        /// Removes the photo instance from the album, deleting it if it no longer appears in any album.
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="albumImageNode">An XElement object representing a picture in the AlbumDB</param>
        /// <returns>The Errorreport of this Action.</returns>
        private ErrorReport removeImageFromImageXml(XElement albumImageNode, bool saveXmlFiles)
        {
            ErrorReport errorReport = new ErrorReport();

            // error prone code here if XElement returned is null. An unhandled exception was raised here while I was testing program -Ryan Causey
            // added error handling code, but how is this call even happening on a photo that doesn't exist..?
            // a db integrity issue?
            XElement imageNode = null;
            if (!_photoBomb_xml.getImageNodeFromImageXml((string)albumImageNode.Attribute("sha1"), _imagesRootXml, out imageNode))
            {
                setErrorReportToFAILURE("There is not imageNode with that hash", ref errorReport);
                return errorReport;
            }

            // From the photo database, get the number of references remaining to this photo.
            int refCount = (int)imageNode.Attribute("refCount");

            // Delete this instance of the photo from the in-memory xml database
            try
            {
                // delete this instance of the picture from the album db, and decrement the refCounter
                _photoBomb_xml.removeNode(albumImageNode);
                --refCount;

                if (refCount == 0)
                {
                    // This was the last reference to the picture, delete it from the photoDB and the filesystem
                    removeImageFromImageDB_backend(imageNode);

                }
                else
                {
                    // The photo is still in the albumDB somewhere, so just update xml with new refcount
                    imageNode.Attribute("refCount").Value = refCount.ToString();
                }
                // TODO: move these two calls out of here for efficiency in removing multiple files!

                if (saveXmlFiles)
                {
                    saveAlbumsXML_backend();
                    saveImagesXML_backend();
                }

            }
            catch // the photo mysteriously disappeared (from the xml!) before removing it..?
            {
                setErrorReportToFAILURE("Failed to remove the photo instance from the xml database", ref errorReport);
            }
            return errorReport;
        }

        //-------------------------------------------------------------------
        //By: Bill Sanders
        //Edited Last: 4/6/13
        //Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// Removes the specified Image from the Image database and from the filesystem.
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="imageNode">An XElement object referencing a photo from the PhotoDB</param>
        /// <returns></returns>
        private ErrorReport removeImageFromImageDB_backend(XElement imageNode)
        {
            ErrorReport errorReport = new ErrorReport();

            // TODO: implement deleting thumbnails, too.
            // First try to delete it from the filesystem
            try
            {
                // TODO JN: cannot delete. 

                File.Delete(imageNode.Element("filePath").Value);

                try
                {
                    File.Delete(imageNode.Element("filePath").Value);
                }
                catch { }

                //since we are using this large thumbnail in the program as the image for the picture tile
                //(tested and it is NOT because the album is using the first photo's large thumbnail as its thumb)
                //this throws a System.IO.IOException because it cannot access the file to delete it as it is in use.
                //then the function skips over the rest of the delete, which can lead to dangling items in the xml
                //causing a unhandled exception later on if more deletes are tried.
                try
                {
                    File.Delete(imageNode.Element("lgThumbPath").Value);
                }
                catch { }

            }
            catch
            {
                // the path was probably wrong...
                setErrorReportToFAILURE("Failed to delete the photo file or a thumbnail from the filesystem", ref errorReport);
                //return errorReport;
            }
            // Now delete this instance of the photo from the in-memory photo database
            // If we've gotten here, we've already deleted it from the albums database


            if (!_photoBomb_xml.removeNode(imageNode))
            {
                setErrorReportToFAILURE("Failed to remove the photo instance from the xml database", ref errorReport);
                return errorReport;
            }

   
            // TODO: move these calls out of here for efficiency in removing multiple files!
            saveAlbumsXML_backend();
            saveImagesXML_backend();
            
            return errorReport;
        }

        /// By: Bill Sanders
        /// Edited Last: Julian Nguyen(4/28/13)
        /// <summary>
        /// Removes the specified album
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="albumUID">The album's UID</param>
        public ErrorReport removeAlbum_backend(Guid albumUID)
        {
            ErrorReport errorReport = new ErrorReport();

            //error prone code here if there is no album with that UID. An unhandled exception was raised here during testing. -Ryan Causey
            XElement albumNode = null;
            if (!_photoBomb_xml.getAlbumNodeFromAlbumXml(albumUID, _albumsRootXml, out albumNode))
            {
                setErrorReportToFAILURE("Failed to find a single album by UID.", ref errorReport);
                return errorReport;
            }

            // linq returns a lazy evaluated ienumberable, which foreach apparently doesn't like, so we convert to a list.
            List<XElement> albumImageNodes = albumNode.Element("albumPhotos").Elements("picture").ToList();

            foreach (XElement albumImageNode in albumImageNodes)
            {
                // remove the picture element
                removeImageFromImageXml(albumImageNode, false);
            }

            // now delete the album itself.
            // Since we've deleted every photo in the album, we can just remove the node.
            _photoBomb_xml.removeNode(albumNode);

            // now sync to the disk
            saveAlbumsXML_backend();
            saveImagesXML_backend();

            // Searches through the albumsCollection and finds the first album with a matching UID
            var albumToRemove = _albumsCollection.FirstOrDefault(album => album.UID == albumUID);
            // ... and then deletes it.
            _albumsCollection.Remove(albumToRemove);

            return errorReport;
        }

        
        /// By: Bill Sanders
        /// Edited Last: Julian Nguyen(4/28/13)
        /// <summary>
        /// Renames the specified album.
        /// </summary>
        /// <param name="albumUID">The album's UID</param>
        /// <param name="newAlbumName">The new name of the album</param>
        /// <return>The error report of this action.</return>
        public ErrorReport setAlbumName_backend(Guid albumUID, string newAlbumName)
        {
            ErrorReport errorReport = new ErrorReport();

            // get the album that we are changing.

            XElement albumNode = null;
            if (!_photoBomb_xml.getAlbumNodeFromAlbumXml(albumUID, _albumsRootXml, out albumNode))
            {
                setErrorReportToFAILURE("Failed to get the albumNode.", ref errorReport);
                return errorReport;
            }


            // change the album's name.

            if (!_photoBomb_xml.setAlbumNodeName(albumNode, newAlbumName))
            {
                setErrorReportToFAILURE("Failed to set the album name.", ref errorReport);
                return errorReport;
            }

            saveAlbumsXML_backend();

            // Searches through the albumsCollection and finds the first album with a matching UID
            var albumDataToRename = _albumsCollection.FirstOrDefault(album => album.UID == albumUID);
            // ... and then renames it.
            albumDataToRename.albumName = newAlbumName;

            return errorReport;
        }


        /// By: Bill Sanders
        /// Edited: Julian Nguyen(4/28/13)
        /// <summary>
        /// This function renames the specified image in this album.
        /// </summary>
        /// <param name="albumUID">The UID of the album the photo is in</param>
        /// <param name="idInAlbum">The id of the photo in this album</param>
        /// <param name="newImageName">The new name of the photo</param>
        /// <returns></returns>
        public ErrorReport setImageName_backend(Guid albumUID, int idInAlbum, string newImageName)
        {
            ErrorReport errorReport = new ErrorReport();

            // get the photo node that we are working on.
            XElement albumImageNode = _photoBomb_xml.getAlbumImageNodeFromAlbumXml(albumUID, idInAlbum, _albumsRootXml);
            if (albumImageNode == null)
            {
                setErrorReportToFAILURE("Failed to get AlbumImageNode", ref errorReport);
                return errorReport;
            }

            // change the photo's name.
            if (!_photoBomb_xml.setAlbumImageNodeName(albumImageNode, newImageName))
            {
                setErrorReportToFAILURE("File to set image name.", ref errorReport);
                return errorReport;
            }

            saveAlbumsXML_backend();

            // Searches through the photosCollection and finds the first photo with a matching id
            var photoToRename = _imagesCollection.FirstOrDefault(picture => picture.idInAlbum == idInAlbum);
            // ... and then renames it.
            photoToRename.name = newImageName;

            return errorReport;
        }


        //By: Bill Sanders
        //Edited Last: Julian Nguyen(4/27/13)
        /// <summary>
        /// This function sets the specified caption of the specified image in this album.
        /// </summary>
        /// <param name="albumUID">The UID of the album the photo is in</param>
        /// <param name="idInAlbum">The id of the photo in this album</param>
        /// <param name="newCaption">The new name of the photo</param>
        /// <returns>The error report for this action.</returns>
        public ErrorReport setImageCaption_backend(Guid albumUID, int idInAlbum, string newCaption)
        {
            ErrorReport errorReport = new ErrorReport();

            // get the photo node that we are working on.
            XElement albumImageNode = _photoBomb_xml.getAlbumImageNodeFromAlbumXml(albumUID, idInAlbum, _albumsRootXml);
            if (albumImageNode == null)
            {
                setErrorReportToFAILURE("Failed to get albumImageNode", ref errorReport);
                return errorReport;
            }

            // change the photo's caption.
            util_setPhotoCaption(errorReport, albumImageNode, newCaption);
            if (errorReport.reportStatus == ReportStatus.FAILURE)
            {
                // TODO:
                return errorReport;
            }

            saveAlbumsXML_backend();

            // Searches through the photosCollection and finds the first photo with a matching id
            var photoToRecaption = _imagesCollection.FirstOrDefault(picture => picture.idInAlbum == idInAlbum);
            // ... and then changes its caption.
            photoToRecaption.caption = newCaption;

            return errorReport;
        }


        /// By: Ryan Moe
        /// Edited Julian Nguyen
        /// <summary>
        /// Will add a new Album to the database.
        /// Note: The UID of the albumData will not be used!!
        /// </summary>
        /// <param name="albumData">The data for the new album. The UID will not be used.</param>
        /// <returns>The error Report of this action.</returns>
        public ErrorReport addNewAlbum_backend(SimpleAlbumData albumData)
        {
            ErrorReport errorReport = new ErrorReport();

            //get a new uid for the new album.
            albumData.UID = Guid.NewGuid(); //util_getNextUID(_albumsRootXml, "album", "uid", 1);

            //add the album to the memory database.
            try
            {
                _photoBomb_xml.addAlbumNodeToAlbumsXml(albumData.UID, albumData.thumbAlbumID, albumData.thumbnailPath, albumData.albumName, _albumsRootXml);

            }
            catch (NullReferenceException)
            {
                setErrorReportToFAILURE("Failed to add an album to the albums xml.", ref errorReport);
                return errorReport;
            }
            //need to update the _albumsCollection observableCollection to reflect this addition in the GUI
            _albumsCollection.Add(albumData); //adds to end of collection

            // Save the albums xml to disk.
            return saveAlbumsXML_backend();
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
        /// 
        public ErrorReport addExistingImagesToAlbum_backend(List<ComplexPhotoData> imageList, Guid albumUID)
        {
            ErrorReport errorReport = new ErrorReport();

            foreach (ComplexPhotoData imageData in imageList)
            {
                if (!File.Exists(imageData.fullPath))
                {
                    // File doesn't exist.
                    continue;
                }

                if (!util_isImageUniqueToAlbum(albumUID, ByteArrayToString(imageData.hash)))
                {
                    errorReport.reportStatus = ReportStatus.SUCCESS_WITH_WARNINGS;
                    errorReport.description = "Picture is not unique.";
                    errorReport.warnings.Add("Picture is not unique: " + imageData.fullPath);
                    continue;
                }

                // Get the refcount (will get zero if the pic is brand new) and increment it.
                imageData.refCount =  _photoBomb_xml.getPhotoRefCount(ByteArrayToString(imageData.hash), _imagesRootXml);
                imageData.refCount++;

                // Otherwise, incremented the refcount, change the xml object in memory and it'll be saved shortly'
                XElement imageNode = null;
                if (!_photoBomb_xml.getImageNodeFromImageXml(ByteArrayToString(imageData.hash), _imagesRootXml, out imageNode))
                {
                    setErrorReportToFAILURE("Failed to get the image node.", ref errorReport);
                    return errorReport;
                }

                imageNode.Attribute("refCount").Value = imageData.refCount.ToString();

                // Currently spec says not to carry captions forawrd when copying photos
                imageData.caption = "";

                util_addImageToAlbumDB(errorReport, imageData, albumUID);

                //if adding to the album database failed
                if (errorReport.reportStatus == ReportStatus.FAILURE)
                {
                    //guiCallback(errorReport, albumUID); //TODO: JN: What the hell is going on??
                    //save to disk.
                    //saveImagesXML_backend();
                    //saveAlbumsXML_backend();
                    break;
                }
            }

            //save to disk.
            saveImagesXML_backend();
            saveAlbumsXML_backend();

            // addNewPictures_callback.
            //guiCallback(errorReport, albumUID);
            return errorReport;
            // You're on your own to update the GUI with the new pictures!
        }

        /// By Ryan Moe
        /// Edited: Julian Nguyen(5/1/13)
        /// <summary>
        /// Test an Album name for uniqueness.
        /// The answer is in the ErrorReport. (Why not??) 
        /// </summary>
        /// <param name="albumName">The Album name to test.</param>
        /// <param name="isUnique">If the Album is unique. </param>
        /// <returns>The ErrorReport of this action.</returns>
        public ErrorReport isAlbumNameUnique_backend(String albumName, out bool isUnique)
        {
            ErrorReport errorReport = new ErrorReport();

            //Test for uniqueness.
            isUnique = _photoBomb_xml.isAlbumNameUnique(albumName, _albumsRootXml);
            
            if (!isUnique)
            {
                setErrorReportToFAILURE("Album name is not unique.", ref errorReport);
            }

            return errorReport;
        }

        /// By Bill Sanders
        /// Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// Will test if am Image name is unique.
        /// The answer is in the ErrorReport. (Why not??) 
        /// </summary>
        /// <param name="imageName">The Image name to test.</param>
        /// <param name="albumUID">The UID of the album.</param>
        /// <returns>The error Report of this action. </returns>
        public ErrorReport isImageNameUnique_backend(String imageName, Guid albumUID, out bool isUnique)
        {
            ErrorReport errorReport = new ErrorReport();

            XElement albumNode = null;
            if (!_photoBomb_xml.getAlbumNodeFromAlbumXml(albumUID, _albumsRootXml, out albumNode))
            {
                isUnique = false;
                setErrorReportToFAILURE("Failed to find the album node.", ref errorReport);
                return errorReport;
            }

            // Test uniqueness!
            if (isUnique = _photoBomb_xml.isImageNameUniqueToAlbum(imageName, albumNode))
            {
                return errorReport;
            }

            setErrorReportToFAILURE("Album name is not unique.", ref errorReport);
            return errorReport;
        }


        /// By Ryan Moe
        /// Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// Will set a name to an Image.
        /// </summary>
        /// <param name="albumUID">The album's UID</param>
        /// <param name="idInAlbum">The in Album UID.</param>
        /// <param name="newImageName">The new mame of the Image.</param>
        public ErrorReport setImageNameByUID_backend(Guid albumUID, int idInAlbum, String newImageName)
        {
            ErrorReport errorReport = new ErrorReport();


            //Get the photo from the album.
            XElement albumImageNode = null;
            if (!_photoBomb_xml.getAlbumImageNodeFromAlbumXml(albumUID, idInAlbum, _albumsRootXml, out albumImageNode))
            {
                setErrorReportToFAILURE("Failed to get the album image node.", ref errorReport);
                return errorReport;
            }

            //change the photo's name.
            if (!_photoBomb_xml.setAlbumImageNodeName(albumImageNode, newImageName))
            {
                setErrorReportToFAILURE("failed to set the image name.", ref errorReport);
                return errorReport;
            }

            // TODO: JN: I don't this is real. 
            return saveAlbumsXML_backend();
        }


        /// By Ryan Moe
        /// Edited: Julian Nguyen(4/28/13)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="photoUserPath"></param>
        /// <param name="photoExtension"></param>
        /// <param name="albumUID"></param>
        /// <param name="pictureNameInAlbum"></param>
        /// <param name="updateCallback"></param>
        /// <param name="updateAmount"></param>
        public void addNewImages_backend(addNewPictures_callback guiCallback, List<String> photoUserPath, List<String> photoExtension, Guid albumUID, List<String> pictureNameInAlbum, ProgressChangedEventHandler updateCallback, int updateAmount)
        {
            _addPhotosThread = new BackgroundWorker();

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
            _addPhotosThread.WorkerReportsProgress = true;
            _addPhotosThread.WorkerSupportsCancellation = true;
            _addPhotosThread.DoWork += new DoWorkEventHandler(addPhotosThread_DoWork);
            _addPhotosThread.ProgressChanged += updateCallback;
            _addPhotosThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(addPhotosThread_RunWorkerCompleted);
            _addPhotosThread.RunWorkerAsync(data);
        }


        /// By Ryan Moe
        /// Edited: Julian Nguyen(5/1/13)
        /// <summary>
        /// This will stop the import of Images.
        /// </summary>
        /// <returns>The error report of this action. </returns>
        public ErrorReport cancelAddNewImagesThread_backend()
        {
            ErrorReport error = new ErrorReport();
            try
            {
                _addPhotosThread.CancelAsync();
            }
            catch
            {
                setErrorReportToFAILURE(errorStrings.stopImportFailure, ref error);
            }
            return error;
        }


        /// By Julian Nguyen
        /// Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// A wrapper for setting an ErrorReport To FAILURE.
        /// </summary>
        /// <param name="description">The description of the failure.</param>
        /// <param name="errReport">The ErrorReport.</param>
        private void setErrorReportToFAILURE(String description, ref ErrorReport errReport)
        {
            if (errReport == null)
                return;

            errReport.reportStatus = ReportStatus.FAILURE;
            errReport.description = description;
        }

    } // End of PhotoBomb.

}