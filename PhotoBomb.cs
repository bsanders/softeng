/**
 * This is the main program for the PhotoBomb backend.
 * These are the function calls for the GUI to call.
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SoftwareEng
{
    //This is a PARTIAL class,
    //it is the public part of the PhotoBomb class.
    partial class PhotoBomb
    {

        //----------------------------------------------
        //By: Ryan Moe
        //Edited Last: 
        //
        //initialize.
        public PhotoBomb()
        {
            _albumsXDocs = null;
            _picturesXDocs = null;

            xmlParser = new XmlParser();
        }

        //----------------------------------------------
        //By: Ryan Moe
        //Edited Last: 
        //
        //open the xml document that represents the
        //user's albums in the program.
        //PARAM 1 = a callback (delegate) to a gui function (see PhotoBombDelegates.cs).
        //PARAM 2 = The path to the album xml, THIS MAY BE REMOVED.
        //
        //ERROR CONDITIONS
        //1) if the xml file does not exist, an error will be returned.
        //2) if the xml file does not contain VALID xml, error.
        public void openAlbumsXML(generic_callback guiCallback, string xmlPath)
        {
            //use this to inform the calling gui of how things went.
            ErrorReport error = new ErrorReport();

            try
            {
                _albumsXDocs = XDocument.Load(xmlPath);
            }
            catch
            {
                error.reportID = ErrorReport.SHIT_JUST_GOT_REAL;
                error.description = "PhotoBomb.openAlbumsXML():failed to load the albums xml file: " + xmlPath;
                guiCallback(error);
                return;
            }

            //The loading of the xml was nominal, report back to the gui callback.
            error.description = "great success!";
            guiCallback(error);
        }

        //----------------------------------------------
        //By: Ryan Moe
        //Edited Last: 
        //
        //Save the album data to an xml file.
        //PARAM 1 = a gui callback (see PhotoBombDelegates.cs).
        //PARAM 2 = path to where you want to save the xml.
        public void saveAlbumsXML(generic_callback guiCallback, string xmlSavePath)
        {
            ErrorReport error = new ErrorReport();
            //error checking
            if (_albumsXDocs == null)
            {
                error.reportID = ErrorReport.FAILURE;
                error.description = "PhotoBomb:saveAlbumsXML:No Album loaded to save.";
                guiCallback(error);
                return;
            }


            guiCallback(error);
        }
        //----------------------------------------------
        //By: Ryan Moe
        //Edited Last: 
        //
        //This method returns a list of Album objects to the
        //callback given by the parameter.
        //PARAM 1 = a gui callback (see PhotoBombDelegates.cs).
        public void getAllUserAlbumNames(getAllUserAlbumNames_callback guiCallback)
        {
            getAllUserAlbumNames_backend(guiCallback);
        }


    }//class




    //-------------------------------------------------



    //BY: Ryan Moe
    //This is a data class that will be used
    //to send a calling gui a list of albums.
    //This class is a SINGLE element of that list.
    public class UserAlbum
    {
        public String albumName;
        public int UID;
        //add more information here if needed...

        //initialize vars.
        public UserAlbum()
        {
            albumName = "";
            UID = -1;//indicates UID not set.
        }
    }


}//namespace
/