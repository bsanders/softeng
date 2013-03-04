/**
 * This is the main program for the PhotoBomb backend.
 * These are the function calls for the GUI to call.
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;

namespace SoftwareEng
{
    //This is a PARTIAL class,
    //it is the public part of the PhotoBomb class.
    public partial class PhotoBomb
    {
        //----------------------------------------------

        //By: Ryan Moe
        //Edited Last: 
        //
        //initialize the backend object AND load the databases.
        //DOES NOT initialize the databases.
        //PARAM 1 = the callback for the results of loading the databses.
        //PARAM 2 = the path to the album database (xml).
        //PARAM 3 = path to the pictures database.
        //PARAM 4 = the path to the folder where all the pictures
        //          tracked by this program are stored.
        //ERROR CONDITIONS
        //Fails if one of the database xml files is not found or if 
        //the library folder does not exist.
        public PhotoBomb(generic_callback guiCallback, string albumDatabasePathIn, string pictureDatabasePathIn, string libraryPath)
        {
            init(guiCallback, albumDatabasePathIn, pictureDatabasePathIn, libraryPath);
        }

        //----------------------------------------------
        //By: Ryan Moe
        //Edited Last: 
        //
        //CAREFULL!!!  This will blow out the databases and make a new library folder.
        public void rebuildBackendOnFilesystem(generic_callback guiCallback)
        {
            rebuildBackendOnFilesystem_backend(guiCallback);
        }

        //----------------------------------------------
        //By: Ryan Moe
        //Edited Last: 
        //
        //reopen the xml document (database) that represents the
        //user's albums in the program.
        //PARAM 1 = a callback (delegate) to a gui function (see PhotoBombDelegates.cs).
        //
        //ERROR CONDITIONS
        //1) if the xml file does not exist, an error will be returned.
        //2) if the xml file does not contain VALID xml, error.
        public void reopenAlbumsXML(generic_callback guiCallback)
        {
            reopenAlbumsXML_backend(guiCallback);
        }

        //----------------------------------------------
        //By: Ryan Moe
        //Edited Last: 
        //
        //Save the album data to an xml file.
        //PARAM 1 = a gui callback (see PhotoBombDelegates.cs).
        public void saveAlbumsXML(generic_callback guiCallback)
        {
            saveAlbumsXML_backend(guiCallback);
        }
        //----------------------------------------------
        //By: Ryan Moe
        //Edited Last: 
        //
        //Open the pictures database (xml file).
        //PARAM 1 = a gui callback (see PhotoBombDelegates.cs).
        public void reopenPicturesXML(generic_callback guiCallback)
        {
            reopenPicturesXML_backend(guiCallback);
        }

        //-----------------------------------------------
        //By: Ryan Moe
        //Edited Last: 
        //
        //Save the pictures database (xml file).
        //PARAM 1 = a gui callback (see PhotoBombDelegates.cs).
        public void savePicturesXML(generic_callback guiCallback)
        {
            savePicturesXML_backend(guiCallback);
        }

        //-----------------------------------------------

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

        //----------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //
        //Adds a picture to the picture database AND the album database.
        public void addNewPicture(generic_callback guiCallback, String photoUserPath, String photoExtension, int albumUID, String pictureNameInAlbum)
        {
            addNewPicture_backend(guiCallback, photoUserPath, photoExtension, albumUID, pictureNameInAlbum);
        }


        //----------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //
        public void addNewAlbum(generic_callback guiCallback, SimpleAlbumData albumData)
        {
            addNewAlbum_backend(guiCallback, albumData);
        }

        //----------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //
        public void addExistingPictureToAlbum(generic_callback guiCallback, int pictureUID, int albumUID, String SimplePhotoData)
        {
            addExistingPictureToAlbum_backend(guiCallback, pictureUID, albumUID, SimplePhotoData);
        }

        //---------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //
        public void checkIfAlbumNameIsUnique(generic_callback guiCallback, String albumName)
        {
            checkIfAlbumNameIsUnique_backend(guiCallback, albumName);
        }

        //---------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //
        public void changePhotoNameByUID(generic_callback guiCallback, int albumUID, int photoUID, String newName)
        {
            changePhotoNameByUID_backend(guiCallback, albumUID, photoUID, newName);
        }

        //------------------------------------------------
        //
        public void addNewPictures(generic_callback guiCallback, List<String> photoUserPath, List<String> photoExtension, int albumUID, List<String> pictureNameInAlbum, ProgressChangedEventHandler updateCallback, int updateAmount)
        {
            addNewPictures_backend(guiCallback, photoUserPath, photoExtension, albumUID, pictureNameInAlbum, updateCallback, updateAmount);
        }

        //--------------------------------------------------

        public ErrorReport cancelAddNewPicturesThread()
        {
            return cancelAddNewPicturesThread_backend();
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
        public String picturesNameInAlbum;
        public int UID;

        public SimplePhotoData()
        {
            picturesNameInAlbum = "";
            UID = -1;
            //path = "";
        }
    }

    //--------------------------------
    //More complex photo data returned by functions like getPhotoDataByUID().
    //By: Ryan Moe
    //Edited Last:
    public class ComplexPhotoData
    {
        //the name of the picture in the album, displayed by the gui
        public int UID;
        public String path;
        public String extension;
        //... add more stuff here when we have more metadata

        public ComplexPhotoData()
        {
            UID = -1;
            path = "";
            extension = "";
        }
    }










}//namespace
