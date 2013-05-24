using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareEng.database
{
    class AlbumsDatabase
    {
        private List<AlbumData> albumsList;

        public AlbumsDatabase()
        {
            albumsList = new List<AlbumData>();
        }

        public AlbumsDatabase(String[] pathToAlbumXmls)
        {

        }










        private AlbumData newAlbumData()
        {
            AlbumData albumData = new AlbumData();


            return albumData;
        }

        private AlbumImageData newAlbumImageData()
        {
            AlbumImageData albumImageData = new AlbumImageData();


            return albumImageData;
        }


    } // End of AlbumsDatabase.
}
