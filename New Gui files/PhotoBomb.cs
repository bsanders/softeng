﻿/**
 * This is the main program for the PhotoBomb backend.
 * These are the function calls for the GUI to call.
 * 
 * Keep these functions short to keep it easier on the gui builder.
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;

/*
 * PhotoBomb TODO:
 *  implement album renaming
 *  implement reference counting for photos in library
 *  Switch pictures in picdb away from UID to hash?
 *    and then pics in albdb get an albumID?
 * 
 */

namespace SoftwareEng
{
    //This is a PARTIAL class,
    //it is the public part of the PhotoBomb class.
    public partial class PhotoBomb
    {
        //----------------------------------------------

        //By: Ryan Moe
        //Edited Last: 
        public PhotoBomb() { }

        //----------------------------------------------
        //By: Ryan Moe
        //Edited Last: 
        //
        //initialize the backend object AND load the databases.
        //DOES NOT re-initialize the databases if there is a problem.
        //PARAM 1 = the callback for the results of loading the databses.
        //PARAM 2 = the path to the album database (xml).
        //PARAM 3 = path to the pictures database.
        //PARAM 4 = the path to the folder where all the pictures
        //          tracked by this program are stored.
        //ERROR CONDITIONS
        //Fails if one of the database xml files is not found or if 
        //the library folder does not exist.
        public void init(generic_callback guiCallback, string albumDatabasePathIn, string pictureDatabasePathIn, string libraryPath)
        {
            init_backend(guiCallback, albumDatabasePathIn, pictureDatabasePathIn, libraryPath);
        }

        //----------------------------------------------
        //By: Ryan Moe
        //Edited Last: 
        //
        //CAREFULL!!!  This will blow out the databases and make a new library folder.
        //Before making the new library folder, if an old folder was found, that old folder
        //will be renamed to libraryName_backup.
        //ERROR CONDITIONS
        //This returns an error if it fails to create one of the database files or
        //the folder.  If libraryName_backup exists it also returns an error.

        public void rebuildBackendOnFilesystem(generic_callback guiCallback)
        {
            rebuildBackendOnFilesystem_backend(guiCallback);
        }

        //BS: Commenting this function out: its never used 03/23/13
        //----------------------------------------------
        //By: Ryan Moe
        //Edited Last: 
        //
        //reopen the xml document (database) that represents the
        //user's albums in the program.
        //PARAM 1 = a callback (delegate) to a gui function (see PhotoBombDelegates.cs).
        //ERROR CONDITIONS
        //1) if the xml file does not exist, an error will be returned.
        //2) if the xml file does not contain VALID xml, error.
        //public void reopenAlbumsXML(generic_callback guiCallback)
        //{
        //    reopenAlbumsXML_backend(guiCallback);
        //}

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

        //BS: Commenting this function out: its never used 03/23/13
        //----------------------------------------------
        //By: Ryan Moe
        //Edited Last: 
        //
        //Open the pictures database (xml file).
        //PARAM 1 = a gui callback (see PhotoBombDelegates.cs).
        //ERROR CONDITIONS
        //1) if the xml file does not exist, an error will be returned.
        //2) if the xml file does not contain VALID xml, error.
        //public void reopenPicturesXML(generic_callback guiCallback)
        //{
        //    reopenPicturesXML_backend(guiCallback);
        //}

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
        public void getAllAlbums(getAllAlbumNames_callback guiCallback)
        {
            getAllAlbums_backend(guiCallback);
        }

        //----------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //
        //This method will return ALL of the pictures 
        //that are within a single album to the callback.
        //PARAM 1 = the gui callback.
        //PARAM 2 = the UID of the album to get all the photo names from.
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
        public void getPhoto(getPhotoByUID_callback guiCallback, int photoUID, int albumUID)
        {
            getPhoto_backend(guiCallback, photoUID, albumUID);
        }

        // BS (4/1/13) Commenting this out: we never add a single picture at a time!
        //----------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //
        //Adds a picture to the picture database AND the album database.
        //Also this makes a copy of the photo in the Library.
        //PARAM 1 = gui callback.
        //PARAM 2 = the path to the photo that is being imported.
        //PARAM 3 = extension of the photo (ex: ".jpg")
        //PARAM 4 = uid of the album that the photo is being added to.
        //PARAM 5 = the name of the photo in the album.  NOTE: you can 
        //          send in "" and the backend will give the photo a default name.
        //public void addNewPicture(generic_callback guiCallback, String photoUserPath, String photoExtension, int albumUID, String pictureNameInAlbum)
        //{
        //    addNewPicture_backend(guiCallback, photoUserPath, photoExtension, albumUID, pictureNameInAlbum);
        //}

