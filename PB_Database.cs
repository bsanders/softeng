using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

/*
 * Change log:
 * 4/25/13 Julian Nguyen: New file. 
 * 
 */


namespace SoftwareEng
{
    class PB_Database
    {

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

      




        private Properties.Settings _settings;

        private PB_Database_FileIO _fileIO;
        private XDocument aa;
        private XDocument ab;
        private XDocument ac;


        private FileInfo _rootDir;



        public PB_Database(String rootDir)
        {
            _rootDir = new FileInfo(rootDir);


            _fileIO = new PB_Database_FileIO(_settings);
        }


        





    }
}
