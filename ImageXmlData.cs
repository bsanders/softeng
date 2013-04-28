/*
 * Change Log: 
 * Julian Nguyen (4/25/13)
 * The class was made.
 * Julian Nguyen (4/26/13)
 * Added a copy con. 
 * This class was rename: PB_ImageData -> ImageXmlData.
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareEng
{
    /// <summary>
    
    /// By: Julian Nguyen (4/25/13)
    /// Last Changed by: Julian Nguyen (4/25/13)
    /// 
    /// </summary>
    class ImageXmlData
    {
        protected readonly String _fileHash;

        public String FileHash
        {
            get { return _fileHash; }
        }

        protected readonly int _refCount;

        public int RefCount
        {
            get { return _refCount; }
        }

        protected readonly String _extension;

        public String Extension
        {
            get { return _extension; }
        }

        protected readonly String _filePath;

        public String FilePath
        {
            get { return _filePath; }
        }

        protected readonly String _lgThumbPath;

        public String LgThumbPath
        {
            get { return _lgThumbPath; }
        }

        /// <summary>
        /// By: Julian Nguyen (4/25/13)
        /// Last Changed by: Julian Nguyen (4/25/13)
        /// 
        /// 
        /// 
        /// </summary>
        /// <param name="fileHash"></param>
        /// <param name="refCount"></param>
        /// <param name="extension"></param>
        /// <param name="filePath"></param>
        /// <param name="lgThumbPath"></param>
        public ImageXmlData(String fileHash, int refCount, String extension, String filePath, String lgThumbPath)
        {
            _fileHash = fileHash;
            _refCount = refCount; 
            _extension = extension;
            _filePath = filePath;
            _lgThumbPath = lgThumbPath;
        }

        /// <summary>
        /// By: Julian Nguyen (4/26/13)
        /// Last Changed by: Julian Nguyen (4/26/13)
        /// 
        /// copy con.
        /// 
        /// </summary>
        /// <param name="imageData"></param>
        protected ImageXmlData(ImageXmlData imageData)
        {
            _fileHash = imageData.FileHash;
            _refCount = imageData.RefCount;
            _extension = imageData.Extension;
            _filePath = imageData.FilePath;
            _lgThumbPath = imageData.LgThumbPath;
        }

    } // End of ImageXmlData.
}
