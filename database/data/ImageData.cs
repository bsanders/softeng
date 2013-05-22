using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareEng.database
{
    [Serializable]
    internal class ImageData
    {
        /// <summary>
        /// The hash of the image file. This will be used as the image UID.
        /// </summary>
        internal byte[] fileHash { get; set; }

        internal String pathToFile { get; set; }
        internal String pathToThumbnail { get; set; }

        internal DateTime dateAdded { get; set; }


        public override bool Equals(object obj)
        {
            ImageData imageData = obj as ImageData;
            if (obj == null)
                return false;
            else return this.fileHash.SequenceEqual(imageData.fileHash);
        }

        public override int GetHashCode()
        {
            return this.fileHash.GetHashCode();
        }

    } // End of ImageData.
}
