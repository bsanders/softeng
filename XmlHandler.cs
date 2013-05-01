/*
 * Change Log:
 * 4/30/13 Julian Nguyen
 * The XmlHandler class was added to remove xml handling from the PhotoBomb class.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace SoftwareEng
{
    class XmlHandler
    {
        // TODO: What are you doing? change this. 
        private Properties.Settings Settings = Properties.Settings.Default;


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
            catch (IOException e)
            {
                xmlRootElement = null;
                return false;
            }

        }


    } // End of XmlHandler. 
}