        //----------------------------------------------
        //By: Bill Sanders
        //Edited Last: 3/26/13
        /// <summary>
        /// This function removes the specified photo from the specified album.
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="uid">The UID of the photo</param>
        /// <param name="albumUID">The UID of the album</param>
        public void removePictureFromAlbum(generic_callback guiCallback, int uid, int albumUID)
        {
            removePictureFromAlbum_backend(guiCallback, uid, albumUID);
        }

        //By: Bill Sanders
        //Edited Last: 3/28/13
        /// <summary>
        /// This function removes the specified album.
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="albumUID">The UID of the album</param>
        public void removeAlbum(generic_callback guiCallback, int albumUID)
        {
            removeAlbum_backend(guiCallback, albumUID);
        }

        //----------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //
        //Adds a new album to the album database.
        //PARAM 2 = a data class that you need to fill out with the new album's info.
        //          NOTE: you don't need to worry about the UID, that gets set in here.
        public void addNewAlbum(generic_callback guiCallback, SimpleAlbumData albumData)
        {
            addNewAlbum_backend(guiCallback, albumData);
        }

        //----------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //
        //Adds a single existing picture (it exists in the picture database already) to
        //an album.
        //PARAM 4 = data class for you to fill out.
        //UNTESTED/UNFINISHED.
        public void addExistingPictureToAlbum(generic_callback guiCallback, int pictureUID, int albumUID, String SimplePhotoData)
        {
            addExistingPictureToAlbum_backend(guiCallback, pictureUID, albumUID, SimplePhotoData);
        }

        //---------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //
        //Checks to see if an album name is unique.
        //This will return FAILED in the error report (in the callback) 
        //if the name is not unique.
        public void checkIfAlbumNameIsUnique(generic_callback guiCallback, String albumName)
        {
            checkIfAlbumNameIsUnique_backend(guiCallback, albumName);
        }

        //---------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //
        //Change the name of a photo (its name in a single album) in the
        //database and save the change to disk.
        public void changePhotoNameByUID(generic_callback guiCallback, int albumUID, int photoUID, String newName)
        {
            changePhotoNameByUID_backend(guiCallback, albumUID, photoUID, newName);
        }

        //------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //
        //THIS IS THREADED, call this instead of multiple calls to addnewPicture to prevent gui lockup.
        //Adds multiple new photos to the databases and moves a copy of the picture to the library.
        //Also writes all these changes to the disk.
        //PARAM 2 = List of photo paths on the disk.
        //PARAM 3 = List of photo extensions (ex: ".jpg").
        //PARAM 4 = the SINGLE uid of the album to add ALL photos to.
        //PARAM 5 = the list of names for the photos in the album.
        //          NOTE: you can pass in NULL for the list for all default names,
        //                or you can have "" for a single element for a single default name.
        //PARAM 6 = a callback for the thread to send progress updates to.
        //PARAM 7 = the number of pictures to add BEFORE sending a progress update.
        public void addNewPictures(generic_callback guiCallback, List<String> photoUserPath, List<String> photoExtension, int albumUID, List<String> pictureNameInAlbum, ProgressChangedEventHandler updateCallback, int updateAmount)
        {
            addNewPictures_backend(guiCallback, photoUserPath, photoExtension, albumUID, pictureNameInAlbum, updateCallback, updateAmount);
        }

        //--------------------------------------------------
        //By: Ryan Moe
        //Edited Last:
        //
        //This will cancel the thread from addNewPictures() if it exists.
        //Returns the error report directly.
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
    //Last Edited By: Ryan Causey
    //Last Edited Date: 3/31/13
    /*
     * Changelog:
     * 3/31/31 Ryan Causey: converted this classes public datamembers into properties to facilitate databinding
     */
    public class SimpleAlbumData
    {
        private String albumNameValue;
        private int UIDValue;
        //add more information here if needed...

        public string albumName
        {
            set
            {
                if (value != this.albumNameValue)
                {
                    albumNameValue = value;
                }
            }
            get
            {
                return this.albumNameValue;
            }
        }

        public int UID
        {
            set
            {
                if (value != this.UIDValue)
                {
                    UIDValue = value;
                }
            }
            get
            {
                return this.UIDValue;
            }
        }

        //initialize vars.
        public SimpleAlbumData()
        {
            albumNameValue = "";
            UIDValue = -1;//indicates UID not set.
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
    //Edited Last: Bill Sanders, added a fields for hash, caption, ref count
    public class ComplexPhotoData
    {
        //the name of the picture in the album, displayed by the gui
        public int UID;
        public byte[] hash;
        public String path;
        public String extension;
        public String caption;
        public int refCount;
        //... add more stuff here when we have more metadata

        public ComplexPhotoData()
        {
            UID = -1;
            hash = null;
            path = "";
            extension = "";
            caption = "";
            refCount = 0;
        }

        // Add a toXML function here?
    }

}//namespace
