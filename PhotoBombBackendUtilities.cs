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
        private Boolean checkAlbumsDatabaseIntegrity(XDocument database, ErrorReport errorReport)
        {
            if (database == null)
            {
                errorReport.reportID = ErrorReport.FAILURE;
                errorReport.description = "PhotoBomb: Albums xml has not been loaded yet!";
                return false;
            }
            //put more things to check here.

            return true;
        }

        //----------------------------------------------------------




    }//class
}
