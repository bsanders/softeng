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

namespace SoftwareEng
{
    /// <summary>
    
    /// By: Julian Nguyen (4/25/13)
    /// Last Changed by: Julian Nguyen (4/25/13)
    /// 
    /// </summary>
    class PB_ImageData
    {
        private readonly String _fileHash;

        public String FileHash
        {
            get { return _fileHash; }
        }

        private readonly int _refCount;

        public int RefCount
        {
            get { return _refCount; }
        }

        private readonly String _extension;

        public String Extension
        {
            get { return _extension; }
        }

        private readonly String _filePath;

        public String FilePath
        {
            get { return _filePath; }
        }

        private readonly String _lgThumbPath;

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
        public PB_ImageData(String fileHash, int refCount, String extension, String filePath, String lgThumbPath)
        {
            _fileHash = fileHash;
            _refCount = refCount; 
            _extension = extension;
            _filePath = filePath;
            _lgThumbPath = lgThumbPath;
        }

    } // End of PB_ImageDate.
}
