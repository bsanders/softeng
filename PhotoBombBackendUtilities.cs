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
        private ErrorReport addPictureToPictureDatabase(ErrorReport errorReport, ComplexPhotoData newPictureData)
        {
            //if uid is not valid
            if (!checkUID(newPictureData.UID))
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "UID is not valid.";
                return errorReport;
            }

            //if uid is not valid
            if (!checkPictureExtension(newPictureData.extension))
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "Extension is not valid.";
                return errorReport;
            }

            //if uid is not valid
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
        private void addPictureToAlbumDatabase(ComplexPhotoData newPicture, int albumUID)
        {

        }

        //--------------------------------------------------------
        //Check UID's here.
        //RETURN: true if the uid is valid, false otherwise.
        private Boolean checkUID(int uid)
        {
            if (uid > 0 && uid < 9999999)
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
            return true;
        }


        //--------------------------------------------------------

        private int getNewUID()
        {
            //HEY MAKE LOGIC TO GET A NEW UID!!!
            return -1;
        }

    }//class
}
