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
 * class SimpleAlbumData was moved to SimpleAlbumData.cs.
 * class ComplexPhotoData was moved to ComplexPhotoData.cs
 * class SimplePhotoData was removed. 
 * Julian Nguyen(2/30/13)
 * Some the error Reports were not being set in some of the functions. (null exception) 
 * Fun() with "Picture" in the name were changed to "Image"
 * Julian Nguyen(5/6/13)
 * Set and get CurrentTheme and Image Grayscale was added.
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
using System.Drawing;

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
    public class zzobsoletezzPhotoBomb_Controller
    {
        // A handy shortcut to the settings class...
        static Properties.Settings Settings = Properties.Settings.Default;

        // Tools of this class.
        private PhotoBomb _photoBombDatabase;
        private KeyValuePairDataBase _settingsDatabase;
        private ImageManipulation _imageManipulation;

        // The path of the files of the database.
        static readonly private String _basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Settings.OrgName);

        static readonly private String _albumDatabasePath = Path.Combine(_basePath, Settings.AlbumXMLFile);
        static readonly private String _imageDatabasePath = Path.Combine(_basePath, Settings.PhotoXMLFile);
        static readonly private String _libraryPath = Path.Combine(_basePath, Settings.PhotoLibraryName);
        static readonly private String _settingPath = Path.Combine(_basePath, Settings.SettingPath); 

        /// By Julian Nguyen
        /// Edited: Julian Ngugen(4/28/13)
        /// <summary>
        /// This will setup the class. 
        /// </summary>
        public zzobsoletezzPhotoBomb_Controller()
        {
            _imageManipulation = new ImageManipulation();
            _photoBombDatabase = new PhotoBomb();
        }


        /// By: Ryan Moe
        /// Edited: Julian Nguyen(4/28/13) 
        /// <summary>
        /// initialize the backend object AND load the databases. DOES NOT re-initialize the databases if there is a problem.
        /// Error: Fails if any of the XML files is nissing or if the image folder does not exist.
        /// </summary>
        /// <param name="guiCallback">The callback to the GUI.</param>
        /// <param name="albumDatabasePathIn">The path to the album XML file.</param>
        /// <param name="imageDatabasePathIn">The path to the image XML file.</param>
        /// <param name="libraryPath">The path to the folder where all the images are stored.</param>
        public void init(generic_callback guiCallback, string albumDatabasePathIn, string imageDatabasePathIn, string libraryPath)
        {
            ErrorReport errReport = null;
            errReport = _photoBombDatabase.init_backend(_albumDatabasePath, _imageDatabasePath, _libraryPath);

            loadsettingsDatabase(_settingPath);

            guiCallback(errReport);


        }


        /// By: Julian Nguyen
        /// Edited : Julian Nguyen(5/7/13)
        /// <summary>
        /// This will load the settings from file.
        /// </summary>
        /// <param name="pathToKeyValue"></param>
        private ReportStatus loadsettingsDatabase(String pathToKeyValue)
        {
            try
            {
                // Try to load the setting.
                _settingsDatabase = new KeyValuePairDataBase(pathToKeyValue);
                return ReportStatus.SUCCESS;
            }
            catch
            {
                // Oh well, here's a new one.
                _settingsDatabase = new KeyValuePairDataBase();
                return ReportStatus.CANNNOT_LOAD_SETTINGS;
            }
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
            errReport =  _photoBombDatabase.rebuildBackendOnFilesystem_backend();
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
            errReport =  _photoBombDatabase.saveAlbumsXML_backend();
            if(guiCallback != null)
                guiCallback(errReport);
        }



        /// By Ryan Moe
        /// Edited: Julian Nguyen(4/28/13)
        /// <summary>
        /// Will save the Image XML to file.
        /// </summary>
        /// <param name="guiCallback">The call back to the GUI.</param>
        public void saveImagesXML(generic_callback guiCallback)
        {
            ErrorReport errReport = null;
            errReport = _photoBombDatabase.saveImagesXML_backend();
            if(guiCallback != null)
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
            errReport = _photoBombDatabase.getAllAlbums_backend(out readOnlyAlbumList);
            guiCallback(errReport, readOnlyAlbumList);
        }




        /// By Ryan Moe
        /// Edited: Julian Nguyen(4/27/13)
        /// <summary>
        /// Will get all the images of an Album. 
        /// </summary>
        /// <param name="guiCallback">The callback for the GUI.</param>
        /// <param name="albumUID">The ID of the Album. </param>
        public void getAllImagesInAlbum(getAllPhotosInAlbum_callback guiCallback, Guid albumUID)
        {
            ErrorReport errReport = null;
            ReadOnlyObservableCollection<ComplexPhotoData> imagesOfAnAlbum = null;
            errReport = _photoBombDatabase.getAllImagesInAlbum_backend(albumUID, out imagesOfAnAlbum);
            guiCallback(errReport, imagesOfAnAlbum);
        }

        /// By: Bill Sanders
        /// Edited: Julian Nguyen(3/28/13)
        /// <summary>
        /// Sends a copy of all the photos in the specified album to the clipboard
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="albumUID">The unique ID of the album</param>
        public void sendSelectedImagesToClipboard(sendAllPhotosInAlbum_callback guiCallback, Guid albumUID)
        {
            ErrorReport errReport = null;
            List<ComplexPhotoData> images = null;
            errReport = _photoBombDatabase.sendSelectedImagesToClipboard_backend(albumUID, out images);
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
        public void getImage(getPhotoByUID_callback guiCallback, int photoUID, Guid albumUID)
        {
            ErrorReport errReport = null;
            ComplexPhotoData imageData = null;
            errReport = _photoBombDatabase.getImage_backend(photoUID, albumUID, out imageData);
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
        public void removeImageFromAlbum(generic_callback guiCallback, int idInAlbum, Guid albumUID)
        {
            ErrorReport errReport = null;
            errReport = _photoBombDatabase.removeImageFromAlbum_backend(idInAlbum, albumUID);
            guiCallback(errReport);
        }

        //By: Bill Sanders
        //Edited Last: 3/28/13
        /// <summary>
        /// This function removes the specified album.
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="albumUID">The UID of the album</param>
        public void removeAlbum(generic_callback guiCallback, Guid albumUID)
        {
            ErrorReport errReport = null;
            errReport =  _photoBombDatabase.removeAlbum_backend(albumUID);
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
        public void setAlbumName(generic_callback guiCallback, Guid albumUID, string newName)
        {
            ErrorReport errReport = null;
            errReport = _photoBombDatabase.setAlbumName_backend(albumUID, newName);
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
        public void setImageName(generic_callback guiCallback, Guid albumUID, int idInAlbum, string newName)
        {
            ErrorReport errReport = null; 
            errReport = _photoBombDatabase.setImageName_backend(albumUID, idInAlbum, newName);
            guiCallback(errReport);
        }


        //By: Bill Sanders
        //Edited Last: Julian Nguyen(4/30/13)
        /// <summary>
        /// This function sets the specified caption on the specified photo in an album.
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="albumUID">The UID of the album the photo is in</param>
        /// <param name="idInAlbum">The id of the photo in this album</param>
        /// <param name="newName">The caption to be added to the photo</param>
        public void setImageCaption(generic_callback guiCallback, Guid albumUID, int idInAlbum, string newCaption)
        {

            ErrorReport errReport = null;
            errReport = _photoBombDatabase.setImageCaption_backend(albumUID, idInAlbum, newCaption);
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
            errReport = _photoBombDatabase.addNewAlbum_backend(albumData);
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
        public void addExistingImagesToAlbum(addNewPictures_callback guiCallback, List<ComplexPhotoData> photoList, Guid albumUID)
        {
            ErrorReport errReport = null;
             errReport = _photoBombDatabase.addExistingImagesToAlbum_backend(photoList, albumUID);
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
            errReport = _photoBombDatabase.isAlbumNameUnique_backend(albumName, out isUnique);
            guiCallback(errReport);
        }

        
        //By: Bill Sanders
        //Edited: Julian Nguyen(4/30/13)
        /// <summary>
        /// Checks to see if a photo name is unique.
        /// This will return FAILED in the error report (in the callback) 
        /// if the name is not unique.
        /// </summary>
        /// <param name="guiCallback">The callback to the GUI.</param>
        /// <param name="photoName">The name of the image to be tested.</param>
        /// <param name="albumUID">The ID of the Album that the Image is in.</param>
        public void isImageNameUnique(generic_callback guiCallback, String photoName, Guid albumUID)
        {
            ErrorReport errReport = null;
            bool isUnique = false;
            errReport = _photoBombDatabase.isImageNameUnique_backend(photoName, albumUID, out isUnique);
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
        public void setImageNameByUID(generic_callback guiCallback, Guid albumUID, int photoUID, String newName)
        {
            ErrorReport errReport = null;
            errReport =  _photoBombDatabase.setImageNameByUID_backend( albumUID, photoUID, newName);
            guiCallback(errReport);

        }
        

        /// By Julian Nguyen
        /// Edited: Julian Nguyen(5/7/13)
        /// <summary>
        /// This will take an image and grayscale it and then add it to the Database.
        /// </summary>
        /// <param name="guiCallback">The call back to the gui.</param>
        /// <param name="albumUID">The uid of the album.</param>
        /// <param name="imagePath">The path to the image.</param>
        public void addImageAsGrayscale(addNewPictures_callback guiCallback, Guid albumUID, String imagePath)
        {
            String pathOfnewGray = String.Empty;
            try
            {
                // Load the image into memory.
                Bitmap image = _imageManipulation.LoadImageNoLock(imagePath);
                // Grayscale the image in memory.
                Bitmap newGray = _imageManipulation.makeGrayscale(image);
                // Get a path for the image.
                pathOfnewGray = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".jpg");
                // Save the image.
                newGray.Save(pathOfnewGray);
            }
            catch (Exception)
            {
                ErrorReport err = new ErrorReport();
                err.reportStatus = ReportStatus.FAILURE;
                guiCallback(err, albumUID);
            }

            
            ErrorReport errReport = null; 
            try
            {
                errReport = _photoBombDatabase.addNewImage(pathOfnewGray, ".jpg", albumUID);
            }
            catch
            {
                errReport =  new ErrorReport();
                errReport.reportStatus = ReportStatus.FAILURE;
            }

            try
            {
                File.Delete(pathOfnewGray);
            } catch { }

            guiCallback(errReport, albumUID);
        }



        /// By: Ryan Moe
        /// Edited Julian Nguyen
        /// <summary>
        /// THIS IS THREADED, call this instead of multiple calls to addnewPicture to 
        /// prevent gui lockup. Adds multiple new photos to the databases and moves 
        /// a copy of the picture to the library. Also writes all these changes to the disk 
        /// </summary>
        /// <param name="guiCallback"></param>
        /// <param name="imageUserPath">List of photo paths on the disk</param>
        /// <param name="imageExtension">List of photo extensions</param>
        /// <param name="albumUID">The Album UID.</param>
        /// <param name="inAlbumImageName">NOTE: you can pass in NULL for the list for all default names, or you can have "" for a single element for a single default name.</param>
        /// <param name="updateCallback">The callback for the thread to send progress updates to.</param>
        /// <param name="updateAmount">The number of pictures to add BEFORE sending a progress update</param>
        public void addNewImages(addNewPictures_callback guiCallback, List<String> imageUserPath, List<String> imageExtension, Guid albumUID, List<String> inAlbumImageName, ProgressChangedEventHandler updateCallback, int updateAmount)
        {
            //TODO: JN: passing in a data class?
            _photoBombDatabase.addNewImages_backend(guiCallback, imageUserPath, imageExtension, albumUID, inAlbumImageName, updateCallback, updateAmount);
        }


        
        /// By Ryan Moe
        /// Edited Julian Nguyen(5/7/13)
        /// <summary>
        /// This will cancel the thread from addNewPictures() if it exists.
        /// </summary>
        /// <returns>The error report  of this action. </returns>
        public ErrorReport cancelAddNewImagesThread()
        {
            return _photoBombDatabase.cancelAddNewImagesThread_backend();
        }


        /// By Julian Nguyen
        /// Edited: Julian Nguyen(5/7/13)
        /// <summary>
        /// This will get the current theme from the setting file. 
        /// </summary>
        /// <param name="currentTheme">The passback of the current theme</param>
        /// <returns>The error report  of this action.</returns>
        public ReportStatus getCurrentTheme(out String currentTheme)
        {
            currentTheme = _settingsDatabase.getValue(Settings.CurrentThemeKey);
            if (currentTheme == null)
                return ReportStatus.FAILURE;
            return ReportStatus.SUCCESS;
        }

        /// By Julian Nguyen
        /// Edited: Julian Nguyen(5/7/13)
        /// <summary>
        /// This will set the current theme from the gui to file.
        /// </summary>
        /// <param name="currentTheme">The current theme from the gui </param>
        /// <returns>The error report  of this action.</returns>
        public ReportStatus setCurrentTheme(String currentTheme)
        {
            _settingsDatabase.setKeyValuePair(Settings.CurrentThemeKey, currentTheme);

            try
            {
                _settingsDatabase.saveToFile(_settingPath);
                return ReportStatus.SUCCESS;
            }
            catch (Exception)
            {
                return ReportStatus.CANNNOT_SAVE_SETTINGS;
            }
        }


    } // End of PhotoBomb_Controller.


}//namespace
