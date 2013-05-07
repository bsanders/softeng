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



        //--------------------------------------------------------
        // By: Bill Sanders
        // Edited Last: 3/28/13
        // 
        // TODO: Examine possible speed up by using a byte-array instead of string comparison in LINQ?
        // TODO2: Add errorreport
        /// <summary>
        /// Retrieves a specific photo instance from a combination of the AlbumID and its hash
        /// </summary>
        /// <param name="albumID">The unique ID of the album</param>
        /// <param name="imageHash">A hex string representation of the hash of the photo</param>
        /// <returns>Returns an xml node of the photo from the AlbumDB, or null.</returns>
        public XElement getImageNode(Guid albumID, string imageHash, XElement albumsRootXml,  XElement imagesRootXml)
        {
            XElement imageNode = null;
            try
            {
                // Try to find a photo hash in this album by hash.
                // Note: this may be working inefficiently.
                // Join every picture element from both databases, where the pictures have the same sha1
                // Of those, we only care about the ones where the picture has the sha1 we're looking for
                // finally, of those, we only care about the cases where the that match exists in the album we're interested in.
                // This query then returns a single xml instance matching these criteria,
                // OR returns null if 0 are found
                // OR throws an exception if more than 1 are found
                imageNode = (from picDB in imagesRootXml.Elements("picture")
                                  join picAlbDB in albumsRootXml.Descendants("picture")
                                  on (string)picDB.Attribute("sha1") equals (string)picAlbDB.Attribute("sha1")
                                  where (string)picDB.Attribute("sha1") == imageHash
                                       && (Guid)picAlbDB.Ancestors("album").Single().Attribute("uid") == albumID
                                  select picAlbDB).SingleOrDefault();
            }
            catch // We only get here if the database is already messed up (two of the same photo in an album)
            {
                throw;
            }
            // If the query returned a single node
            return imageNode;
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
            albumImageNode =  getAlbumImageNodeFromAlbumXml(albumUID, inAlbumId, albumRootXml);
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


        /// By: Bill Sanders
        /// Edited Ryan Causey(4/5/13)
        /// This function replaces the function util_convertPhotoNodeToComplexPhotoData() below.
        /// <summary>
        /// Combines the data from both databases for a specific photo instance.
        /// </summary>
        /// <param name="errorReport"></param>
        /// <param name="albumImageNode">An XElement of a picture from the Album DB</param>
        /// <param name="albumUID">The unique ID number of the album the photo instance is in</param>
        /// <returns>Returns a ComplexPhotoData object, or null if the object could not be created.</returns>
        public ComplexPhotoData getComplexPhotoDataFromAlbumImageNode(XElement albumImageNode, XElement _imagesRootXml)
        {
            // We have all the data we need from the AlbumDB,
            // but the PhotoDB has important data as well.
            XElement imageNode = null;
            if (!getImageNodeFromImageXml((string)albumImageNode.Attribute("sha1"), _imagesRootXml, out imageNode))
                return null;

            ComplexPhotoData imageData = new ComplexPhotoData();

            //TRANSFER ALL DATA TO THE DATA CLASS HERE.
            try
            {
                // PhotoDB data
                imageData.UID = (int)imageNode.Attribute("uid");
                imageData.hash = stringToByteArray((string)imageNode.Attribute("sha1"));
                imageData.refCount = (int)imageNode.Attribute("refCount");
                imageData.fullPath = imageNode.Element("filePath").Value;
                imageData.lgThumbPath = imageNode.Element("lgThumbPath").Value;
                imageData.addedDate = stringToDateTime(imageNode.Element("dateAdded").Value);

                imageData.extension = (String)imageNode.Element("filePath").Attribute("extension");

                // AlbumDB data
                imageData.idInAlbum = (int)albumImageNode.Attribute("idInAlbum");
                imageData.name = albumImageNode.Element("name").Value;
                if (imageData.name == string.Empty)
                {
                    string defaultPhotoName = albumImageNode.Parent.Parent.Element("albumName").Value
                        + " "
                        + Settings.DefaultImageName
                        + " "
                        + imageData.idInAlbum;
                    imageData.name = defaultPhotoName;
                    albumImageNode.Element("name").Value = defaultPhotoName;
                }
                imageData.caption = albumImageNode.Element("caption").Value;
            }
            catch (Exception)
            {
                return null;
            }

            return imageData;
        }



        /// By: Julian Nguyen
        /// Edited Julian Nguyen(5/7/13)
        /// <summary>
        /// Will take a time in String form and return a datetime.
        /// </summary>
        /// <param name="sDate"></param>
        /// <returns></returns>
        private DateTime stringToDateTime(String sDate)
        {
            DateTime date = DateTime.MinValue;

            if (!DateTime.TryParse(sDate, out date))
            {
                return DateTime.MinValue;
            }

            return date;
        }

        /// By: Bill Sanders
        /// Edited Julian Nguyen(5/7/13)
        /// Should be parallelizeable 
        /// <summary>
        /// Convert a HexString (such as a SHA1 hash) to an array of Bytes
        /// </summary>
        /// <param name="hexStr"></param>
        /// <returns>a byte array</returns>
        private byte[] stringToByteArray(String hexStr)
        {
            int NumberChars = hexStr.Length / 2;
            byte[] bytes = new byte[NumberChars];
            StringReader sr = new StringReader(hexStr);
            for (int i = 0; i < NumberChars; i++)
            {
                bytes[i] = Convert.ToByte(new string(new char[2] { (char)sr.Read(), (char)sr.Read() }), 16);
            }
            sr.Dispose();
            return bytes;
        }


        /// By: Ryan Moe
        /// Edited Julian Nguyen(5/7/13)
        /// <summary>
        /// Test of an album name is unique.
        /// </summary>
        /// <param name="albumName"></param>
        /// <param name="albumsRootXml"></param>
        /// <returns></returns>
        public Boolean isAlbumNameUnique(String albumName, XElement albumsRootXml)
        {
            try
            {
                //try and find a matching album name.
                //throws exception if we find NO matching names.
                (from c in albumsRootXml.Elements("album")
                 where (String)c.Element("albumName") == albumName
                 select c).First();
            }
            //we didn't find a matching name, success!
            catch
            {
                return true;
            }
            return false;
        }


        /// By ?? 
        /// Edited: Julian Nguyen(5/7/13)
        /// <summary>
        /// Looks up the photo by hash in the Photos XML database, returns the number of appearances in the Album database.
        /// </summary>
        /// <param name="hash">A string representing the hash value of the file.</param>
        /// <returns>An integer representing the number of times this photo appears in the the Album database</returns>
        public int getPhotoRefCount(string hash, XElement imagesRootXml)
        {
            ErrorReport errorReport = new ErrorReport();

            XElement imageNode = getImageNodeFromImageXml(hash,  imagesRootXml);

            if (imageNode == null)
            {
                return 0;
            }
            else
            {
                return (int)imageNode.Attribute("refCount");
            }
        }

    } // End of PhotoBomb_Xml.
}
