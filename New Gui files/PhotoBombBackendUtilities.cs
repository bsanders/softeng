/**
 * By: Ryan Moe
 * 
 * This class is for utility functions for the PhotoBomb backend.
 * By utility I mean for functions that do a single simple task
 * that many other funcitons may want to use.
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Security.Cryptography; //Namespace for SHA1

namespace SoftwareEng
{
    public partial class PhotoBomb
    {
        //----------------------------------------------------------
        //By: Bill Sanders
        //Edited Last: 3/28/13
        /// <summary>
        /// Query the Photo DB for a photo by hash
        /// </summary>
        /// <param name="error"></param>
        /// <param name="hash"></param>
        /// <returns>An XElement object representing the photo in the PhotoDB</returns>
        private XElement util_getPhotoDBNode(ErrorReport error, string hash)
        {
            if (util_checkPhotoDBIntegrity(error))
            {
                //Try searching for the album with the uid specified.
                XElement specificPicture;
                try
                {
                    //for(from) every c in the database's children (all albums),
                    //see if it's attribute uid is the one we want,
                    //and if so return the first instance of a match.
                    specificPicture = (from c in _picturesDatabase.Elements()
                                       where (string)c.Attribute("sha1") == hash
                                       select c).Single();//NOTE: this will throw error if more than one OR none at all.
                }
                //failed to find the picture
                catch
                {
                    error.reportID = ErrorReport.FAILURE;
                    error.description = "Failed to find the picture specified.";
                    return null;
                }
                //success!
                return specificPicture;
            }

            //database is not clean!
            else
            {
                //error object already filled out by integrity checker.
                return null;
            }
        }//method


        //----------------------------------------------------------
        //By: Ryan Moe
        //Edited Last: Bill Sanders, 3/29/13
        /// <summary>
        /// Query the Photo DB for a photo by hash
        /// </summary>
        /// <param name="error"></param>
        /// <param name="photoUID"></param>
        /// <returns>An XElement object representing the photo in the PhotoDB</returns>
        private XElement util_getPhotoDBNode(ErrorReport error, int photoUID)
        {
            if (util_checkPhotoDBIntegrity(error))
            {
                //Try searching for the album with the uid specified.
                XElement specificPicture;
                try
                {
                    //for(from) every c in the database's children (all albums),
                    //see if it's attribute uid is the one we want,
                    //and if so return the first instance of a match.
                    specificPicture = (from c in _picturesDatabase.Elements()
                                       where (int)c.Attribute("uid") == photoUID
                                       select c).Single();//NOTE: this will throw error if more than one OR none at all.
                }
                //failed to find the picture
                catch
                {
                    error.reportID = ErrorReport.FAILURE;
                    error.description = "Failed to find the picture specified.";
                    return null;
                }
                //success!
                return specificPicture;
            }

            //database is not clean!
            else
            {
                //error object already filled out by integrity checker.
                return null;
            }
        }//method

        //--------------------------------------------------------
        // By: Bill Sanders
        // Edited Last: 3/28/13
        /// <summary>
        /// Retrieves a specific photo instance from a combination of the AlbumID and its Photo ID in that album
        /// </summary>
        /// <param name="albumID">The unique ID of the album</param>
        /// <param name="photoID">The unique ID of the image in that album</param>
        /// <returns>Returns an xml node of the photo from the AlbumDB, or null.</returns>
        private XElement util_getAlbumDBPhotoNode(int albumID, int photoID)
        {
            ErrorReport errorReport = new ErrorReport();

            // first get the album by albumID
            XElement albumNode = util_getAlbum(errorReport, albumID);

            // Then get the photo node from that.
            XElement photoInstance = null;
            try
            {
                photoInstance = util_getAlbumDBPhotoNode(errorReport, albumNode, photoID);
            }
            catch
            {
                return null;
            }
            return photoInstance;
        }


        //--------------------------------------------------------
        // By: Bill Sanders
        // Edited Last: 3/28/13
        // 
        // TODO: Examine possible speed up by using a byte-array instead of string comparison in LINQ?
        // TODO2: Add errorreport
        /// <summary>
        /// Retrieves a specific photo instance from a combination of the AlbumID and its hash
        /// </summary>
        /// <param name="albumID">The unique ID of the album</param>
        /// <param name="hash">A hex string representation of the hash of the photo</param>
        /// <returns>Returns an xml node of the photo from the AlbumDB, or null.</returns>
        private XElement util_getAlbumDBPhotoNode(int albumID, string hash)
        {
            XElement photoInstance = null;
            try
            {
                // Try to find a photo hash in this album by hash.
                // Note: this may be working inefficiently.
                // Join every picture element from both databases, where the pictures have the same sha1
                // Of those, we only care about the ones where the picture has the sha1 we're looking for
                // finally, of those, we only care about the cases where the that match exists in the album we're interested in.
                // This query then returns a single xml instance matching these criteria, or throws an exception if 0 or more than 1 are found
                photoInstance = (from picDB in _picturesDatabase.Elements("picture")
                                 join picAlbDB in _albumsDatabase.Descendants("picture")
                                 on (string)picDB.Attribute("sha1") equals (string)picAlbDB.Attribute("sha1")
                                 where (string)picDB.Attribute("sha1") == hash
                                      && (int)picAlbDB.Ancestors("album").Single().Attribute("uid") == albumID
                                 select picAlbDB).Single();
            }
            catch // We only get here if the database is already messed up (two of the same photo in an album)
            {
                throw new ArgumentNullException();
            }
            // If the query returned a single node
            return photoInstance;
        }

        //--------------------------------------------------------------------------
        //By: Ryan Moe, comments by Bill Sanders
        //Edited Last: 3/24/13
        /// <summary>
        /// Searches for a photo by UID in the specified album.
        /// </summary>
        /// <param name="error"></param>
        /// <param name="albumNode"></param>
        /// <param name="photoUID"></param>
        /// <returns>Returns an xml node of the photo from the AlbumDB, or null.</returns>
        private XElement util_getAlbumDBPhotoNode(ErrorReport error, XElement albumNode, int photoUID)
        {
            try
            {
                // Search through a specific album for a specific photo by ID
                // Return an (XElement) picture with the matching uid if it exists
                // Throws exception if it doesn't find exactly 1 match
                return (from c in albumNode.Element("albumPhotos").Elements("picture")
                        where (int)c.Attribute("uid") == photoUID
                        select c).Single();
            }
            catch
            {
                error.reportID = ErrorReport.FAILURE;
                error.description = "Failed to find a single picture by UID.";
                return null;
            }
        }

        //--------------------------------------------------------
        //By: Ryan Moe
        //Edited Last: Bill Sanders, 3/29/13
        //This adds a picture to JUST the picture database.
        //Does not use the UID or the albumName from the newPictureData.
        private ErrorReport util_addPicToPhotoDB(ErrorReport errorReport, ComplexPhotoData newPictureData)
        {

            //if picture extension is not valid
            if (!util_checkPictureExtension(newPictureData.extension))
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "Extension is not valid.";
                return errorReport;
            }

            //if path is not valid
            if (!util_checkPicturePath(newPictureData.path))
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "Path is not valid.";
                return errorReport;
            }

            //make the object that will go into the xml database.
            XElement newPicRoot = new XElement("picture",
                new XAttribute("uid", newPictureData.UID),
                new XAttribute("sha1", ByteArrayToString(newPictureData.hash)),
                new XAttribute("refCount", newPictureData.refCount),
                new XElement("filePath", new XAttribute("extension", newPictureData.extension), newPictureData.path)
                );

            //add to the database (in memory, not on disk).
            _picturesDatabase.Add(newPicRoot);
            return errorReport;
        }

        //--------------------------------------------------------
        //By: Ryan Moe
        //Edited Last: Bill Sanders (4/1/13, removed inline linq, replaced with lookup function; added comments)
        /// <summary>
        /// Adds a photo to a specific album in the album database 
        /// </summary>
        /// <param name="errorReport"></param>
        /// <param name="newPicture"></param>
        /// <param name="albumUID"></param>
        /// <param name="albumName"></param>
        private void util_addPicToAlbumDB(ErrorReport errorReport, ComplexPhotoData newPicture, int albumUID, String albumName)
        {
            //Get the specific album we will be adding to.
            XElement specificAlbum = util_getAlbum(errorReport, albumUID);
            // If the lookup returns null, the album doesn't exist, or there's more than one album with that UID (db error)
            if (specificAlbum == null)
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "Found more than one album with that UID or none at all.";
                return;
            }

            // UID for a photo is unique to an album in the albums database.
            newPicture.UID = util_getNextUID(specificAlbum.Element("albumPhotos"), "picture", 1);
            // check to make sure we got a valid number back...
            if (!util_checkUIDIsValid(newPicture.UID))
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "Photo UID is not valid.";
                return;
            }

            //construct the object we will be adding to the album.
            XElement newPhotoElem = new XElement("picture",
                                            new XAttribute("uid", newPicture.UID),
                                            new XAttribute("sha1", ByteArrayToString(newPicture.hash)),
                                            new XElement("name", albumName),
                                            new XElement("caption", newPicture.caption));

            // Now add it to the albums database
            specificAlbum.Element("albumPhotos").Add(newPhotoElem);

        }

        //--------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //Check UID's here.
        //RETURN: true if the uid is valid, false otherwise.
        private Boolean util_checkUIDIsValid(int uid)
        {
            if (uid > 0 && uid < UID_MAX_SIZE)
                return true;
            return false;
        }

        //--------------------------------------------------------
        // By: Bill Sanders
        // Edited Last: 3/24/13
        // TODO: Speed up by using a byte-array instead of string comparison in LINQ?
        // could make this function return empty list (null?) if hash not found, photo ID if found
        // (photo ID list! what if multiple photos, but not in the album we're adding into)
        /// <summary>
        /// Checks to see if the photo is unique (using a SHA1 hash) to the library
        /// </summary>
        /// <param name="hash">A hex string representation of the hash</param>
        /// <returns></returns>
        private Boolean util_checkPhotoIsUnique(string hash)
        {
            try
            {
                // Try to find a duplicate hash.
                // throws exception if we find NO matching names.
                (from c in _picturesDatabase.Elements("picture")
                 where (String)c.Attribute("sha1") == hash
                 select c).First();
            }
            // Did not find a matching hash, photo is unique
            catch
            {
                return true;
            }
            // Otherwise we found at least one match
            return false;
        }

        //--------------------------------------------------------
        // By: Bill Sanders
        // Edited Last: 3/25/13
        // Possible alternate strategy: Add the photo anyway?  If found, see if it is in a different album.
        // If so, get its XElement, give it a new UID, but keep the rest.
        // TODO: Examine possible speed up by using a byte-array instead of string comparison in LINQ?
        /// <summary>
        /// Checks to see if the photo is unique (using a SHA1 hash) to a given album
        /// </summary>
        /// <param name="albumID">The unique ID of the album</param>
        /// <param name="hash">A hex string representation of the hash of the photo</param>
        /// <returns>False if the photo already exists in this album, otherwise true.</returns>
        private Boolean util_checkPhotoIsUniqueToAlbum(int albumID, string hash)
        {
            // start by assuming the photo does not exist in this album
            Boolean photoExistsInAlbum = false;
            try
            {
                // Try to find a duplicate hash in this album.
                // Note: this may be working inefficiently.
                // Join every picture element from both databases, where the pictures have the same sha1
                // Of those, we only care about the ones where the picture has the sha1 we're looking for
                // finally, of those, we only care about the cases where the that match exists in the album we're interested in.
                // This query then returns true if it found an item matching these criteria, or false if it did not.
                photoExistsInAlbum = (from picDB in _picturesDatabase.Elements("picture")
                                      join picAlbDB in _albumsDatabase.Descendants("picture")
                                      on (string)picDB.Attribute("sha1") equals (string)picAlbDB.Attribute("sha1")
                                      where (string)picDB.Attribute("sha1") == hash
                                           && (int)picAlbDB.Ancestors("album").Single().Attribute("uid") == albumID
                                      select picDB).Any();
            }
            catch // we shouldn't be able to get here, as long as the databases exist
            {
                //throw new ArgumentNullException();
            }
            // If the query returned true, the picture is already in the album, and therefore NOT unique
            return !photoExistsInAlbum;
        }

        //--------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //Check a pictures extension.
        //RETURN: true if the extension is valid, false otherwise.
        private Boolean util_checkPictureExtension(String extension)
        {
            //This is not being use right now because
            //this job has been given to the gui.
            //Uncomment out this to re-enable it, everything
            //is still hooked up!
            /*
            if (extension.Equals(".jpg") || extension.Equals(".png"))
            {
                return true;
            }
            return false;
             */
            return true;
        }

        //--------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //Check a picture's path.
        //RETURN: true if the uid is valid, false otherwise.
        private Boolean util_checkPicturePath(String path)
        {
            if (path != "")
                return true;
            return false;
        }

        /// <summary>
        /// Looks up the photo by hash in the Photos XML database, returns the number of appearances in the Album database.
        /// </summary>
        /// <param name="hash">A string representing the hash value of the file.</param>
        /// <returns>An integer representing the number of times this photo appears in the the Album database</returns>
        private int util_getPicRefCount(string hash)
        {
            ErrorReport errorReport = new ErrorReport();

            XElement specificPhoto = util_getPhotoDBNode(errorReport, hash);

            int refCount;
            if (specificPhoto == null)
            {
                refCount = 0;
            }
            else
            {
                refCount = (int)specificPhoto.Attribute("refCount");
            }

            return refCount;
        }

        //--------------------------------------------------------
        // By: Bill Sanders
        // Edited Last: 3/23/13
        // Should be parallelizeable 
        /// <summary>
        /// Compute the SHA1 Hash of a file
        /// </summary>
        /// <param name="fullFilePath">The path and file name of the file to SHA1</param>
        /// <returns>A byte array representation of the SHA1</returns>
        private byte[] util_getHashOfFile(string fullFilePath)
        {
            // fileHash will hold a byte array representation of the SHA1 hash.
            byte[] fileHash = null;

            // Read the file in as a stream
            try
            {
                using (FileStream fs = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read))
                using (BufferedStream bs = new BufferedStream(fs))
                {
                    // SHA1Managed is the .NET class that actually holds the hashing logic
                    using (SHA1Managed sha1 = new SHA1Managed())
                    {
                        fileHash = sha1.ComputeHash(bs);
                    }
                }
            }
            // If there's some file error, just leave it set to null.
            catch (IOException)
            {
                fileHash = null;
            }

            return fileHash;
        }

        //--------------------------------------------------------
        // By: Bill Sanders
        // Edited Last: 3/23/13
        /// <summary>
        /// Convert an array of bytes to a HexString
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns>The hexadecimal string representation of a byte array</returns>
        public static string ByteArrayToString(byte[] byteArray)
        {
            StringBuilder hexStr = new StringBuilder(byteArray.Length * 2);
            foreach (byte b in byteArray)
            {
                hexStr.AppendFormat("{0:x2}", b);
            }
            return hexStr.ToString();
        }

        //--------------------------------------------------------
        // By: Bill Sanders
        // Edited Last: 3/23/13
        // Should be parallelizeable 
        /// <summary>
        /// Convert a HexString (such as a SHA1 hash) to an array of Bytes
        /// </summary>
        /// <param name="hexStr"></param>
        /// <returns>a byte array</returns>
        private static byte[] StringToByteArray(String hexStr)
        {
            int NumberChars = hexStr.Length / 2;
            byte[] bytes = new byte[NumberChars];
            StringReader sr = new StringReader(hexStr);
            for (int i = 0; i < NumberChars; i++)
            {
                bytes[i] = Convert.ToByte(new string(new char[2] { (char)sr.Read(), (char)sr.Read() }), 16);
            }
            sr.Dispose();
            return bytes;
        }

        //--------------------------------------------------------
        //By: Ryan Moe
        //Edited Last: Bill Sanders (4/1/13) moved search to function call
        //NOTE: this is the slow way of doing it, but
        //      it does not leave holes in the uids.
        //      Ex: if a picture is deleted, we reuse it's uid
        //      when a new picture is added.
        /// <summary>
        /// Performs a linear search through the Photo DB for a valid, unused UID for a photo
        /// </summary>
        /// <param name="potentialUID">The starting point to search from</param>
        /// <returns>A valid UID to use, or -1 if there was a problem</returns>
        private int util_getNextUID(XElement searchNode, string nodeType, int potentialUID = 1)
        {
            int newUID;//the UID we will search against and return eventually.

            //error checking the starting point.
            if (util_checkUIDIsValid(potentialUID))
            {
                newUID = potentialUID;
            }
            else
            {
                newUID = 1;//default starting point.
            }

            Boolean uidFound = false;
            // fetch from the photo DB by uid until we find one that returns null
            while (!uidFound && newUID < UID_MAX_SIZE)
            {
                try
                {
                    //if one or more (hope not more!) uid's are found
                    //to match our testing uid, then incriment the testing
                    //uid and try again.
                    (from c in searchNode.Elements(nodeType)
                     where (int)c.Attribute("uid") == newUID
                     select c).First();
                    ++newUID;
                }
                //we found a unique one!
                catch
                {
                    uidFound = true;
                }
            }

            // if we scanned through all UID's until UID_MAX_SIZE (!!)
            if (newUID > UID_MAX_SIZE)
            {
                return -1;
            }
            else
            {
                return newUID;
            }
        }

        //--------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //load the database from disk into memory.
        // BS: This function is being slated for merging with util_openPicturesXML
        private void util_openAlbumsXML(ErrorReport error)
        {
            try
            {
                _albumsDatabase = XDocument.Load(albumsDatabasePath).Element(Properties.Settings.Default.XMLRootElement);
            }
            catch
            {
                error.reportID = ErrorReport.FAILURE;
                error.description = "PhotoBomb.openAlbumsXML():failed to load the albums xml file: " + albumsDatabasePath;
                return;
            }

            //The loading of the xml was nominal.
            error.description = "great success!";
        }

        //-------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //load the database from disk into memory.
        // BS: This function is being slated for merging with util_openAlbumXML
        private void util_openPicturesXML(ErrorReport error)
        {
            try
            {
                _picturesDatabase = XDocument.Load(picturesDatabasePath).Element(Properties.Settings.Default.XMLRootElement);
            }
            catch
            {
                error.reportID = ErrorReport.FAILURE;
                error.description = "PhotoBomb.openPicturesXML():failed to load the pictures xml file: " + picturesDatabasePath;
                return;
            }

            //The loading of the xml was nominal.
            error.description = "great success!";
        }

        // Commenting this out, its functionality has been merged with util_getNextUID()
        // to replicate: util_getNextUID(_albumsDatabase, "album", 1)
        //------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //Returns a new VALID uid for a new album.
        //NOTE: this is the slow way of doing it, but
        //      it does not leave holes in the uids.
        //      Ex: if an album is deleted, we reuse it's uid
        //      when a new album is added.
        //RETURNS -1 if there was a problem getting a new uid.
        //private int util_getNewAlbumUID(ErrorReport error)
        //{
        //    int newUID = 1;
        //    Boolean uidNotFound = true;
        //    while (uidNotFound && newUID < UID_MAX_SIZE)
        //    {
        //        try
        //        {
        //            //if one or more (hope not more!) uid's are found
        //            //to match our testing uid, then incriment the testing
        //            //uid and try again.
        //            (from c in _albumsDatabase.Elements("album")
        //             where (int)c.Attribute("uid") == newUID
        //             select c).First();
        //            ++newUID;
        //        }
        //        //we found a unique one!
        //        catch
        //        {
        //            uidNotFound = false;
        //        }
        //    }//while

        //    if (newUID < UID_MAX_SIZE)
        //        return newUID;
        //    return -1;
        //}

        //-------------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //add an album to the database in memory ONLY.
        private void util_addAlbumToAlbumDB(ErrorReport errorReport, SimpleAlbumData albumData)
        {
            //make sure the album database is valid.
            if (!util_checkAlbumDatabase(errorReport))
            {
                return;
            }

            //construct the object we will be adding to the database.
            XElement newAlbum = new XElement("album",
                                            new XAttribute("uid", albumData.UID),
                                            new XElement("albumName", albumData.albumName),
                                            new XElement("albumPhotos"));

            //add to the database in memory.
            _albumsDatabase.Add(newAlbum);
        }//method

        //-------------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //use this to convert a photo element into a complexPhotoData data class.
        //Try and keep this updated if new fields are added to complexPhotoData.
        private ComplexPhotoData util_convertPhotoNodeToComplexPhotoData(ErrorReport errorReport, XElement elem)
        {
            ComplexPhotoData data = new ComplexPhotoData();

            //TRANSFER ALL DATA TO THE DATA CLASS HERE.
            try
            {
                data.UID = (int)elem.Attribute("UID");
                data.hash = StringToByteArray((string)elem.Attribute("SHA1"));
                data.refCount = (int)elem.Attribute("refCount");
                data.path = elem.Element("filePath").Value;
                data.extension = (String)elem.Element("filePath").Attribute("extension");
            }
            catch
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "Error converting XElement to struct.";
                return null;
            }

            return data;
        }

        //----------------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //use this to convert a photo element into a simplePhotoData data class.
        //Try and keep this updated if new fields are added to simplePhotoData.
        private SimplePhotoData util_convertPhotoElemToSimpleStruct(ErrorReport errorReport, XElement elem)
        {
            SimplePhotoData data = new SimplePhotoData();

            //TRANSFER ALL DATA TO THE DATA CLASS HERE.
            try
            {
                data.UID = (int)elem.Attribute("UID");
                data.picturesNameInAlbum = elem.Element("name").Value;

            }
            catch
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "Error getting required data from photo element.";
                return null;
            }

            return data;
        }

        //-------------------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //returns true if the album name is unique.
        private Boolean util_checkAlbumNameIsUnique(String albumName)
        {
            try
            {
                //try and find a matching album name.
                //throws exception if we find NO matching names.
                (from c in _albumsDatabase.Elements("album")
                 where (String)c.Element("albumName") == albumName
                 select c).First();
            }
            //we didn't find a matching name, success!
            catch
            {
                return true;
            }
            return false;
        }

        //-------------------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //copies a picture into our picture library.
        //DOES NOT CHECK picturePath or picName, please do before hand.
        private String util_copyPicToLibrary(ErrorReport errorReport, String picturePath, String picNameInLibrary)
        {
            //check if file exists first!!!
            //if the picture does NOT exist.
            if (!File.Exists(picturePath))
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "Can't find the new picture to import to the library.";
                return "";
            }

            //check if the library is ok.
            if (!util_checkLibraryDirectory())
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "Something is wrong with the photo library.";
                return "";
            }

            //make the full picture path.
            String newPath = System.IO.Path.Combine(libraryPath, picNameInLibrary);

            // I haven't thought much about whether or not this is the right place to put this
            Imazen.LightResize.ResizeJob resizeJob = new Imazen.LightResize.ResizeJob();
            // specifies a maximum height resolution constraint 
            resizeJob.Height = 120;
            // Actually processes the image, copying it to the new location, should go in a try/catch for IO
            // One of Build's overloads allows you to use file streams instead of filepaths.
            // If images have to be resized on-the-fly instead of stored, that may work as well.
            resizeJob.Build(
                picturePath,
                System.IO.Path.Combine(
                    libraryPath,
                    Properties.Settings.Default.PhotoLibraryThumbsDir,
                    picNameInLibrary),
                Imazen.LightResize.JobOptions.CreateParentDirectory
            );

            try
            {
                //copy the photo to the library.
                System.IO.File.Copy(picturePath, newPath, true);
            }
            catch
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "Unable to make a copy of the photo in the library.";
                return "";
            }

            //new library path
            return newPath;
        }

        //-------------------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //check if the library is ok.
        private Boolean util_checkLibraryDirectory()
        {
            //anything else we need to check?
            return (Directory.Exists(libraryPath));
        }

        //-------------------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //check if the library is ok, with ErrorReport param.
        private void util_checkLibraryDirectory(ErrorReport error)
        {
            //anything else we need to check?
            if (!Directory.Exists(libraryPath))
            {
                error.reportID = ErrorReport.FAILURE;
                error.description = "Library folder not found.";
            }
        }

        //--------------------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last: Bill Sanders 4/1/13 (renamed function, added comments)
        /// <summary>
        /// Renames an instance of a photo in an album
        /// </summary>
        /// <param name="error"></param>
        /// <param name="photoElem">An XElement representation of the photo from the album DB</param>
        /// <param name="newName">The new name for the photo</param>
        private void util_renamePhoto(ErrorReport error, XElement photoElem, String newName)
        {
            try
            {
                photoElem.Element("name").Value = newName;
            }
            catch
            {
                // this probably means they passed in a XElem from the pictures library...?
                error.reportID = ErrorReport.FAILURE;
                error.description = "Failed to change the name of a photo.";
            }
        }

        //--------------------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last: Bill Sanders, 3/29/13
        /// <summary>
        /// Queries the Album database for the specified album.
        /// </summary>
        /// <param name="error"></param>
        /// <param name="albumUID">The Unique ID of the album</param>
        /// <returns>An XElement object referring to the specified album, or null if not found.</returns>
        private XElement util_getAlbum(ErrorReport error, int albumUID)
        {
            try
            {
                //find and return an album whos uid is the one we are looking for.
                //Throws exception if none or more than one match is found.
                return (from c in _albumsDatabase.Elements("album")
                        where (int)c.Attribute("uid") == albumUID
                        select c).Single();
            }
            catch
            {
                error.reportID = ErrorReport.FAILURE;
                error.description = "Failed to find a single album by UID.";
                return null;
            }
        }

        //---------------------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //returns true if the album database is ok.
        private Boolean util_checkAlbumDatabase(ErrorReport errorReport)
        {
            if (_albumsDatabase == null)
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "PhotoBomb: The album database has not been loaded yet!";
                return false;
            }

            //put more checks here.

            return true;
        }
        //-----------------------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //returns true if the picture database is ok.
        private Boolean util_checkPhotoDBIntegrity(ErrorReport errorReport)
        {
            if (_picturesDatabase == null)
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "PhotoBomb: The album database has not been loaded yet!";
                return false;
            }

            //put more checks here.

            return true;
        }
    }//class
}
