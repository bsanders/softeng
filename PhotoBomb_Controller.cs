/**
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
 * Julian Nguyen (4/28/13)
 * Change this class to PhotoBomb -> PhotoBomb_Controller
 * 
 * 
 ***************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    /// By Julian Nguyen
    /// Edited: Julian Ngugen(4/28/13)
    /// <summary>
    /// 
    /// </summary>
    public class PhotoBomb_Controller
    {
        private PhotoBomb photoBombDatabase;


        /// By Julian Nguyen
        /// Edited: Julian Ngugen(4/28/13)
        /// <summary>
        /// This will setup the class. 
        /// </summary>
        public PhotoBomb_Controller()
        {
            photoBombDatabase = new PhotoBomb();
        }


        /// By: Ryan Moe
        /// Edited: Julian Nguyen(4/28/13) 
        /// <summary>
        /// initialize the backend object AND load the databases. DOES NOT re-initialize the databases if there is a problem.
        /// Error: Fails if any of the XML files is nissing or if the image folder does not exist.
        /// </summary>
        /// <param name="guiCallback">The callback to the GUI.</param>
        /// <param name="albumDatabasePathIn">The path to the album XML file.</param>
        /// <param name="pictureDatabasePathIn">The path to the image XML file.</param>
        /// <param name="libraryPath">The path to the folder where all the images are stored.</param>
        public void init(generic_callback guiCallback, string albumDatabasePathIn, string pictureDatabasePathIn, string libraryPath)
        {
            ErrorReport errReport = null; 
            errReport =  photoBombDatabase.init_backend( albumDatabasePathIn, pictureDatabasePathIn, libraryPath);


            guiCallback(errReport);
        }

        

        /// By Ryan Moe
        /// Edited: Julian Nguyen(4/28/13)
        /// <summary>
        /// CAREFULL!!!  This will blow out the databases and make a new library folder. 
        /// Before making the new library folder, if an old folder was found, 
        /// that old folder will be renamed to libraryName_backup.
        ///ERROR: This returns an error if it fails to create one of the database files or the folder.  
        ///If libraryName_backup exists it also returns an error.
        /// </summary>
        /// <param name="guiCallback">The callback to the GUI.</param>
        public void rebuildBackendOnFilesystem(generic_callback guiCallback)
        {
            ErrorReport errReport = null;
            errReport =  photoBombDatabase.rebuildBackendOnFilesystem_backend();
            guiCallback(errReport);
        }



        /// By Ryan Moe
        /// Edited: Julian Nguyen(4/28/13)
        /// <summary>
        /// Will save the album XML to file. 
        /// </summary>
        /// <param name="guiCallback">The callback to the GUI.</param>
        public void saveAlbumsXML(generic_callback guiCallback)
        {
            ErrorReport errReport = null;
            errReport =  photoBombDatabase.saveAlbumsXML_backend();
            guiCallback(errReport);
        }



        /// By Ryan Moe
        /// Edited: Julian Nguyen(4/28/13)
        /// <summary>
        /// Will save the Image XML to file.
        /// </summary>
        /// <param name="guiCallback">The call back to the GUI.</param>
        public void savePicturesXML(generic_callback guiCallback)
        {
            ErrorReport errReport = null;
            errReport = photoBombDatabase.saveImagesXML_backend();
            guiCallback(errReport);

        }


        /// By Ryan Moe
        /// Edited: Julian Nguyen(4/27/13)
        /// <summary>
        /// This method returns a list of Album objects to the callback given by the parameter.
        /// </summary>
        /// <param name="guiCallback">The callback for the GUI.</param>
        public void getAllAlbums(getAllAlbumNames_callback guiCallback)
        {
            ErrorReport errReport = null;
            ReadOnlyObservableCollection<SimpleAlbumData> readOnlyAlbumList = null;
            errReport = photoBombDatabase.getAllAlbums_backend(out readOnlyAlbumList);
            guiCallback(errReport, readOnlyAlbumList);
        }




        /// By Ryan Moe
        /// Edited: Julian Nguyen(4/27/13)
        /// <summary>
        /// Will get all the images of an Album. 
        /// </summary>
        /// <param name="guiCallback">The callback for the GUI.</param>
        /// <param name="albumUID">The ID of the Album. </param>
        public void getAllPhotosInAlbum(getAllPhotosInAlbum_callback guiCallback, int albumUID)
        {
            ErrorReport errReport = null;
            ReadOnlyObservableCollection<ComplexPhotoData> imagesOfAnAlbum = null;
            errReport = photoBombDatabase.getAllImagesInAlbum_backend(albumUID, out imagesOfAnAlbum);
            guiCallback(errReport, imagesOfAnAlbum);
        }

        /// By: Bill Sanders
        /// Edited: Julian Nguyen(3/28/13)
        /// <summary>
        /// Sends a copy of all the photos in the specified album to the clipboard
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="albumUID">The unique ID of the album</param>
        public void sendSelectedPhotosToClipboard(sendAllPhotosInAlbum_callback guiCallback, int albumUID)
        {
            ErrorReport errReport = null;
            List<ComplexPhotoData> images = null;
            errReport = photoBombDatabase.sendSelectedPhotosToClipboard_backend(albumUID, out images);
            guiCallback(errReport, images);
        }



        //By: Ryan Moe
        //Edited: Julian Nguyen(4/28/13)
        /// <summary>
        /// This method will return a complex photo data object filled 
        /// out with the data of one photo referenced by the uid param.
        /// </summary>
        /// <param name="guiCallback">The callback to the GUI</param>
        /// <param name="photoUID">The Image's ID in the Album.</param>
        /// <param name="albumUID">The Album's ID.</param>
        public void getPhoto(getPhotoByUID_callback guiCallback, int photoUID, int albumUID)
        {
            ErrorReport errReport = null;
            ComplexPhotoData imageData = null;
            errReport = photoBombDatabase.getImage_backend(photoUID, albumUID, out imageData);
            guiCallback(errReport, imageData);
        }


        /// By: Bill Sanders
        /// Edited Julian Nguyen(4/28/13)
        /// <summary>
        /// This function removes the specified photo from the specified album.
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="uid">The idInAlbum of the photo</param>
        /// <param name="albumUID">The UID of the album</param>
        public void removePictureFromAlbum(generic_callback guiCallback, int idInAlbum, int albumUID)
        {
            ErrorReport errReport = null;
            errReport = photoBombDatabase.removePictureFromAlbum_backend(idInAlbum, albumUID);
            guiCallback(errReport);
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
            ErrorReport errReport = null;
            errReport =  photoBombDatabase.removeAlbum_backend(albumUID);
            guiCallback(errReport);
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
            ErrorReport errReport = null;
            errReport = photoBombDatabase.renameAlbum_backend(albumUID, newName);
            guiCallback(errReport);
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
            ErrorReport errReport = null; 
            errReport = photoBombDatabase.renamePhoto_backend(albumUID, idInAlbum, newName);
            guiCallback(errReport);
        }


        //By: Bill Sanders
        //Edited Last: Julian Nguyen(4/28/13)
        /// <summary>
        /// This function sets the specified caption on the specified photo in an album.
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="albumUID">The UID of the album the photo is in</param>
        /// <param name="idInAlbum">The id of the photo in this album</param>
        /// <param name="newName">The caption to be added to the photo</param>
        public void setImageCaption(generic_callback guiCallback, int albumUID, int idInAlbum, string newCaption)
        {

            ErrorReport errReport = null;
            photoBombDatabase.setPhotoCaption_backend(albumUID, idInAlbum, newCaption);
            guiCallback(errReport);
        }


        /// By: Ryan Moe
        /// Edited Julian Nguyen(4/27/13) 
        /// <summary>
        /// Adds a new album to the album database.
        /// </summary>
        /// <param name="guiCallback">The callback for the GUI.</param>
        /// <param name="albumData">a data class that you need to fill out with the new album's info.
        ///                         NOTE: you don't need to worry about the UID, that gets set in here.</param>
        public void addNewAlbum(generic_callback guiCallback, SimpleAlbumData albumData)
        {
            ErrorReport errReport = null;
            errReport = photoBombDatabase.addNewAlbum_backend(albumData);
            guiCallback(errReport);
        }

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
            ErrorReport errReport = null;
             errReport = photoBombDatabase.addExistingPhotosToAlbum_backend(photoList, albumUID);
             guiCallback(errReport, albumUID);
        }


        /// By Ryan Moe
        /// Edited Julian Nguyen(4/27/13)
        /// <summary>
        /// Checks to see if an album name is unique.
        /// This will return FAILED in the error report (in the callback) 
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="albumName"></param>
        public void checkIfAlbumNameIsUnique(generic_callback guiCallback, String albumName)
        {
            ErrorReport errReport = null;
            bool isUnique = false;
            errReport = photoBombDatabase.checkIfAlbumNameIsUnique_backend(albumName, out isUnique);
            guiCallback(errReport);
        }

        
        //By: Bill Sanders
        //Edited: Julian Nguyen(4/28/13)
        /// <summary>
        /// Checks to see if a photo name is unique.
        /// This will return FAILED in the error report (in the callback) 
        /// if the name is not unique.
        /// </summary>
        /// <param name="guiCallback">The callback to the GUI.</param>
        /// <param name="photoName">The name of the image to be tested.</param>
        /// <param name="albumUID">The ID of the Album that the Image is in.</param>
        public void checkIfPhotoNameIsUnique(generic_callback guiCallback, String photoName, int albumUID)
        {
            ErrorReport errReport = null;
            bool isUnique = false;
            photoBombDatabase.checkIfPhotoNameIsUnique_backend(photoName, albumUID, out isUnique);
            guiCallback(errReport);
        }


        //By: Ryan Moe
        //Edited Last:
        //
        //Change the name of a photo (its name in a single album) in the
        //database and save the change to disk.
        /// <summary>
        /// Will change the name of an Image in an album. 
        /// </summary>
        /// <param name="guiCallback">The callback to the GUI.</param>
        /// <param name="albumUID">The ID of the album.</param>
        /// <param name="photoUID">The ID of Image (In Album ID)</param>
        /// <param name="newName"></param>
        public void changePhotoNameByUID(generic_callback guiCallback, int albumUID, int photoUID, String newName)
        {
            ErrorReport errReport = null;
            errReport =  photoBombDatabase.changePhotoNameByUID_backend( albumUID, photoUID, newName);
            guiCallback(errReport);

        }


        /// By: Ryan Moe
        /// Edited Julian Nguyen
        /// <summary>
        /// THIS IS THREADED, call this instead of multiple calls to addnewPicture to 
        /// prevent gui lockup. Adds multiple new photos to the databases and moves 
        /// a copy of the picture to the library. Also writes all these changes to the disk 
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="photoUserPath">List of photo paths on the disk</param>
        /// <param name="photoExtension">List of photo extensions</param>
        /// <param name="albumUID">The Album UID.</param>
        /// <param name="pictureNameInAlbum">NOTE: you can pass in NULL for the list for all default names, or you can have "" for a single element for a single default name.</param>
        /// <param name="updateCallback">The callback for the thread to send progress updates to.</param>
        /// <param name="updateAmount">The number of pictures to add BEFORE sending a progress update</param>
        public void addNewPictures(addNewPictures_callback guiCallback, List<String> photoUserPath, List<String> photoExtension, int albumUID, List<String> pictureNameInAlbum, ProgressChangedEventHandler updateCallback, int updateAmount)
        {
            //TODO: JN: passing in a data class?
            photoBombDatabase.addNewPictures_backend(guiCallback, photoUserPath, photoExtension, albumUID, pictureNameInAlbum, updateCallback, updateAmount);
        }


        //By: Ryan Moe
        //Edited Last:
        //
        //This will cancel the thread from addNewPictures() if it exists.
        //Returns the error report directly.
        public ErrorReport cancelAddNewPicturesThread()
        {
            return photoBombDatabase.cancelAddNewImagesThread_backend();
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
        private int _thumbAlbumID;
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

        public int thumbAlbumID
        {
            set
            {
                if (value != _thumbAlbumID)
                {
                    _thumbAlbumID = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("thumbAlbumID");
                }
            }
            get
            {
                return _thumbAlbumID;
            }
        }

        //initialize vars.
        public SimpleAlbumData()
        {
            _albumName = "";
            _UID = -1;//indicates UID not set.
            _thumbnailPath = "";
            _thumbAlbumID = -1;
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

            if (File.Exists(source))
            {
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

            return "";
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
