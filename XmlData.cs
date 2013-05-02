/*
 * Change Log:
 * Julian Nguyen(4/29/13)
 * This file was made with the classes XmlData, ImageXmlData, AlbumXmlData, and AlbumImageXmlData
 * Julian Nguyen(5/1/13)
 * Add Equals GetHashCode fun() to all class. But not XmlData class.
 * Julian Nguyen(5/1/13)
 * change albumID from int to Guid
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareEng
{
    /// By Julian Nguyen
    /// Edited: Julian Nguyen(4/29/13)
    /// <summary>
    /// This is just a base class for XmlData. It's not really needed.
    /// </summary>
    public class XmlData
    {


    } // End of XmlData.



    // Image Xml Data Classes.

    /// By Julian Nguyen
    /// Edited: Julian Nguyen(5/1/13)
    /// <summary>
    /// A data class for the image.
    /// </summary>
    [Serializable()]
    public class ImageXmlData 
    {

        // ID
        public byte[] _hashValue { get; set; }

        // Files
        public string _filePath { get; set; }
        public String _lgThumbPath { get; set; }

        // File Data.
        public String _extension { get; set; }
        public DateTime addedDate { get; set; }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
                return false;

            ImageXmlData otherData = obj as ImageXmlData;
            if ((System.Object)otherData == null)
                return false;

            return _hashValue.Equals(otherData._hashValue);
        }

        public override int GetHashCode()
        {
            int sum = 0;
            foreach (byte b in _hashValue)
            {
                sum ^= b * b;
            }
            return sum;
        }

    } // End of ImageXmlData.



    // Album Xml Data classes.

    /// By Julian Nguyen
    /// Edited: Julian Nguyen(5/2/13)
    /// <summary>
    /// A data class for the album. 
    /// </summary>
    [Serializable()]
    public class AlbumXmlData 
    {
        // Album data.
        public Guid _albumUID { get; set; }
        public String _albumName { get; set; }
        public byte[] _thumbHashValue { get; set; }

        // All the images in the Album.
        public List<AlbumImageXmlData> _images { get; set; }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
                return false;

            AlbumXmlData otherData = obj as AlbumXmlData;
            if ((System.Object)otherData == null)
                return false;

            return _albumUID.Equals(otherData._albumUID);
        }

        public override int GetHashCode()
        {
            //TODO:  What are you doing??
            return _albumUID.GetHashCode();
        }

    } // End of AlbumXmlData.

    /// By Julian Nguyen
    /// Edited: Julian Nguyen(5/1/13)
    /// <summary>
    /// A data class for the iamges in an Album.
    /// </summary>
    [Serializable()]
    public class AlbumImageXmlData 
    {

        // ID
        public byte[] _imageHashValue { get; set; }

        // Image data.
        public String _imageName { get; set; }
        public String _imageCaption { get; set; }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
                return false;

            AlbumImageXmlData otherData = obj as AlbumImageXmlData;
            if ((System.Object)otherData == null)
                return false;

            return _imageHashValue.Equals(otherData._imageHashValue);
        }

        public override int GetHashCode()
        {
            int sum = 0;
            foreach (byte b in _imageHashValue)
            {
                sum ^= b * b;
            }
            return sum;
        }

    } // End of AlbumImageXmlData. 

}
