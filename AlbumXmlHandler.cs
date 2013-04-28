/*
 * Change Log: 
 * Julian Nguyen (4/26/13)
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
    /// 
    /// </summary>
    class AlbumXmlHandler
    {
        /// <summary>
        /// 
        /// </summary>
        private class AlbumNode
        {
            public String _albumUID;
            public String _albumName;
            public String _thumbFileHash;
            public List<ImageNode> _images;

            public AlbumNode(String albumUID, String albumName, String thumbFileHash)
            {
                _albumUID = albumUID;
                _albumName = albumName;
                _thumbFileHash = thumbFileHash;
                _images = new List<ImageNode>();
            }

            public bool contains(String imageFileHash)
            {
                foreach(ImageNode image in _images)
                {
                    if(imageFileHash.Equals(image._fileHash))
                        return true;
                }
                return false;
            }

        } // End of AlbumNode.

        /// <summary>
        /// 
        /// </summary>
        private class ImageNode
        {
            public String _fileHash;
            public String _imageName;
            public String _caption;

            public ImageNode(String fileHash, String imageName, String caption)
            {
                _fileHash = fileHash;
                _imageName = imageName;
                _caption = caption;
            }
        } // End of ImageNode.



        List<AlbumNode> _albumList;


        /// <summary>
        /// 
        /// </summary>
        public AlbumXmlHandler(XDocument albumXML)
        {
            _albumList = xDocumentToList(albumXML);
        }


        public bool addAlbum(String albumUID, String albumName, String thumbFileHash)
        {
            if (this.contains(albumUID))
                return false;
            
            _albumList.Add(new AlbumNode(albumUID, albumName, thumbFileHash));
            return true;
        }

        public bool removeAlbum(String albumUID)
        {
            AlbumNode album = getAlbumNode(albumUID);

            if (album == null)
                return false;

            _albumList.Remove(album);
            return true;
        }

        public bool renameAlbum(String albumUID, String newAlbumName)
        {
            AlbumNode album = getAlbumNode(albumUID);

            if (album == null)
                return false;

            album._albumName = newAlbumName;
            return true;
        }

        public bool getAlbum(String albumUID, out AlbumXmlData albumData)
        {
            albumData = null;

            foreach (AlbumNode album in _albumList)
            {
                if (albumUID.Equals(album._albumUID))
                {
                    albumData = albumNodeToAlbumXmlData(album);
                    return true;
                }
            }
            return false;
        }

        public void getAlbums(out List<AlbumXmlData> list)
        {
            list = new List<AlbumXmlData>();

            foreach (AlbumNode album in _albumList)
            {
                list.Add(albumNodeToAlbumXmlData(album));
            }
        }


        public bool addImage(String albumUID, String imageFilehash, String imageName, String caption)
        {
            foreach (AlbumNode album in _albumList)
            {
                if (albumUID.Equals(album._albumUID) && !album.contains(imageFilehash))
                {
                    album._images.Add(new ImageNode(imageFilehash, imageName, caption));
                    return true;
                }
            }
            return false;
        }

        public bool removeImage(String albumUID, String imageFilehash)
        {
            foreach (AlbumNode album in _albumList)
            {
                if (albumUID.Equals(album._albumUID))
                {
                    foreach(ImageNode image in album._images)
                    {
                        if(imageFilehash.Equals(image._fileHash))
                        {
                            album._images.Remove(image);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool setImageName(String albumUID, String imageFilehash, String newImageName)
        {
            ImageNode image = getImageNode(albumUID, imageFilehash);
            if (image == null)
                return false;

            image._imageName = newImageName;
            return true;
        }

        public bool setImageCaption(String albumUID, String imageFilehash, String newCaptionName)
        {
            ImageNode image = getImageNode(albumUID, imageFilehash);
            if (image == null)
                return false;

            image._caption = newCaptionName;
            return true;
        }



        public bool contains(String albumUID)
        {
            foreach (AlbumNode album in _albumList)
            {
                if (albumUID.Equals(album._albumUID))
                    return true;
            }
            return false;
        }


        private AlbumNode getAlbumNode(String albumUID)
        {
            foreach (AlbumNode album in _albumList)
            {
                if (albumUID.Equals(album._albumUID))
                {
                    return album;
                }
            }
            return null;
        }

        private ImageNode getImageNode(String albumUID, String imageFilehash)
        {
            foreach (AlbumNode album in _albumList)
            {
                if (albumUID.Equals(album._albumUID))
                {
                    foreach (ImageNode image in album._images)
                    {
                        if (imageFilehash.Equals(image._fileHash))
                        {
                            return image;
                        }
                    }
                }
            }
            return null;
        }


        public bool toXDocument(out XDocument xDoc)
        {
            xDoc = null;
            return false;
        }

        private List<AlbumNode> xDocumentToList(XDocument xDoc)
        {
            return null;
        }



        private AlbumXmlData albumNodeToAlbumXmlData(AlbumNode node)
        {
            List<AlbumImageXmlData> outGoingImages = new List<AlbumImageXmlData>();

            foreach(ImageNode image in node._images)
            {
                outGoingImages.Add(imageNodeToAlbumImageXmlData(image));
            }

            return new AlbumXmlData(node._albumUID, node._albumName, node._thumbFileHash, outGoingImages);

        }

        private AlbumImageXmlData imageNodeToAlbumImageXmlData(ImageNode node)
        {
            return new AlbumImageXmlData(node._fileHash, node._imageName, node._caption);
        }


    } // End of PB_AlbumXmlHandler.
}
