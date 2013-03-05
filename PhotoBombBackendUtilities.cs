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

namespace SoftwareEng
{



    public partial class PhotoBomb
    {


        //----------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //RETURN: the element that has the param uid from the picture database.
        private XElement util_getComplexPictureByUID(ErrorReport error, int uid)
        {
            if (util_checkPicturesDatabase(error))
            {
                //Try searching for the album with the uid specified.
                XElement specificPicture;
                try
                {
                    //for(from) every c in the database's children (all albums),
                    //see if it's attribute uid is the one we want,
                    //and if so return the first instance of a match.
                    specificPicture = (from c in _picturesDatabase.Element("database").Elements()
                                       where (int)c.Attribute("uid") == uid
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
        //By: Ryan Moe
        //Edited Last:
        //This adds a picture to JUST the picture database.
        //Does not use the UID or the albumName from the newPictureData.
        private ErrorReport util_addPicToPicDatabase(ErrorReport errorReport, ComplexPhotoData newPictureData)
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
                new XElement("filePath", new XAttribute("extension", newPictureData.extension), newPictureData.path)
                );

            //add to the database (in memory, not on disk).
            _picturesDatabase.Element("database").Add(newPicRoot);
            return errorReport;
        }

        //--------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //This adds a picture to JUST the album database.
        //Does not use the UID or the albumName from the newPictureData.
        private void util_addPicToAlbumDatabase(ErrorReport errorReport, ComplexPhotoData newPicture, int albumUID, String albumName)
        {
            //Get the specific album we will be adding to.
            XElement specificAlbum;
            try
            {
                specificAlbum = (from c in _albumsDatabase.Element("database").Elements()
                                 where (int)c.Attribute("uid") == albumUID
                                 select c).Single();//NOTE: this will throw error if more than one OR none at all.
            }
            catch
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "Found more than one album with that UID or none at all.";
                return;
            }

            //check UID
            if (!util_checkPhotoUID(newPicture.UID))
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "Photo UID is not valid.";
                return;
            }

            //construct the object we will be adding to the album.
            XElement newPhotoElem = new XElement("picture",
                                            new XAttribute("uid", newPicture.UID),
                                            new XElement("name", albumName));


            specificAlbum.Element("albumPhotos").Add(newPhotoElem);

        }

        //--------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //Check UID's here.
        //RETURN: true if the uid is valid, false otherwise.
        private Boolean util_checkPhotoUID(int uid)
        {
            if (uid > 0 && uid < UID_MAX_SIZE)
                return true;
            return false;
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

        //--------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //Check a picture's album name.
        //RETURN: true if the uid is valid, false otherwise.
        private Boolean util_checkPictureAlbumName(String name)
        {
            return true;
        }


        //--------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //Returns a new VALID uid for a new picture.
        //PARAM 1 = where to start linearly searching for a new uid.
        //          Use this for searching speed up!
        //NOTE: this is the slow way of doing it, but
        //      it does not leave holes in the uids.
        //      Ex: if a picture is deleted, we reuse it's uid
        //      when a new picture is added.
        //RETURNS -1 if there was a problem getting a new uid.
        private int util_getNewPicUID(int searchStartingpoint)
        {
            int newUID;//the UID we will search against and return eventually.

            //error checking the starting point.
            if (searchStartingpoint > 0 && searchStartingpoint < UID_MAX_SIZE)
                newUID = searchStartingpoint;
            else
                newUID = 1;//default starting point.

            Boolean uidNotFound = true;

            //loop through and test uid's until we find one
            //that works.
            while (uidNotFound && newUID < UID_MAX_SIZE)
            {
                try
                {
                    //if one or more (hope not more!) uid's are found
                    //to match our testing uid, then incriment the testing
                    //uid and try again.
                    (from c in _picturesDatabase.Element("database").Elements("picture")
                     where (int)c.Attribute("uid") == newUID
                     select c).First();
                    ++newUID;
                }
                //we found a unique one!
                catch
                {
                    uidNotFound = false;
                }
            }//while

            if (newUID < UID_MAX_SIZE)
                return newUID;
            return -1;
        }

        //--------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //load the database from disk into memory.
        private void util_openAlbumsXML(ErrorReport error)
        {
            try
            {
                _albumsDatabase = XDocument.Load(albumsDatabasePath);
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
        private void util_openPicturesXML(ErrorReport error)
        {
            try
            {
                _picturesDatabase = XDocument.Load(picturesDatabasePath);
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

        //------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //Returns a new VALID uid for a new album.
        //NOTE: this is the slow way of doing it, but
        //      it does not leave holes in the uids.
        //      Ex: if an album is deleted, we reuse it's uid
        //      when a new album is added.
        //RETURNS -1 if there was a problem getting a new uid.
        private int util_getNewAlbumUID(ErrorReport error)
        {
            int newUID = 1;
            Boolean uidNotFound = true;
            while (uidNotFound && newUID < UID_MAX_SIZE)
            {
                try
                {
                    //if one or more (hope not more!) uid's are found
                    //to match our testing uid, then incriment the testing
                    //uid and try again.
                    (from c in _albumsDatabase.Element("database").Elements("album")
                     where (int)c.Attribute("uid") == newUID
                     select c).First();
                    ++newUID;
                }
                //we found a unique one!
                catch
                {
                    uidNotFound = false;
                }
            }//while

            if (newUID < UID_MAX_SIZE)
                return newUID;
            return -1;
        }

        //-------------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //add an album to the database in memory ONLY.
        private void util_addAlbumToAlbumDatabase(ErrorReport errorReport, SimpleAlbumData albumData)
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
            _albumsDatabase.Element("database").Add(newAlbum);
        }//method

        //-------------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //use this to convert a photo element into a complexPhotoData data class.
        //Try and keep this updated if new fields are added to complexPhotoData.
        private ComplexPhotoData util_convertPhotoElemToComplexStruct(ErrorReport errorReport, XElement elem)
        {
            ComplexPhotoData data = new ComplexPhotoData();

            //TRANSFER ALL DATA TO THE DATA CLASS HERE.
            try
            {
                data.UID = (int)elem.Attribute("UID");
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
                (from c in _albumsDatabase.Element("database").Elements("album")
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

        //--------------------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //changes the photo name in the given xelement.
        private void util_changePhotoNameInAlbumPhotoElem(ErrorReport error, XElement simplePhotoElem, String newName)
        {
            try
            {
                simplePhotoElem.Element("name").Value = newName;
            }
            catch
            {
                error.reportID = ErrorReport.FAILURE;
                error.description = "Failed to change the name of a photo.";
            }
        }
        //--------------------------------------------------------------------------

        //By: Ryan Moe
        //Edited Last:
        //returns the xelement that represents the requested photo,
        //gets this xelement from the parameter xelement that represents
        //the album that the picture might be in.
        private XElement util_getPhotoFromAlbumElemByUID(ErrorReport error, XElement albumElem, int photoUID)
        {
            try
            {
                //find and return a picture whos uid is the one we are looking for.
                //Throws exception if none or more than one match is found.
                return (from c in albumElem.Element("albumPhotos").Elements("picture")
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

        //--------------------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //returns an xelement representing an album, gets it
        //from the album database in memory.
        private XElement util_getAlbumByUID(ErrorReport error, int albumUID)
        {
            try
            {
                //find and return an album whos uid is the one we are looking for.
                //Throws exception if none or more than one match is found.
                return (from c in _albumsDatabase.Element("database").Elements("album")
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
        private Boolean util_checkPicturesDatabase(ErrorReport errorReport)
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
