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
        // TODO: What are you doing, change this. 
        private Properties.Settings Settings = Properties.Settings.Default;


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


    }
}
