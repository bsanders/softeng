using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace SoftwareEng
{
    class PhotoBomb_Xml
    {

        public PhotoBomb_Xml()
        {

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

        public XElement getAlbumImageNodeFromAlbumXml(Guid albumUID, int inAlbumId, XElement albumsRootXml)
        {
            XElement albumNode = getAlbumNodeFromAlbumXml(albumUID, albumsRootXml);
            if (albumNode == null)
                return null;
            return getAlbumImageNodeFromAlbumNode(albumNode, inAlbumId);
        }
        



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

    } // End of PhotoBomb_Xml.
}
