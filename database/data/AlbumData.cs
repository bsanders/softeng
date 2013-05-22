using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareEng.database
{
    [Serializable]
    internal class AlbumData
    {
        internal Guid albumUID { get; set; }

        internal String albumName { get; set; }

        internal DateTime dateAdded { get; set; }

        internal List<AlbumImageData> imagesList { get; set; }

        public override bool Equals(object obj)
        {
            AlbumData albumData = obj as AlbumData;
            if (albumData == null)
                return false;
            else return this.albumUID.Equals(albumData.albumUID);
        }

        public override int GetHashCode()
        {
            return this.albumUID.GetHashCode();
        }

    } // End of AlbumData
}
