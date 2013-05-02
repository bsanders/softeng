using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;

namespace SoftwareEng
{
    class zPhotoBomb_HackerHook
    {


    }


    public partial class PhotoBomb
    {
        private const String THUMBS_DB_Name = @"thumbs_db\";


        public ReportStatus initDataBase(string albumXmlPathIn, string imageXmlPathIn, string imagelibraryDirPathIn)
        {
            ErrorReport errorReport = new ErrorReport();

            //keep the paths to databases and library.
            _albumsXmlPath = albumXmlPathIn;
            _imageXmlPath = imageXmlPathIn;
            _imagelibraryDirPath = imagelibraryDirPathIn;

            
            try
            {
                // Load the xml files into memory(the XmlDataBase)
                _xmlDataBase = new XmlDataBase(albumXmlPathIn, imageXmlPathIn);
            }
            catch (IOException e)
            {
                _xmlDataBase = null;
                return ReportStatus.CANNNOT_LOAD_XML;
            }

            //check the library directory.
            ReportStatus status =  _fileDataBase.setImagelibraryDirPath(imagelibraryDirPathIn);
            if (status != ReportStatus.SUCCESS)
                return status;

            return ReportStatus.SUCCESS;
        }


        public ReportStatus rebuildDatabase()
        {
            // If there is no image dir, then do nothing. Call it a day.  
            if (!makeReadyFileDataBase())
                return ReportStatus.INVALID_IMAGE_DIR;

            // Get a new xml database.
            _xmlDataBase = new XmlDataBase();





            return ReportStatus.SUCCESS;
        }



        private bool makeReadyFileDataBase()
        {
            ReportStatus status = _fileDataBase.areDirectoriesThere();

            if (status == ReportStatus.INVALID_IMAGE_DIR)
                return false;

            // Not testing of the thumbs dir is there. (somewhat safe.)
            _fileDataBase.purgeThumbs();

            return true;
        }


        private ReportStatus getAllAlbums(out ReadOnlyObservableCollection<SimpleAlbumData> readOnlyAlbumList)
        {

            List<AlbumXmlDataReadOnly> albums = null; 
            _xmlDataBase.getAllAlbums(out albums);



            readOnlyAlbumList = null;

            return ReportStatus.SUCCESS;
        }




    } // End of PhotoBomb.
}
