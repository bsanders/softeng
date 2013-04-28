/*
 * Change Log: 
 * Julian Nguyen (4/26/13)
 * The class was made.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareEng
{
    /// <summary>
    /// 
    /// </summary>
    class AlbumXmlData 
    {
        private readonly String _albumUID;

        public String AlbumUID
        {
            get { return _albumUID; }
        }

        private readonly String _albumName;

        public String AlbumName
        {
            get { return _albumName; }
        } 

        private readonly String _thumbFileHash;

        public String ThumbFileHash
        {
            get { return _thumbFileHash; }
        } 

        private readonly List<AlbumImageXmlData> _images;

        internal List<AlbumImageXmlData> Images
        {
            get { return _images; }
        } 


        
        public AlbumXmlData(String albumUID, String albumName, String thumbFileHash, List<AlbumImageXmlData> images)
        {
            _albumUID = albumUID;
            _albumName = albumName;
            _thumbFileHash = thumbFileHash;
            _images = images;
        }
    }
}
