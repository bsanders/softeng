﻿/**
 * This is the main program for the PhotoBomb backend.
 * These are the function calls for the GUI to call.
 * 
 * Keep these functions short to keep it easier on the gui builder.
 * 
 ****************************************************************************************************************
 * Changelog:
 * 3/29/13 Bill Sanders: added a fields for hash, caption, ref count
 * 3/31/13 Ryan Causey: converted SimpleAlbumData's public datamembers into properties to facilitate databinding
 * 4/1/13 Ryan Causey: converting the rest of the data class's public datamember into properties
 *                     Implementing the INotifyPropertyChanged interface for all data classes
 * 4/5/13 Ryan Causey: Adding small, med, and large thumbnail paths to ComplexPhotoData.
 * 4/6/13 Ryan Causey: Fixed a bug where we would give the wrong picture UID to the remove picture from album
 *                     function.
 ***************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;
using System.IO;

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
        // A handy shortcut to the settings class...
        Properties.Settings Settings = Properties.Settings.Default;
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

        //----------------------------------------------
        //By: Bill Sanders
        /// <summary>
        /// Sends a copy of all the photos in the specified album to the clipboard
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="albumUID">The unique ID of the album</param>
        public void sendSelectedPhotosToClipboard(sendAllPhotosInAlbum_callback guiCallback, int albumUID)
        {
            sendSelectedPhotosToClipboard_backend(guiCallback, albumUID);
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
        //Edited Last By: Ryan Causey
        //Edited Last Date: 4/6/13
        /// <summary>
        /// This function removes the specified photo from the specified album.
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="uid">The idInAlbum of the photo</param>
        /// <param name="albumUID">The UID of the album</param>
        public void removePictureFromAlbum(generic_callback guiCallback, int idInAlbum, int albumUID)
        {
            removePictureFromAlbum_backend(guiCallback, idInAlbum, albumUID);
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

        //By: Bill Sanders
        //Edited Last: 4/6/13
        /// <summary>
        /// This function renames the specified album.
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="albumUID">The UID of the album</param>
        /// <param name="newName">The new name of the album</param>
        public void renameAlbum(generic_callback guiCallback, int albumUID, string newName)
        {
            renameAlbum_backend(guiCallback, albumUID, newName);
        }

        //By: Bill Sanders
        //Edited Last: 4/6/13
        /// <summary>
        /// This function renames the specified image in this album.
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="albumUID">The UID of the album the photo is in</param>
        /// <param name="idInAlbum">The id of the photo in this album</param>
        /// <param name="newName">The new name of the photo</param>
        public void renamePhoto(generic_callback guiCallback, int albumUID, int idInAlbum, string newName)
        {
            renamePhoto_backend(guiCallback, albumUID, idInAlbum, newName);
        }


        //By: Bill Sanders
        //Edited Last: 4/6/13
        /// <summary>
        /// This function sets the specified caption on the specified photo in an album.
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="albumUID">The UID of the album the photo is in</param>
        /// <param name="idInAlbum">The id of the photo in this album</param>
        /// <param name="newName">The new name of the photo</param>
        public void setPhotoCaption(generic_callback guiCallback, int albumUID, int idInAlbum, string newName)
        {
            setPhotoCaption_backend(guiCallback, albumUID, idInAlbum, newName);
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

        //-------------------------------------------------------------
        //By: Bill Sanders
        //Edited Last By: 
        //Edited Last Date: 4/7/13
        /// <summary>
        /// Adds a photo that already exists in one album to another album
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="photoList">A ComplexPhotoData object which contains all the information about a photo</param>
        /// <param name="albumUID">The unique ID of the album to copy the photo into</param>
        public void addExistingPhotosToAlbum(addNewPictures_callback guiCallback, List<ComplexPhotoData> photoList, int albumUID)
        {
            addExistingPhotosToAlbum_backend(guiCallback, photoList, albumUID);
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
        //By: Bill Sanders
        //Edited Last:
        //
        //Checks to see if a photo name is unique.
        //This will return FAILED in the error report (in the callback) 
        //if the name is not unique.
        public void checkIfPhotoNameIsUnique(generic_callback guiCallback, String photoName, int albumUID)
        {
            checkIfPhotoNameIsUnique_backend(guiCallback, photoName, albumUID);
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
        public void addNewPictures(addNewPictures_callback guiCallback, List<String> photoUserPath, List<String> photoExtension, int albumUID, List<String> pictureNameInAlbum, ProgressChangedEventHandler updateCallback, int updateAmount)
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
    public class SimpleAlbumData : INotifyPropertyChanged
    {
        private String _albumName;
        private int _UID;
        private string _thumbnailPath;
        //add more information here if needed...
        //event for changing a property
        public event PropertyChangedEventHandler PropertyChanged;

        public String albumName
        {
            set
            {
                if (value != _albumName)
                {
                    _albumName = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("albumName");
                }
            }
            get
            {
                return _albumName;
            }
        }

        public int UID
        {
            set
            {
                if (value != _UID)
                {
                    _UID = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("UID");
                }
            }
            get
            {
                return _UID;
            }
        }

        public string thumbnailPath
        {
            get
            {
                return _thumbnailPath;
            }
            set
            {
                if (value != _thumbnailPath)
                {
                    _thumbnailPath = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("thumbnailPath");
                }
            }
        }

        //initialize vars.
        public SimpleAlbumData()
        {
            _albumName = "";
            _UID = -1;//indicates UID not set.
            _thumbnailPath = "";
        }

        /*
         * Call this function when any property is set as part of implementing INotifyPropertyChanged
         * @Param: name is the name of the property, E.G. changing UID would mean name = "UID"
         */
        protected void OnPropertyChanged(String name)
        {
            PropertyChangedEventHandler changedHandler = PropertyChanged;

            if (changedHandler != null)
            {
                changedHandler(this, new PropertyChangedEventArgs(name));
            }
        }
    }


    //-----------------------------------
    //Simple photo data returned by functions like getAllPhotosInAlbum().
    //By: Ryan Moe
    //Edited Last By: Bill Sanders (4/4/13), dropped member variables entirely, renamed UID -> idInAlbum
    //Edited Last By: Ryan Causey (4/1/13)
    public class SimplePhotoData : INotifyPropertyChanged
    {
        //event for changing a property
        private string _Name;
        private int _idInAlbum;
        public event PropertyChangedEventHandler PropertyChanged;

        public String Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (value != _Name)
                {
                    _Name = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("Name");
                }
            }
        }

        public int idInAlbum
        {
            get
            {
                return _idInAlbum;
            }
            set
            {
                if (value != _idInAlbum)
                {
                    _idInAlbum = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("idInAlbum");
                }
            }
        }

        public SimplePhotoData()
        {
            idInAlbum = -1;
            Name = "";
        }

        /*
         * Call this function when any property is set as part of implementing INotifyPropertyChanged
         * @Param: name is the name of the property, E.G. changing UID would mean name = "UID"
         */
        protected void OnPropertyChanged(String name)
        {
            PropertyChangedEventHandler changedHandler = PropertyChanged;

            if (changedHandler != null)
            {
                changedHandler(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    //--------------------------------
    //More complex photo data returned by functions like getPhotoDataByUID().
    //By: Ryan Moe
    //Edited Last By: Ryan Causey
    //Edited Date: 4/5/13
    public class ComplexPhotoData : INotifyPropertyChanged
    {
        //the name of the picture in the album, displayed by the gui

        private int _UID;
        private int _idInAlbum;
        private byte[] _hash;
        private String _fullPath;
        private String _lgThumbPath;
        private String _extension;
        private String _caption;
        private String _name;
        private int _refCount;

        Properties.Settings Settings = Properties.Settings.Default;

        //event for changing a property
        public event PropertyChangedEventHandler PropertyChanged;
        //... add more stuff here when we have more metadata

        //
        // Public properties of this class.
        //



        public int UID
        {
            get
            {
                return _UID;
            }
            set
            {
                if (value != _UID)
                {
                    _UID = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("UID");
                }
            }
        }

        public int idInAlbum
        {
            get
            {
                return _idInAlbum;
            }
            set
            {
                if (value != _idInAlbum)
                {
                    _idInAlbum = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("idInAlbum");
                }
            }
        }

        public byte[] hash
        {
            get
            {
                return _hash;
            }
            set
            {
                if (value != _hash)
                {
                    _hash = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("hash");
                }
            }
        }

        public String fullPath
        {
            get
            {
                return _fullPath;
            }
            set
            {
                if (value != _fullPath)
                {
                    _fullPath = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("fullPath");
                }
            }
        }

        public String lgThumbPath
        {
            get
            {
                if (!File.Exists(_lgThumbPath))
                {
                    _lgThumbPath = regenerateThumbnail(fullPath, Path.GetFileName(fullPath), Settings.lrgThumbSize);
                }
                return _lgThumbPath;
            }
            set
            {
                if (value != _lgThumbPath)
                {
                    _lgThumbPath = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("lgThumbPath");
                }
            }
        }

        public String extension
        {
            get
            {
                return _extension;
            }
            set
            {
                if (value != _extension)
                {
                    _extension = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("extension");
                }
            }
        }

        public string caption
        {
            get
            {
                return _caption;
            }
            set
            {
                if (value != _caption)
                {
                    _caption = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("caption");
                }
            }
        }

        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("name");
                }
            }
        }

        public int refCount
        {
            get
            {
                return _refCount;
            }
            set
            {
                if (value != _refCount)
                {
                    _refCount = value;
                }
            }
        }

        public ComplexPhotoData()
        {
            UID = -1;
            idInAlbum = -1;
            hash = null;
            fullPath = "";
            extension = "";
            caption = "";
            name = "";
            refCount = 0;
        }

        private string regenerateThumbnail(string source, string filename, int size)
        {
            Imazen.LightResize.ResizeJob resizeJob = new Imazen.LightResize.ResizeJob();
            string thumbSubDir = "";
            string fullThumbPath = "";

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
            fullThumbPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Settings.OrgName,
                Settings.PhotoLibraryName,
                Settings.PhotoLibraryThumbsDir,
                thumbSubDir,
                filename);

            // Actually processes the image, copying it to the new location, should go in a try/catch for IO
            // One of Build's overloads allows you to use file streams instead of filepaths.
            // If images have to be resized on-the-fly instead of stored, that may work as well.
            try
            {
                resizeJob.Build(
                    source,
                    fullThumbPath,
                    Imazen.LightResize.JobOptions.CreateParentDirectory
                );
            }
            catch (IOException)
            {
                return "";
            }

            return fullThumbPath;
        }

        /*
         * Call this function when any property is set as part of implementing INotifyPropertyChanged
         * @Param: name is the name of the property, E.G. changing UID would mean name = "UID"
         */
        protected void OnPropertyChanged(String name)
        {
            PropertyChangedEventHandler changedHandler = PropertyChanged;

            if (changedHandler != null)
            {
                changedHandler(this, new PropertyChangedEventArgs(name));
            }
        }

        // Add a toXML function here?
    }

}//namespace
