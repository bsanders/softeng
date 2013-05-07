using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.IO;

namespace SoftwareEng
{
    class PhotoBomb_Xml
    {
        // TODO: What are you doing? change this. 
        private Properties.Settings Settings = Properties.Settings.Default;

        public PhotoBomb_Xml()
        {

        }


        /// By Julian Nguyen
        /// Edited: Julian Nguyen(4/30/13)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathToXml"></param>
        /// <param name="xmlRootElement"></param>
        /// <returns></returns>
        public bool loadXmlRootElement(String pathToXml, out XElement xmlRootElement)
        {
            try
            {
                xmlRootElement = XDocument.Load(pathToXml).Element(Settings.XMLRootElement);
                return true;
            }
            catch (IOException)
            {
                xmlRootElement = null;
                return false;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageHash"></param>
        /// <param name="imagesRootXml"></param>
        /// <param name="imageNode"></param>
        /// <returns></returns>
        public bool getImageNodeFromImageXml(string imageHash, XElement imagesRootXml, out XElement imageNode)
        {
            imageNode = getImageNodeFromImageXml(imageHash, imagesRootXml);
            return imageNode != null;
        }

        /// By: Bill Sanders
        /// Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// Query the Photo DB for a photo by hash
        /// </summary>
        /// <param name="error"></param>
        /// <param name="imageHash"></param>
        /// <returns>An XElement object representing the photo in the PhotoDB</returns>
        public XElement getImageNodeFromImageXml(string imageHash, XElement imagesRootXml)
        {
            try
            {
                //for(from) every c in the database's children (all albums),
                //see if it's attribute uid is the one we want,
                //and if so return the first instance of a match.
                return (from c in imagesRootXml.Elements()
                                    where (string)c.Attribute("sha1") == imageHash
                                    select c).Single(); // NOTE: this will throw error if more than one OR none at all.
            }
            catch (Exception ex)
            {
                 // There were multiple pictures...
                if (ex is InvalidOperationException || ex is ArgumentNullException)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="albumUID"></param>
        /// <param name="albumsRootXml"></param>
        /// <param name="albumNode"></param>
        /// <returns></returns>
        public bool getAlbumNodeFromAlbumXml(Guid albumUID, XElement albumsRootXml, out XElement albumNode)
        {
            albumNode = getAlbumNodeFromAlbumXml(albumUID, albumsRootXml);
            return albumNode != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="albumUID"></param>
        /// <param name="albumsRootXml"></param>
        /// <returns></returns>
        public XElement getAlbumNodeFromAlbumXml(Guid albumUID, XElement albumsRootXml)
        {
            try
            {
                //find and return an album whos uid is the one we are looking for.
                //Throws exception if none or more than one match is found.
                return (from c in albumsRootXml.Elements("album")
                        where (Guid)c.Attribute("uid") == albumUID
                        select c).Single();
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="albumNode"></param>
        /// <param name="inAlbumId"></param>
        /// <param name="albumImageNode"></param>
        /// <returns></returns>
        public bool getAlbumImageNodeFromAlbumNode(XElement albumNode, int inAlbumId, out XElement albumImageNode)
        {
            albumImageNode = getAlbumImageNodeFromAlbumNode(albumNode, inAlbumId);
            return albumImageNode != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="albumNode"></param>
        /// <param name="inAlbumId"></param>
        /// <returns></returns>
        public XElement getAlbumImageNodeFromAlbumNode(XElement albumNode, int inAlbumId)
        {
            try
            {
                // Search through a specific album for a specific photo by ID
                // Return an (XElement) picture with the matching uid if it exists
                // Throws exception if it doesn't find exactly 1 match <--- this is not true!!!
                return (from c in albumNode.Element("albumPhotos").Elements("picture")
                        where (int)c.Attribute("idInAlbum") == inAlbumId
                        select c).SingleOrDefault();
            }
            catch(Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="albumUID"></param>
        /// <param name="inAlbumId"></param>
        /// <param name="albumRootXml"></param>
        /// <param name="albumImageNode"></param>
        /// <returns></returns>
        public bool getAlbumImageNodeFromAlbumXml(Guid albumUID, int inAlbumId, XElement albumRootXml, out XElement albumImageNode)
        {
            albumImageNode = getAlbumImageNodeFromAlbumXml(albumUID, inAlbumId, albumRootXml);
            return albumImageNode != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="albumUID"></param>
        /// <param name="inAlbumId"></param>
        /// <param name="albumsRootXml"></param>
        /// <returns></returns>
        public XElement getAlbumImageNodeFromAlbumXml(Guid albumUID, int inAlbumId, XElement albumsRootXml)
        {
            XElement albumNode = getAlbumNodeFromAlbumXml(albumUID, albumsRootXml);
            if (albumNode == null)
                return null;
            return getAlbumImageNodeFromAlbumNode(albumNode, inAlbumId);
        }


        public void addAlbumNodeToAlbumsXml(Guid albumUID, int inAlbumThumbID, String thumbnailPath, String albumName, XElement albumsRootXml)
        {

            //construct the object we will be adding to the database.
            XElement newAlbum = newAlbumNode(albumUID, inAlbumThumbID, thumbnailPath, albumName);
            //add to the database in memory.
            albumsRootXml.Add(newAlbum);

        }//method



        public bool removeNode(XElement node)
        {
            try
            {
                node.Remove();
                return true;
            } 
            catch(InvalidOperationException)
            {
                return false;
            }
        }


        public void newAlbumNode()
        {
        }


        public XElement newAlbumNode(Guid albumUID, int inAlbumThumbID, String thumbnailPath, String albumName)
        {
            return new XElement("album",
                new XAttribute("uid", albumUID),
                new XElement("thumbnailPath", new XAttribute("thumbAlbumID", inAlbumThumbID), thumbnailPath),
                new XElement("albumName", albumName),
                new XElement("albumPhotos"));
        }

        public XElement newAlbumImageNode(int inAlbumID, String imageHash, String inAlbumImageName, String imageCaption)
        {
            return new XElement("picture",
                new XAttribute("idInAlbum", inAlbumID),
                new XAttribute("sha1", imageHash),
                new XElement("name", inAlbumImageName),
                new XElement("caption", imageCaption));
        }

        public void newImageNode()
        {
        }



        public bool setAlbumNodeName(XElement albumNode, String newAlbumName)
        {
            try
            {
                albumNode.Element("albumName").Value = newAlbumName;
                return true;             
            }
            catch
            {
                return false;
            }
        }


        public bool setAlbumImageNodeName(XElement imageNode, String newImageName)
        {
            try
            {
                imageNode.Element("name").Value = newImageName;
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// By: Bill Sanders
        /// Edited Julian Nguyen(4/1/13)
        /// <summary>
        /// Check to see if a photo name is unique to an album.
        /// </summary>
        /// <param name="imageName">The name to check</param>
        /// <param name="albumNode">The XElement albumDB node to check in</param>
        /// <returns>Returns true if the photo name is unique to that album</returns>
        public Boolean isImageNameUniqueToAlbum(String imageName, XElement albumNode)
        {
            try
            {
                //try and find a matching photo name.
                //throws exception if we find NO matching names.
                (from c in albumNode.Descendants("picture")
                 where (String)c.Element("name") == imageName
                 select c).First();
                return false;
            }
            //we didn't find a matching name, success!
            catch (Exception)
            {
                return true;
            }
        }

    } // End of PhotoBomb_Xml.
}
