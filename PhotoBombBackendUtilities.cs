/**
 * By: Ryan Moe
 * 
 * This class is for utility functions for the PhotoBomb backend.
 * By utility I mean for functions that do a single simple task
 * that many other funcitons may want to use.
 **********************************************************************************
 * Changelog:
 * 4/1/13 Bill Sanders: removed inline linq, replaced with lookup function; added comments
 * 4/3/13 Bill Sanders: added comments, thumbnail generation
 * 4/5/13 Ryan Causey: moved generate thumbnail utility calls out of copy photo to library
 *                     utility function call and into addNewPicture_backend in PhotoBombBackend.cs
 *                     Editing thumbnail generation utility function to return path to thumbnail.
 *                     Edited addPhotoToDB and getComplexPhotoData to include new thumbnail paths.
 * 4/6/13 Ryan Causey: Fixed a bug where the thumbnail path for the album was being set to an invalid path.
 * 4/8/13 Ryan Causey: Fixed a bug in the check caption is valid utility.
 *                     Fixed a bug in the change caption utility where we were trying to access caption as
 *                     an attribute instead of an element.
 *                     Fixed a bug in thumbnail generation utility.
 * Julian Nguyen(4/30/13)
 * ErrorReports constants numbers removed and replaced with ReportStatus enums.
 * Julian Nguyen(5/1/13)
 * setErrorReportToFAILURE() replaced setting an an ErrorReport to FAILURE and it's description.
 * Fun() with "Picture" in the name were changed to "Image"
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;
using System.IO;
using System.Security.Cryptography; //Namespace for SHA1

namespace SoftwareEng
{
    public partial class PhotoBomb
    {
       
        /// By: Bill Sanders
        /// Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// Query the Photo DB for a photo by hash
        /// </summary>
        /// <param name="error"></param>
        /// <param name="imageHash"></param>
        /// <returns>An XElement object representing the photo in the PhotoDB</returns>
        private XElement util_getImageDBNode___(ErrorReport error, string imageHash)
        {
            if (!util_checkPhotoDBIntegrity(error))
            {
                setErrorReportToFAILURE("Found more than one picture with that hash!", ref error);
                return null;
            }

            return _photoBomb_xml.getImageNodeFromImageXml(imageHash, _imagesRootXml);

        }//method


        /// By: Ryan Moe
        /// Edited Last: Julian Nguyen(5/1/13)
        /// <summary>
        /// Query the Photo DB for a photo by hash
        /// </summary>
        /// <param name="error"></param>
        /// <param name="photoUID"></param>
        /// <returns>An XElement object representing the photo in the PhotoDB</returns>
        [Obsolete]
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
                    specificPicture = (from c in _imagesRootXml.Elements()
                                       where (int)c.Attribute("uid") == photoUID
                                       select c).Single();//NOTE: this will throw error if more than one OR none at all.
                }
                //failed to find the picture
                catch
                {
                    setErrorReportToFAILURE("Failed to find the picture specified.", ref error);
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
        /// Retrieves a specific photo instance from a combination of the AlbumID and the photo's ID in that album
        /// </summary>
        /// <param name="albumUID">The unique ID of the album</param>
        /// <param name="inAlbumID">The ID number of the image in that album</param>
        /// <returns>Returns an xml node of the photo from the AlbumDB, or null.</returns>
        [Obsolete]
        private XElement getAlbumImageNodeFromAlbumXml(Guid albumUID, int inAlbumID)
        {
            // first get the album by albumID
            XElement albumNode = _photoBomb_xml.getAlbumNodeFromAlbumXml(albumUID, _albumsRootXml);
            try
            {
                return _photoBomb_xml.getAlbumImageNodeFromAlbumNode(albumNode, inAlbumID);        
            }
            catch
            {
                return null;
            }
        }
        

        /// By Ryan Moe
        /// Edited: Julan nguyen(5/1/13)
        /// <summary>
        /// This adds a picture to JUST the picture database.
        /// </summary>
        /// <param name="errorReport">Why is this being passed in??</param>
        /// <param name="newPictureData">The data of the new image to be added. </param>
        /// <returns>The errorReport of this action.</returns>
        private ErrorReport util_addImageToImageDB(ErrorReport errorReport, ComplexPhotoData newPictureData)
        {
            // TODO: Test the incoming errorReport for failure. 

            //if picture extension is not valid
            if (!util_checkPhotoExtension(newPictureData.extension))
            {
                setErrorReportToFAILURE("Extension is not valid.", ref errorReport);
                return errorReport;
            }

            //if path is not valid
            if (!util_checkFilePath(newPictureData.fullPath))
            {
                setErrorReportToFAILURE("Path is not valid", ref errorReport);
                return errorReport;
            }

            //make the object that will go into the xml database.
            XElement newPicRoot = new XElement("picture",
                new XAttribute("uid", newPictureData.UID),
                new XAttribute("sha1", ByteArrayToString(newPictureData.hash)),
                new XAttribute("refCount", newPictureData.refCount),
                new XElement("filePath", new XAttribute("extension", newPictureData.extension), newPictureData.fullPath),
                new XElement("lgThumbPath", newPictureData.lgThumbPath),
                new XElement("dateAdded", DateTime.Today.ToString())
                );

            //add to the database (in memory, not on disk).
            _imagesRootXml.Add(newPicRoot);
            return errorReport;
        }


        /// By Ryan Moe
        /// Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// Adds a image to a specific album in the album database 
        /// </summary>
        /// <param name="errorReport"></param>
        /// <param name="newImage">An object with all the data necessary to create the Image in both DBs</param>
        /// <param name="albumUID">The unique ID of the album</param>
        private void util_addImageToAlbumDB(ErrorReport errorReport, ComplexPhotoData newImage, Guid albumUID)
        {
            //Get the specific album we will be adding to.
            XElement specificAlbum = _photoBomb_xml.getAlbumNodeFromAlbumXml(albumUID, _albumsRootXml);
            // If the lookup returns null, the album doesn't exist, or there's more than one album with that UID (db error)
            if (specificAlbum == null)
            {
                setErrorReportToFAILURE("Found more than one album with that UID or none at all.", ref errorReport);
                return;
            }

            // idInAlbum for a photo is unique to an album in the albums database.
            newImage.idInAlbum = util_getNextUID(specificAlbum.Element("albumPhotos"), "picture", "idInAlbum", 1);
            // check to make sure we got a valid number back...
            if (!util_checkIDIsValid(newImage.idInAlbum))
            {
                setErrorReportToFAILURE("Photo id for album is not valid.", ref errorReport);
                return;
            }

            // check to see if this is going to be the first photo in an otherwise empty library...
            XElement photoNeighbor = (from c in specificAlbum.Descendants("picture") select c).FirstOrDefault();
            // ... If there are no neighbors, this is the solo photo in the album, and thus the first...
            if (photoNeighbor == null)
            {
                // ... so set it to be the album thumbnail
                util_setAlbumThumbnail(specificAlbum, newImage);
            }

            // Note as per requirements, the default photo name is the name of the album, plus its id number
            string nameInAlbum = specificAlbum.Element("albumName").Value + " Image " + newImage.idInAlbum;
            while (!_photoBomb_xml.isImageNameUniqueToAlbum(nameInAlbum, specificAlbum))
            {
                newImage.idInAlbum = util_getNextUID(specificAlbum.Element("albumPhotos"), "picture", "idInAlbum", newImage.idInAlbum + 1);
                nameInAlbum = specificAlbum.Element("albumName").Value + " Image " + newImage.idInAlbum;
            }

             //construct the object we will be adding to the album.
            XElement newImageNode = _photoBomb_xml.newAlbumImageNode(newImage.idInAlbum, ByteArrayToString(newImage.hash), nameInAlbum, newImage.caption);

  
            // Now add it to the albums database in memory
            specificAlbum.Element("albumPhotos").Add(newImageNode);
        }





        //--------------------------------------------------------
        // By: Bill Sanders
        // Edited Last By: Ryan Causey
        // Edited Last Date: 4/6/13
        /// <summary>
        /// Sets the thumbnail path for an album.
        /// </summary>
        /// <param name="albumNode">An XElement of the album to change the thumbnail of</param>
        /// <param name="photoObject">A ComplexPhotoData object which contains the path information</param>
        private void util_setAlbumThumbnail(XElement albumNode, ComplexPhotoData photoObject)
        {
            ErrorReport errorReport = new ErrorReport();
            String thumbPath;
            // If the objects look good, set up the (long...) path.
            if ((albumNode != null) && (photoObject != null))
            {
                thumbPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    Settings.OrgName,
                    Settings.PhotoLibraryName,
                    Settings.PhotoLibraryThumbsDir,
                    Settings.lrgThumbDir,
                    photoObject.UID.ToString() +
                    photoObject.extension
                    );

                thumbPath = util_generateThumbnail(
                    errorReport,
                    photoObject.fullPath,
                    photoObject.UID.ToString() + photoObject.extension,
                    Settings.lrgThumbSize);
            }
            // If either of these objects is null, just invalidate the path
            else
            {
                thumbPath = String.Empty;
            }

            albumNode.Element("thumbnailPath").Value = thumbPath;
            albumNode.Element("thumbnailPath").Attribute("thumbAlbumID").Value = photoObject.idInAlbum.ToString();
        }

        /// By: Bill Sanders
        /// Edited: Julian Nguyen(5/1/13)
        /// <summary>
        /// Sets the caption of a photo in a specific album to the provided string
        /// </summary>
        /// <param name="error">The errorReport. </param>
        /// <param name="albumPhotoNode">The XElement object of the photo </param>
        /// <param name="caption">The new caption to set for this image.</param>
        private void util_setPhotoCaption(ErrorReport error, XElement albumPhotoNode, string caption)
        {
            try
            {
                albumPhotoNode.Element("caption").Value = caption;
            }
            catch
            {
                setErrorReportToFAILURE("Failed to change the caption of a photo.", ref error);
            }
        }

        //--------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //Check UID's here.
        //RETURN: true if the uid is valid, false otherwise.
        private Boolean util_checkIDIsValid(int id)
        {
            if (id > 0 && id < UID_MAX_SIZE)
                return true;
            return false;
        }

        //--------------------------------------------------------
        //By: Bill Sanders
        //Edited Last By: Ryan Causey
        //Edited Last Date: 4/8/13
        //RETURN: true if the caption is valid, false otherwise.
        private Boolean util_checkCaptionIsValid(string caption)
        {
            // other checks go here.
            if ((caption.Length >= 0) && (caption.Length < Settings.CaptionMaxLength))
                return true;
            return false;
        }

        //--------------------------------------------------------
        // By: Bill Sanders
        // Edited Last: 4/4/13
        // Scrapped the lookup query in favor of calling a function
        /// <summary>
        /// Checks to see if the photo is unique (using a SHA1 hash) to the library
        /// </summary>
        /// <param name="hash">A hex string representation of the hash</param>
        /// <returns>Returns True if the photo does not already exist in the library</returns>
        private Boolean util_checkPhotoIsUniqueToLibrary(string hash)
        {
            XElement Element = _photoBomb_xml.getImageNodeFromImageXml(hash, _imagesRootXml);

            // If the query returned null, we did not find a match, so the photo is unique
            if (Element == null)
            {
                return true;
            }
            // The query returned an element, which means this photo already exists.
            else
            {
                return false;
            }
        }


        /// By: Bill Sanders
        /// Edited Julian Nguyen(5/1/13)
        /// BS: 
        /// Possible alternate strategy: Add the photo anyway?  If found, see if it is in a different album.
        /// If so, get its XElement, give it a new UID, but keep the rest.
        /// TODO: Examine possible speed up by using a byte-array instead of string comparison in LINQ?
        /// <summary>
        /// Checks to see if the photo is unique (using a SHA1 hash) to a given album
        /// </summary>
        /// <param name="albumID">The unique ID of the album</param>
        /// <param name="imageHash">A hex string representation of the hash of the image</param>
        /// <returns>If the image name is unique or not.</returns>
        private Boolean util_isImageUniqueToAlbum(Guid albumID, string imageHash)
        {
            // start by assuming the photo does not exist in this album
            //Boolean photoExistsInAlbum = false;
            XElement image = _photoBomb_xml.getImageNode(albumID, imageHash, _albumsRootXml, _imagesRootXml);

            // If the photo lookup returns null, the photo is not in this album, so the photo woudl be unique to this album
            return (image == null);
        }

        //--------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //Check a pictures extension.
        //RETURN: true if the extension is valid, false otherwise.
        private Boolean util_checkPhotoExtension(String extension)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private Boolean util_checkFilePath(String path)
        {
            if (path != String.Empty)
                return true;
            return false;
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

        
        /// By: Bill Sanders
        /// Edited Last: 3/23/13
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

        
        // By: Bill Sanders
        // Edited Last: 3/23/13
        // Should be parallelizeable 
        /// <summary>
        /// Convert a HexString (such as a SHA1 hash) to an array of Bytes
        /// </summary>
        /// <param name="hexStr"></param>
        /// <returns>a byte array</returns>
        public static byte[] stringToByteArray(String hexStr)
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
        /// <param name="potentialID">The starting point to search from</param>
        /// <returns>A valid UID to use, or -1 if there was a problem</returns>
        private int util_getNextUID(XElement searchNode, string nodeType, string attributeName, int potentialID = 1)
        {
            int newID;//the UID we will search against and return eventually.

            //error checking the starting point.
            if (util_checkIDIsValid(potentialID))
            {
                newID = potentialID;
            }
            else
            {
                newID = 1;//default starting point.
            }

            Boolean uidFound = false;
            // fetch from the photo DB by uid until we find one that returns null
            while (!uidFound && newID < UID_MAX_SIZE)
            {
                try
                {
                    //if one or more (hope not more!) uid's are found
                    //to match our testing uid, then incriment the testing
                    //uid and try again.
                    (from c in searchNode.Elements(nodeType)
                     where (int)c.Attribute(attributeName) == newID
                     select c).First();
                    ++newID;
                }
                //we found an unused one!
                catch
                {
                    uidFound = true;
                }
            }

            // if we scanned through all UID's until UID_MAX_SIZE (!!)
            if (newID > UID_MAX_SIZE)
            {
                return -1;
            }
            else
            {
                return newID;
            }
        }



  

        //-------------------------------------------------------
        //By: Bill Sanders
        //Edited Last: Bill Sanders 4/8/13
        /// <summary>
        /// Removes all instances of the picture from the DB files
        /// </summary>
        /// <param name="error"></param>
        /// <param name="hash">The hash of the file to remove</param>
        private void util_purgePhotoFromDB(ErrorReport error, string hash)
        {
            // First get all of the intsances of it from the AlbumDB
            IEnumerable<XElement> albumDBPhotos = util_getAlbumDBPhotoNodes(error, hash);
            // Note, foreaching over an IEnumerable is sometimes broken, so .ToList()
            foreach (XElement photoNode in albumDBPhotos.ToList())
            {
                // And remove them!
                photoNode.Remove();
            }

            // Now get the reference to it in the Photos DB
            XElement imageNode = _photoBomb_xml.getImageNodeFromImageXml(hash, _imagesRootXml);
            // And git rid of it and remove it from the backend.
            if(imageNode != null)
                removeImageFromImageDB_backend(imageNode);

        }

        //-------------------------------------------------------
        /// By: Bill Sanders
        /// Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// Return all of the pictures in the AlbumsDB that match the provided hash
        /// </summary>
        /// <param name="error"></param>
        /// <param name="hash">The hash to search for</param>
        private IEnumerable<XElement> util_getAlbumDBPhotoNodes(ErrorReport error, string hash)
        {
            // Don't bother with an empty hash...
            // in the future I guess this could return every picture instance in the DB?
            if (hash == string.Empty)
            {
                setErrorReportToFAILURE("Invalid sha1", ref error);
                return null;
            }

            // The return var
            IEnumerable<XElement> allAlbumDBPhotos = new List<XElement>();

            // Descendants() lets us search for the element ignoring tag nesting
            try
            {
                allAlbumDBPhotos = (from c in _albumsRootXml.Descendants("picture")
                                    where (string)c.Attribute("sha1") == hash
                                    select c);
            }
            catch
            {
                // how do we get here?
                throw new NotImplementedException();
            }
            return allAlbumDBPhotos;
        }



        //-------------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //add an album to the database in memory ONLY.
        private void addAlbumNodeToAlbumsXml__(SimpleAlbumData albumData)
        {

            //construct the object we will be adding to the database.
            XElement newAlbum = new XElement("album",
                new XAttribute("uid", albumData.UID),
                new XElement("thumbnailPath", new XAttribute("thumbAlbumID", albumData.thumbAlbumID), albumData.thumbnailPath),
                new XElement("albumName", albumData.albumName),
                new XElement("albumPhotos"));

            //add to the database in memory.
            _albumsRootXml.Add(newAlbum);

        }//method



        /// By: Julian Nguyen
        /// Edited Julian Nguyen(5/7/13)
        /// <summary>
        /// Will take a time in String form and return a datetime.
        /// </summary>
        /// <param name="sDate"></param>
        /// <returns></returns>
        public DateTime stringToDateTime(String sDate)
        {

            DateTime date = DateTime.MinValue;

            if (!DateTime.TryParse(sDate, out date))
            {
                return DateTime.MinValue;
            }

            return date;
        }



        

        /// By: Ryan Moe
        /// Edited Julian Nguyen(5/1/13)
        /// Currently, as a side effect, this function also generates thumbnails.
        /// <summary>
        /// Copies a picture from a source to the library on the filesystem.
        /// </summary>
        /// <param name="errorReport"></param>
        /// <param name="srcImageFullFilepath"></param>
        /// <param name="imageNameInLibrary"></param>
        /// <returns>A string representing the path of the new file.</returns>
        private String util_copyImageToLibrary(ErrorReport errorReport, String srcImageFullFilepath, String imageNameInLibrary)
        {
            //check if file exists first!!!
            //if the picture does NOT exist.
            if (!File.Exists(srcImageFullFilepath))
            {
                setErrorReportToFAILURE("Can't find the new picture to import to the library", ref errorReport);
                return String.Empty;
            }

            //check if the library is ok.
            if (!util_checkLibraryDirectory())
            {
                setErrorReportToFAILURE("Something is wrong with the photo library.", ref errorReport);
                return String.Empty;
            }

            // Create the full path where the picture will go
            String newPath = System.IO.Path.Combine(_imagelibraryDirPath, imageNameInLibrary);

            // Wrapped in a try primarily in case of IO errors
            try
            {
                //copy the photo to the library.
                System.IO.File.Copy(srcImageFullFilepath, newPath, true);
            }
            catch
            {
                setErrorReportToFAILURE("Unable to make a copy of the photo in the library.", ref errorReport);
                return String.Empty;
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
            return (Directory.Exists(_imagelibraryDirPath));
        }


        /// By Ryan Moe
        /// By Julian Nguyen(5/1/13)
        /// <summary>
        /// check if the library is ok, with ErrorReport param.
        /// </summary>
        /// <param name="error">The errorReport for this action, for bool are not good enough.</param>
        private void util_checkLibraryDirectory(ErrorReport error)
        {
            //anything else we need to check?
            if (!Directory.Exists(_imagelibraryDirPath))
            {
                setErrorReportToFAILURE("Library folder not found.", ref error);
            }
        }
        

        /// By Ryan Moe
        /// Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// returns true if the album database is ok.
        /// </summary>
        /// <param name="errorReport">The errorReport for this action. </param>
        /// <returns>If the album is good or not.</returns>
        private bool util_checkAlbumDatabase(ErrorReport errorReport)
        {
            if (_albumsRootXml == null)
            {
                setErrorReportToFAILURE("PhotoBomb: The album database has not been loaded yet!", ref errorReport);
                return false;
            }

            //put more checks here.

            return true;
        }


        /// By Ryan Moe
        /// Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// returns true if the picture database is ok.
        /// </summary>
        /// <param name="errorReport">The errorReport for this action.</param>
        /// <returns>returns true if the picture database is ok else false.</returns>
        private Boolean util_checkPhotoDBIntegrity(ErrorReport errorReport)
        {
            if (_imagesRootXml == null)
            {
                setErrorReportToFAILURE("PhotoBomb: The album database has not been loaded yet!", ref errorReport);
                return false;
            }
            
            //put more checks here.

            return true;
        }

        /// By ??
        /// Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// Will make a new database for photobomb. (new xml files.)
        /// </summary>
        /// <param name="errorReport"></param>
        private void createDefaultXML(ErrorReport errorReport)
        {
            XDocument initDB = new XDocument();
            XElement root = new XElement(Settings.XMLRootElement);
            initDB.Add(root);
            try
            {
                initDB.Save(_albumsXmlPath);
                initDB.Save(_imageXmlPath);
            }
            catch
            {
                setErrorReportToFAILURE("Unable to create the new database files.", ref errorReport);
            }
        }

        //---------------------------------------------------------------------------
        /// By: Bill Sanders
        /// Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// Generates a thumbnail for a specified image file and places it in an appropriate sub directory
        /// </summary>
        /// <param name="error"></param>
        /// <param name="srcPath"></param>
        /// <param name="picFileName"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private String util_generateThumbnail(ErrorReport error, string srcPath, string picFileName, int size)
        {
            if ((srcPath == string.Empty) || (picFileName == string.Empty) || !File.Exists(srcPath))
            {
                setErrorReportToFAILURE("Unable to generate thumbnail, bad path or filename.", ref error);
                return String.Empty; 
            }
            // I haven't thought much about whether or not this is the right place to put this
            Imazen.LightResize.ResizeJob resizeJob = new Imazen.LightResize.ResizeJob();
            string thumbSubDir = String.Empty;
            string fullThumbPath = String.Empty;

            // Which sub directory of thumbs_db to put this in...
            if (size == Settings.lrgThumbSize)
            {
                thumbSubDir = Settings.lrgThumbDir;
            }

            // Specifies a maximum height resolution constraint to scale the image down to
            resizeJob.Height = size;
            resizeJob.Width = size;
            resizeJob.Mode = Imazen.LightResize.FitMode.Crop;

            //get the full path
            fullThumbPath = System.IO.Path.Combine(_imagelibraryDirPath, Settings.PhotoLibraryThumbsDir, thumbSubDir, picFileName);

            // Actually processes the image, copying it to the new location, should go in a try/catch for IO
            // One of Build's overloads allows you to use file streams instead of filepaths.
            // If images have to be resized on-the-fly instead of stored, that may work as well.
            resizeJob.Build(
                srcPath,
                fullThumbPath,
                Imazen.LightResize.JobOptions.CreateParentDirectory
            );

            return fullThumbPath;
        }



    }//class
}
