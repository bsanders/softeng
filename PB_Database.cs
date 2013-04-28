using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.ComponentModel;

/*
 * Change log:
 * 4/25/13 Julian Nguyen: New file. 
 * 
 */


namespace SoftwareEng
{
    class PB_Database
    {
        private static readonly String _somePath = String.Empty;
        /// <summary>
        /// 
        /// </summary>
        private static readonly String _pathToAlbumsXML = String.Empty;
        /// <summary>
        /// 
        /// </summary>
        private static readonly String _pathToImagesXML = String.Empty;
        /// <summary>
        /// 
        /// </summary>
        private static readonly String _pathToImages = "photo library\\";
        /// <summary>
        /// 
        /// </summary>
        private static readonly String _pathToThumbs_db = _pathToImages + "thumbs_db\\";
        /// <summary>
        /// 
        /// </summary>
        private static readonly String _pathToLRGThumbs_db = _pathToThumbs_db + "lrg\\";

        

        private PB_Database_FileIO fileIO;


        public bool rebuildBackendOnFilesystem()
        {
            return false;
        }


        public bool writeXmlToDisk()
        {
            

            XDocument albumXmlDoc = null;
            _AlbumXml.toXDocument(out albumXmlDoc);

            XDocument imageXmlDoc = null;
            _ImageXml.toXDocument(out imageXmlDoc);

            return false;
            
        }


        public void getAllAlbums()
        {
            
        }


        public void getAllPhotosInAlbum(getAllPhotosInAlbum_callback guiCallback, int albumUID)
        {
           
        }


        public void sendSelectedPhotosToClipboard(sendAllPhotosInAlbum_callback guiCallback, int albumUID)
        {
           
        }


        public void getPhoto(getPhotoByUID_callback guiCallback, int photoUID, int albumUID)
        {
           ;
        }


        public void removePictureFromAlbum(generic_callback guiCallback, int idInAlbum, int albumUID)
        {
            
        }


        public void removeAlbum(generic_callback guiCallback, int albumUID)
        {
            
        }


        public void renameAlbum(generic_callback guiCallback, int albumUID, string newName)
        {
           
        }


        public void renamePhoto(generic_callback guiCallback, int albumUID, int idInAlbum, string newName)
        {
            
        }


        public void setPhotoCaption(generic_callback guiCallback, int albumUID, int idInAlbum, string newCaption)
        {
           ;
        }


        public void addNewAlbum(generic_callback guiCallback, SimpleAlbumData albumData)
        {
           
        }


        public void addExistingPhotosToAlbum(addNewPictures_callback guiCallback, List<ComplexPhotoData> photoList, int albumUID)
        {
            
        }


        public void checkIfAlbumNameIsUnique(generic_callback guiCallback, String albumName)
        {
           
        }

        public void checkIfPhotoNameIsUnique(generic_callback guiCallback, String photoName, int albumUID)
        {
            
        }


        public void changePhotoNameByUID(generic_callback guiCallback, int albumUID, int photoUID, String newName)
        {
            
        }



        public void addNewPictures(addNewPictures_callback guiCallback, List<String> photoUserPath,
            List<String> photoExtension, int albumUID, List<String> pictureNameInAlbum,
            ProgressChangedEventHandler updateCallback, int updateAmount)
        {
            
        }
        





    }
}
