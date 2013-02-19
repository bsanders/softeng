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
            openAlbumsXML_backend(guiCallback, xmlPath);
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
            saveAlbumsXML_backend(guiCallback, xmlSavePath);
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

        //----------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //
        //This method will return ALL of the pictures 
        //that are within a single album.
        public void getAllPhotosInAlbum(getAllPhotosInAlbum_callback guiCallback, int albumUID)
        {
            getAllPhotosInAlbum_backend(guiCallback, albumUID);
        }


        //---------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //
        //This method will return a complex photo data object
        //filled out with the data of one photo referenced by the uid param.
        //NOT TESTED YET.
        public void getPictureByUID(getPhotoByUID_callback guiCallback, int uid)
        {
            getPictureByUID_backend(guiCallback, uid);
        }

    }//class







    //-------------------------------------------------
    //DATA CLASSES-------------------------------------
    //-------------------------------------------------



    //BY: Ryan Moe
    //This is a data class that will be used
    //to send a calling gui a list of albums.
    //This class is a SINGLE element of that list.
    public class SimpleAlbumData
    {
        public String albumName;
        public int UID;
        //add more information here if needed...

        //initialize vars.
        public SimpleAlbumData()
        {
            albumName = "";
            UID = -1;//indicates UID not set.
        }
    }


    //-----------------------------------
    //Simple photo data returned by functions like getAllPhotosInAlbum().
    //By: Ryan Moe
    //Edited Last:
    public class SimplePhotoData
    {
        public String pictureName;
        public int UID;
        public String path;

        public SimplePhotoData()
        {
            pictureName = "";
            UID = -1;
            path = "";
        }
    }

    //--------------------------------
    //More complex photo data returned by functions like getPhotoDataByUID().
    //By: Ryan Moe
    //Edited Last:
    public class ComplexPhotoData
    {
        public String pictureName;
        public int UID;
        public String path;
        //... add more stuff here when we have more metadata

        public ComplexPhotoData()
        {
            pictureName = "";
            UID = -1;
            path = "";

        }
    }


}//namespace
