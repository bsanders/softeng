using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareEng.database
{
    class ImagesDatabase
    {
        private Dictionary<byte[], ImageData> imagesMap;

        public ImagesDatabase()
        {
            imagesMap = new Dictionary<byte[], ImageData>();
        }

        public ImagesDatabase(String pathToImageXml)
        {
            
        }


        public ImageData addImage(ImageData imageData)
        {
            if (imagesMap.ContainsKey(imageData.fileHash))
                return null;

            imagesMap.Add(imageData.fileHash, imageData);

            return imageData;
        }

        public ImageData removeImage(ImageData imageData)
        {
            return removeImage(imageData.fileHash);
        }

        public ImageData removeImage(byte[] fileHash)
        {
            ImageData imageData = null;
            if (imagesMap.TryGetValue(fileHash, out imageData))
            {
                imagesMap.Remove(fileHash);
                return imageData;
            }
            return null;
        }

        public ImageData getImage(byte[] fileHash)
        {
            ImageData imageData = null;
            if (imagesMap.TryGetValue(fileHash, out imageData))
                return imageData;
            else return null;
        }

        public bool containsFileHash(byte[] fileHash)
        {
            return imagesMap.ContainsKey(fileHash);
        }



    } // End of ImagesDatabase.
}
