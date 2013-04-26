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
    /// This class is to handle the XML in memory.
    /// 
    /// </summary>
    class PB_DatabaseXML
    {

        private class albumNode
        {
            private XElement _albumElement;
            private String _uid;
           


        }


        private XDocument _albumXML;
        private XDocument _imageXML;

        /// <summary>
        /// By: Julian Nguyen (4/25/13)
        /// Last Changed by: Julian Nguyen (4/25/13)
        /// 
        /// Class constructor.
        /// 
        /// </summary>
        /// <param name="albumXML">The XDocument for the album XML</param>
        /// <param name="imageXML">The XDocument image XML</param>
        public PB_DatabaseXML(XDocument albumXML, XDocument imageXML)
        {
            _albumXML = albumXML;
            _imageXML = imageXML;
        }




        public void album_add(String albumName)
        {

            
        }

        public void album_remove(String albumName)
        {

            
        }

        public void album_rename(String albumName)
        {

            
        }

        public void album_get(String uid)
        {


    
        }
         


        public void image_add()
        {
            
        }

        public void image_remove()
        {
            
        }

        public void image_rename()
        {
            
        }

        public void image_setCaption()
        {
            
        }





        public XDocument AlbumXML
        {
            get { return _albumXML; }
        }

        public XDocument ImageXML
        {
            get { return _imageXML; }
        }

    } //End of PB_DatabaseXML.
}
