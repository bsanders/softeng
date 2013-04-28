using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareEng
{
    class AlbumImageXmlData
    {
        private readonly String _fileHash;

        public String FileHash
        {
            get { return _fileHash; }
        } 
            
        private readonly String _imageName;

        public String ImageName
        {
            get { return _imageName; }
        }

        private readonly String _caption;

        public String Caption
        {
            get { return _caption; }
        }

        public AlbumImageXmlData(String fileHash, String imageName, String caption)
        {
            _fileHash = fileHash;
            _imageName = imageName;
            _caption = caption;
        }

    } // End of AlbumImageXmlData. 
}
