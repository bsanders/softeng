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
using System.Xml.Linq;

namespace SoftwareEng
{
    /// <summary>
    /// By: Julian Nguyen (4/25/13)
    /// Last Changed by: Julian Nguyen (4/25/13)
    /// 
    /// 
    /// 
    /// </summary>
    class PB_ImageXmlHandler
    {

        /// <summary>
        /// By: Julian Nguyen (4/25/13)
        /// Last Changed by: Julian Nguyen (4/25/13)
        /// 
        /// This is a data class just for the PB_ImageXmlHandler. 
        /// 
        /// </summary>
        private class ImageNode
        {
            public String _fileHash;
            public int _refCount;
            public String _extension;
            public String _filePath;
            public String _lgThumbPath;

            public ImageNode(String fileHash, int refCount, String extension, String filePath, String lgThumbPath)
            {
                _fileHash = fileHash;
                _refCount = refCount; 
                _extension = extension;
                _filePath = filePath;
                _lgThumbPath = lgThumbPath;
            }
        } // End of ImageNode. 

        /// <summary>
        /// When a new iamge is added it will be given this number had its start refCount. 
        /// </summary>
        private readonly static int _refCount_start = 1;
        /// <summary>
        /// This is a map of fileHash to ImageNode. 
        /// </summary>
        private Dictionary<String, ImageNode> _fileHashToImage;


        /// <summary>
        /// By: Julian Nguyen (4/25/13)
        /// Last Changed by: Julian Nguyen (4/25/13)
        /// 
        /// 
        /// </summary>
        /// <param name="imageXML"></param>
        PB_ImageXmlHandler(XDocument imageXML)
        {
            _fileHashToImage = xDocumentToDictionary(imageXML);
        }

        /// <summary>
        /// By: Julian Nguyen (4/25/13)
        /// Last Changed by: Julian Nguyen (4/25/13)
        /// 
        /// This will only match two images by their filehash.
        /// 
        /// </summary>
        /// <param name="fileHash"></param>
        /// <param name="extension"></param>
        /// <param name="filePath"></param>
        /// <param name="lgThumbPath"></param>
        /// <returns>The refcount for the image.</returns>
        public int addImage(String fileHash, String extension, String filePath, String lgThumbPath)
        {
            ImageNode node = null;
            bool isGood = _fileHashToImage.TryGetValue(fileHash, out node);

            if (isGood)
            {
                // Test if the image was in the map.
                return ++node._refCount;
            }
            else
            {
                // The image was not in the set, so add it. 
                node = new ImageNode(fileHash, _refCount_start, extension, fileHash, lgThumbPath);
                _fileHashToImage.Add(fileHash, node);

                return _refCount_start;
            }
        }

        /// <summary>
        /// By: Julian Nguyen (4/25/13)
        /// Last Changed by: Julian Nguyen (4/25/13)
        /// 
        /// This will remove an image by their fileHash. 
        /// Then this will return the refcount of the image. 
        /// 
        /// If the filehash is not in the set of filehashs, then this will return -1.
        /// 
        /// </summary>
        /// <param name="fileHash"></param>
        /// <returns></returns>
        public int removeImage(String fileHash)
        {
            ImageNode node = null;
            bool isGood = _fileHashToImage.TryGetValue(fileHash, out node);

            if (!isGood)
                return -1; //The fileHash was not in the map, so return error.

            if (node._refCount == _refCount_start)
            {
                // Test if the refCount is back at the start, if so then remove it. 
                _fileHashToImage.Remove(fileHash);
                return 0;
            }
            // Decrement the refcount then return it. 
            return --node._refCount;
        }


        /// <summary>
        /// By: Julian Nguyen (4/25/13)
        /// Last Changed by: Julian Nguyen (4/25/13)
        /// 
        /// 
        /// </summary>
        /// <param name="fileHash"></param>
        /// <param name="imageData"></param>
        /// <returns></returns>
        public bool getImage(String fileHash, out PB_ImageData imageData)
        {
            ImageNode node = null;
            bool isGood = _fileHashToImage.TryGetValue(fileHash, out node);

            if (isGood)
            {
                imageData = imageNodeToImageData(node);
                return true;
            }
            else
            {
                imageData = null;
                return false;
            }
        }

        /// <summary>
        /// By: Julian Nguyen (4/25/13)
        /// Last Changed by: Julian Nguyen (4/25/13)
        /// 
        /// 
        /// 
        /// </summary>
        /// <param name="list"></param>
        public void getImages(out List<PB_ImageData> list)
        {
            list = new List<PB_ImageData>();
            foreach (KeyValuePair<String, ImageNode> pair in _fileHashToImage)
            {
                list.Add(imageNodeToImageData(pair.Value));
            }
        }


        /// <summary>
        /// By: Julian Nguyen (4/25/13)
        /// Last Changed by: Julian Nguyen (4/25/13)
        /// 
        /// 
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private PB_ImageData imageNodeToImageData(ImageNode node)
        {
            return new PB_ImageData(node._fileHash, node._refCount, node._extension, node._filePath, node._lgThumbPath);
        }




        /// <summary>
        /// By: Julian Nguyen (4/25/13)
        /// Last Changed by: Julian Nguyen (4/25/13)
        /// 
        /// 
        /// </summary>
        /// <returns>A new XDocument of this ADT.</returns>
        public XDocument toXDocument()
        {
            return null;
        }

        /// <summary>
        /// By: Julian Nguyen (4/25/13)
        /// Last Changed by: Julian Nguyen (4/25/13)
        /// 
        /// 
        /// 
        /// </summary>
        /// <param name="xDoc"></param>
        /// <returns></returns>
        private Dictionary<String, ImageNode> xDocumentToDictionary(XDocument xDoc)
        {
            return null;
        }


    } // End of PB_ImageXmlHandler.
}
