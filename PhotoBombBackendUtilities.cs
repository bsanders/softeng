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
        //call this before using the albums database,
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

        private XElement getPictureElementByUID(ErrorReport error, int uid)
        {
            if (checkDatabaseIntegrity(_picturesXDocs, error))
            {
                //Try searching for the album with the uid specified.
                XElement specificPicture;
                try
                {
                    //for(from) every c in the database's children (all albums),
                    //see if it's attribute uid is the one we want,
                    //and if so return the first instance of a match.
                    specificPicture = (from c in _picturesXDocs.Element("database").Elements()
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




    }//class
}
