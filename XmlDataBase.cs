﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareEng
{

    /// By Julian Nguyen
    /// Edited: Julian Nguyen(4/30/13)
    /// <summary>
    /// 
    /// </summary>
    class XmlDataBase
    {
        // Tool for loading and saving a xml file.
        private XmlLoader _xmlLoader;

        // Class data structures 
        private List<AlbumXmlData> _allAlbumsList; 
        private Dictionary<byte[], ImageXmlData> _allHashToImageMap;

        /// By Julian Nguyen
        /// Edited: Julian Nguyen(4/30/13)
        /// <summary>
        /// 
        /// </summary>
        public XmlDataBase()
        {
            _xmlLoader = new XmlLoader();

            _allAlbumsList = new List<AlbumXmlData>();
            _allHashToImageMap = new Dictionary<byte[], ImageXmlData>();
        }

        /// By Julian Nguyen
        /// Edited: Julian Nguyen(4/30/13)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="albumsXmlPath"></param>
        /// <param name="imagesXmlPath"></param>
        public XmlDataBase(String albumsXmlPath, String imagesXmlPath)
        {
            _xmlLoader = new XmlLoader();

            // Load the images from the XML file.
            // A list of all images in the XML,
            List<ImageXmlData> allImagesList_tmp = null;
            // Load the images from the XML.
            _xmlLoader.loadToList<ImageXmlData>(imagesXmlPath, out allImagesList_tmp);
            // Take the list of Images and make it into a Map.
            this.listToMap(allImagesList_tmp, out _allHashToImageMap);

            // Load the albums from the XML file.
            _xmlLoader.loadToList<AlbumXmlData>(albumsXmlPath, out _allAlbumsList);
        }




        public void saveImagesToXml(String pathToFile)
        {
            // To List the images Map and save it to file.
            _xmlLoader.saveToFile<ImageXmlData>(pathToFile, new List<ImageXmlData>(_allHashToImageMap.Values.ToList()));
        }

        public void saveAlbumsToXml(String pathToFile)
        {
            // To List the images Map and save it to file.
            _xmlLoader.saveToFile<AlbumXmlData>(pathToFile, _allAlbumsList);
        }

        public void getAllAlbums(out List<AlbumXmlDataReadOnly> albums)
        {
            albums = new List<AlbumXmlDataReadOnly>();
            foreach (AlbumXmlData albumData in _allAlbumsList)
            {
                albums.Add(new AlbumXmlDataReadOnly(albumData));
            }
        }


        public bool getAnAlbum(int uid, out AlbumXmlDataReadOnly albumData)
        {
            foreach (AlbumXmlData aData in _allAlbumsList)
            {
                if(aData._albumUID.Equals(uid))
                {
                    albumData = new AlbumXmlDataReadOnly(aData);
                    return true;
                }
            }

            albumData = null;
            return false;
        }



        // Image stuff -->

        //public bool set



        public bool setImageName(int albumUID, byte[] imageHash, String newImageName)
        {
            AlbumXmlData albumData = null;
            if (!getAnAlbum(albumUID, out albumData))
                return false;

            AlbumImageXmlData imageDate = null;
            if (!getAnImage(albumData, imageHash, out imageDate))
                return false;

            imageDate._imageName = newImageName;
            return true;
        }

        public bool setImageCaption(int albumUID, byte[] imageHash, String newImageCaption)
        {
            AlbumXmlData albumData = null;
            if (!getAnAlbum(albumUID, out albumData))
                return false;

            AlbumImageXmlData imageDate = null;
            if (!getAnImage(albumData, imageHash, out imageDate))
                return false;

            imageDate._imageName = newImageCaption;
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="albumsXmlPath"></param>
        /// <param name="imagesXmlPath"></param>
        /// <returns></returns>
        public bool reLoadXmlAndOverwrite(String albumsXmlPath, String imagesXmlPath)
        {
            // Load the images from the XML file.
            // A list of all images in the XML,
            List<ImageXmlData> allImagesList_tmp = null;
            // Load the images from the XML.
            _xmlLoader.loadToList<ImageXmlData>(imagesXmlPath, out allImagesList_tmp);

            List<AlbumXmlData> allAlbumsList_tmp = null;
            Dictionary<byte[], ImageXmlData> allHashToImageMap_tmp = null; 


            // Load the albums from the XML file.
            _xmlLoader.loadToList<AlbumXmlData>(albumsXmlPath, out allAlbumsList_tmp);

            // Take the list of Images and make it into a Map.
            this.listToMap(allImagesList_tmp, out allHashToImageMap_tmp);


            // Only overwrite the data structures only if all files where load okay!
            if (allAlbumsList_tmp != null && allHashToImageMap_tmp != null)
            {
                _allAlbumsList = allAlbumsList_tmp;
                _allHashToImageMap = allHashToImageMap_tmp;
                return true;
            }
            return false;
        }







        /// By Julian Nguyen
        /// Edited: Julian Nguyen(4/30/13)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="map"></param>
        private void listToMap(List<ImageXmlData> list, out Dictionary<byte[], ImageXmlData> map)
        {
            // Get a new Dictionary
            map = new Dictionary<byte[], ImageXmlData>();

            // There is a  linq solution, but ain't nobody got time for that.
            foreach (ImageXmlData imageData in list)
            {
                // This will overwrite data for days.
                map.Add(imageData._hashValue, imageData);
            }
        }

        private bool getAnAlbum(int uid, out AlbumXmlData albumData)
        {
            foreach (AlbumXmlData aData in _allAlbumsList)
            {
                if (aData._albumUID.Equals(uid))
                {
                    albumData = aData;
                    return true;
                }
            }

            albumData = null;
            return false;
        }


        private bool getAnImage(AlbumXmlData albumData, byte[] imageHash, out AlbumImageXmlData imageData)
        {
            foreach (AlbumImageXmlData iData in albumData._images)
            {
                if(iData._imageHashValue.Equals(imageHash))
                {
                    imageData = iData;
                    return true;
                }
            }
            imageData = null;
            return false;
        }



    } // End of XmlDataBase.
}
