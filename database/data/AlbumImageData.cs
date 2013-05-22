using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareEng.database
{
    [Serializable]
    internal class AlbumImageData
    {
        /// <summary>
        /// The hash of the image file. This will be used as the image UID.
        /// </summary>
        internal byte[] fileHash { get; set; }


        internal String imageName { get; set; }
        internal String imageCaption { get; set; }

        internal DateTime dateAdded { get; set; }


        public override bool Equals(object obj)
        {
            AlbumImageData aid = obj as AlbumImageData;
            if (aid == null)
                return false;
            else return this.fileHash.SequenceEqual(aid.fileHash);
        }

        public override int GetHashCode()
        {
            return this.fileHash.GetHashCode();
        }

    } // End of AlbumImageData
}
