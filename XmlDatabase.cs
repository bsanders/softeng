/*
 * Change Log: 
 * Julian Nguyen (4/25/13)
 * The class was made.
 * 
 * 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;



namespace SoftwareEng
{
    /// <summary>
    /// By: Julian Nguyen (4/25/13)
    /// Last Changed by: Julian Nguyen (4/25/13)
    /// 
    /// This class is to handle the XML in memory.
    /// 
    /// </summary>
    class XmlDatabase
    {

        private AlbumXmlHandler _albumsXML;
        private ImageXmlHandler _ImagesXML;




        public XmlDatabase(XDocument albumXmlDoc, XDocument imageXmlDoc)
        {
            _albumsXML = new AlbumXmlHandler(albumXmlDoc);
            _ImagesXML = new ImageXmlHandler(imageXmlDoc);
        }



        public bool addAlbum(String albumUID, String albumName, String thumbFileHash)
        {
            return _albumsXML.addAlbum(albumUID, albumName, thumbFileHash);
        }

        public bool removeAlbum(String albumUID, out List<String> fileToRemove)
        {
            fileToRemove = null;

            return false;
        }

        public bool setAlbumName(AlbumXmlData albumData)
        {
            return _albumsXML.renameAlbum(albumData.AlbumUID, albumData.AlbumName);
        }

        public bool addImageToAlbum(AlbumXmlData albumData, AlbumImageXmlData albumImageData)
        {
            if(!_albumsXML.addImage(albumData.AlbumUID, albumImageData.FileHash, albumImageData.ImageName, albumImageData.Caption))
                return false;

            ImageXmlData imageXmlData = null;
            if (_ImagesXML.getImage(albumImageData.FileHash, out imageXmlData))
            {
                int refCount = 0;
                _ImagesXML.getImageRefCount(albumImageData.FileHash, out refCount);
                _ImagesXML.setImageRefCount(albumImageData.FileHash, ++refCount);
                return true;
            }
            return false;
        }


        public bool removeImageFromAlbum(AlbumXmlData albumData, AlbumImageXmlData imageData, out ImageXmlData removeImage, out bool removeImageFlag)
        {
            removeImageFlag = false;
            removeImage = null;

            if(!_albumsXML.removeImage(albumData.AlbumUID, imageData.FileHash))
                return false;

            int imageRefCount = 0;
            if (_ImagesXML.getImageRefCount(imageData.FileHash, out imageRefCount))
            {
                if (imageRefCount == 0)
                {
                    removeImageFlag = true;
                    return _ImagesXML.removeImage(imageData.FileHash, out removeImage);
                }
                return true;
            }

            return false;
        }

        public bool addImageToLib(String fileHash, String extension, String filePath, String lgThumbPath)
        {
            int refCount = 0;
            if(_ImagesXML.getImageRefCount(fileHash, out refCount))
            {
                return _ImagesXML.setImageRefCount(fileHash, ++refCount);
            }
            return _ImagesXML.addImage(fileHash, 0, extension, filePath, lgThumbPath);
        }

        public bool setImageName(AlbumXmlData albumData, AlbumImageXmlData newImageData)
        {
            return _albumsXML.setImageName(albumData.AlbumUID, newImageData.FileHash, newImageData.ImageName);
        }


        public bool setImageCaption(AlbumXmlData albumData, AlbumImageXmlData newImageData)
        {
            return _albumsXML.setImageCaption(albumData.AlbumUID, newImageData.FileHash, newImageData.Caption);
        }


        public void getAllAlbums(out List<AlbumXmlData> albums)
        {
            _albumsXML.getAlbums(out albums);
        }


    } //End of XmlDatabase.
}
