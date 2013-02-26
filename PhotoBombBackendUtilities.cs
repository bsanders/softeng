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

namespace SoftwareEng
{
    partial class PhotoBomb
    {
        //----------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //call this before reading from the albums database,
        //this will check for integrity problems.
        //RETURNS: true = good to go, false = the database is bad!
        //ALSO: this will append warnings/errors to the errorReport Parameter.
        private Boolean checkDatabaseIntegrity(XDocument database, ErrorReport errorReport)
        {
            if (database == null)
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "PhotoBomb: The database has not been loaded yet!";
                return false;
            }
            //put more things to check here.

            return true;
        }


        //----------------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        private XElement getPictureElementByUID(ErrorReport error, int uid)
        {
            if (checkDatabaseIntegrity(_picturesDatabase, error))
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
        private ErrorReport addPictureToPictureDatabase(ErrorReport errorReport, ComplexPhotoData newPictureData)
        {

            //if picture extension is not valid
            if (!checkPictureExtension(newPictureData.extension))
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "Extension is not valid.";
                return errorReport;
            }

            //if path is not valid
            if (!checkPicturePath(newPictureData.path))
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
        private void addPictureToAlbumDatabase(ErrorReport errorReport, ComplexPhotoData newPicture, int albumUID)
        {
            //Error checking goes here!!!

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

            
            /*
           //make the object that will go into the xml database.
            XElement newPicRoot = new XElement("picture",
                new XAttribute("uid", newUID),
                new XElement("filePath", new XAttribute("extension", newPictureData.extension), newPictureData.path)
                );
             */

            //construct the object we will be adding to the album.
            XElement newPhotoElem = new XElement("picture",
                                            new XAttribute("uid", newPicture.UID),
                                            new XElement("name", newPicture.picturesAlbumName));


            specificAlbum.Element("albumPhotos").Add(newPhotoElem);

        }

        //--------------------------------------------------------
        //Check UID's here.
        //RETURN: true if the uid is valid, false otherwise.
        private Boolean checkUID(int uid)
        {
            if (uid > 0 && uid < 999999)
                return true;
            return false;
        }
        //--------------------------------------------------------

        private Boolean checkPictureExtension(String extension)
        {
            if (extension.Equals(".jpg") || extension.Equals(".png"))
            {
                return true;
            }
            return false;
        }

        //--------------------------------------------------------

        private Boolean checkPicturePath(String path)
        {
            if(path != "")
                return true;
            return false;
        }


        //--------------------------------------------------------

        private int getNewPictureUID_slow()
        {
            int newUID = 1;
            Boolean uidNotFound = true;
            while (uidNotFound && newUID < 999999)
            {
                try
                {
                    (from c in _picturesDatabase.Element("database").Elements("picture")
                     where (int)c.Attribute("uid") == newUID
                     select c).First();//NOTE: this will throw an exception if no elements' id matches the one we have.
                    ++newUID;
                }
                //we found one!
                catch
                {
                    uidNotFound = false;
                }
            }//while

            if(newUID != 999999)
                return newUID;
            return -1;
        }

        //--------------------------------------------------------

        private void openAlbumsXML(ErrorReport error)
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

        private void openPicturesXML(ErrorReport error)
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

        private int getNewAlbumUID_slow(ErrorReport error)
        {
            int newUID = 1;
            Boolean uidNotFound = true;
            while (uidNotFound && newUID < 999999)
            {
                try
                {
                    (from c in _albumsDatabase.Element("database").Elements("album")
                     where (int)c.Attribute("uid") == newUID
                     select c).First();//NOTE: this will throw an exception if no elements' id matches the one we have.
                    ++newUID;
                }
                //we found one!
                catch
                {
                    uidNotFound = false;
                }
            }//while

            if(newUID != 999999)
                return newUID;
            return -1;
        }

        //-------------------------------------------------------------------

        private void addAlbumToAlbumDatabase(ErrorReport errorReport, SimpleAlbumData albumData){
            //Error checking goes here!!!

            //construct the object we will be adding to the database.
            XElement newAlbum = new XElement("album",
                                            new XAttribute("uid", albumData.UID),
                                            new XElement("albumName", albumData.albumName),
                                            new XElement("albumPhotos"));

            _albumsDatabase.Element("database").Add(newAlbum);
        }//method








    }//class
}
