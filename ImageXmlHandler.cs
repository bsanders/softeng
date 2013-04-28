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
    class ImageXmlHandler
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
        public ImageXmlHandler(XDocument imageXML)
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
        public bool addImage(String fileHash, int refCount, String extension, String filePath, String lgThumbPath)
        {
            ImageNode node = null;
            if(_fileHashToImage.TryGetValue(fileHash, out node))
                return false;

            // The image was not in the set, so add it. 
            node = new ImageNode(fileHash, refCount, extension, fileHash, lgThumbPath);
            _fileHashToImage.Add(fileHash, node);

            return true;
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
        public bool removeImage(String fileHash, out ImageXmlData imageData)
        {
            imageData = null;
            ImageNode node = null;
            if (!_fileHashToImage.TryGetValue(fileHash, out node))
                return false;

            imageData = imageNodeToImageXmlData(node);
            _fileHashToImage.Remove(fileHash);
            
            return true;
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
        public bool getImage(String fileHash, out ImageXmlData imageData)
        {
            ImageNode node = null;
            bool isGood = _fileHashToImage.TryGetValue(fileHash, out node);

            if (isGood)
            {
                imageData = imageNodeToImageXmlData(node);
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
        public void getImages(out List<ImageXmlData> list)
        {
            list = new List<ImageXmlData>();
            foreach (KeyValuePair<String, ImageNode> pair in _fileHashToImage)
            {
                list.Add(imageNodeToImageXmlData(pair.Value));
            }
        }

        public bool getImageRefCount(String fileHash, out int refCount)
        {
            refCount = -1;
            ImageNode node = null;
            if (!_fileHashToImage.TryGetValue(fileHash, out node))
                return false;

            refCount = node._refCount;
            return true;
        }

        public bool setImageRefCount(String fileHash, int refCount)
        {
            ImageNode node = null;
            if (!_fileHashToImage.TryGetValue(fileHash, out node))
                return false;

            node._refCount = refCount;
            return true;
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
        private ImageXmlData imageNodeToImageXmlData(ImageNode node)
        {
            return new ImageXmlData(node._fileHash, node._refCount, node._extension, node._filePath, node._lgThumbPath);
        }




        /// <summary>
        /// By: Julian Nguyen (4/25/13)
        /// Last Changed by: Julian Nguyen (4/25/13)
        /// 
        /// 
        /// </summary>
        /// <returns>A new XDocument of this ADT.</returns>
        public bool toXDocument(out XDocument xDoc)
        {
            xDoc = null;
            return false;
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


    } // End of ImageXmlHandler.
}
