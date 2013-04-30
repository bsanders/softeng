/*
 * Change Log:
 * Julian Nguyen(4/30/13)
 * This file was made with the classes XmlDataReadOnly, ImageXmlDataReadOnly, AlbumXmlDataReadOnly, and AlbumImageXmlDataReadOnly
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareEng
{
    /// By Julian Nguyen
    /// Edited: Julian Nguyen(4/30/13)
    /// <summary>
    /// 
    /// </summary>
    public class XmlDataReadOnly
    {
    }

    // Image Xml Data Classes.

    /// By Julian Nguyen
    /// Edited: Julian Nguyen(4/30/13)
    /// <summary>
    /// A read only data class for the image.
    /// </summary>
    public class ImageXmlDataReadOnly
    {

        // ID
        private readonly byte[] _hashValue;

        // Files
        private readonly string _filePath;
        private readonly String _lgThumbPath;

        // File Data.
        private readonly String _extension;
        private readonly DateTime _addedDate;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageData"></param>
        public ImageXmlDataReadOnly(ImageXmlData imageData)
        {
            imageData._hashValue = _hashValue;
            imageData._filePath = _filePath;
            imageData._lgThumbPath = _lgThumbPath;
            imageData._extension = _extension;
            imageData.addedDate = _addedDate;
        }


        // Getters.

        public byte[] HashValue
        {
            get { return _hashValue; }
        }

        public String FilePath
        {
            get { return _filePath; }
        }

        public String LgThumbPath
        {
            get { return _lgThumbPath; }
        }


        public String Extension
        {
            get { return _extension; }
        }


        public DateTime AdddedDate
        {
            get { return _addedDate; }
        } 

    } // End of ImageXmlData.



    // Album Xml Data classes.

    /// By Julian Nguyen
    /// Edited: Julian Nguyen(4/30/13)
    /// <summary>
    /// A read only data class for the album. 
    /// </summary>
    public class AlbumXmlDataReadOnly
    {
        // Album data
        private readonly int _albumUID;
        private readonly String _albumName;
        private readonly byte[] _thumbHashValue;

        // All the images in the Album.
        private readonly List<AlbumImageXmlDataReadOnly> _images;

        /// By Julian Nguyen
        /// Edited: Julian Nguyen(4/30/13)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="albumData"></param>
        public AlbumXmlDataReadOnly(AlbumXmlData albumData)
        {
            albumData._albumUID = _albumUID;
            albumData._albumName = _albumName;
            albumData._thumbHashValue = _thumbHashValue;

            _images = new List<AlbumImageXmlDataReadOnly>();
            foreach (AlbumImageXmlData imageData in albumData._images)
            {
                _images.Add(new AlbumImageXmlDataReadOnly(imageData));
            }
        }

        // Getters.



        public int AlbumUID
        {
            get { return _albumUID; }
        }

        public String AlbumName
        {
            get { return _albumName; }
        }

        public byte[] ThumbHashValue
        {
            get { return _thumbHashValue; }
        }

        public List<AlbumImageXmlDataReadOnly> Images
        {
            get { return _images; }
        } 


    } // End of AlbumXmlData.

    /// By Julian Nguyen
    /// Edited: Julian Nguyen(4/29/13)
    /// <summary>
    /// A read only data class for the iamges in an Album.
    /// </summary>
    public class AlbumImageXmlDataReadOnly
    {
        // ID
        private readonly byte[] _imageHashValue;

        // Image data.
        private readonly String _imageName;
        private readonly String _imageCaption;

        /// By Julian Nguyen
        /// Edited: Julian Nguyen(4/30/13)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="albumImageData"></param>
        public AlbumImageXmlDataReadOnly(AlbumImageXmlData albumImageData)
        {
            albumImageData._imageHashValue = _imageHashValue;
            albumImageData._imageName = _imageName;
            albumImageData._imageCaption = ImageCaption;
        }

        // Getters.

        public byte[] ImageHashValue
        {
            get { return _imageHashValue; }
        } 
 
        public String ImageName
        {
            get { return _imageName; }
        }

        public String ImageCaption
        {
            get { return _imageCaption; }
        } 

    } // End of AlbumImageXmlData. 
}
