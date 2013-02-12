using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace SoftwareEng
{
    class XmlParser
    {

        //--------------------------------------------------------------
        private List<XElement> searchForElement(XElement parent, string lookingFor)
        {
            List<XElement> _list = new List<XElement>();
            foreach (XElement subElement in parent.Descendants(lookingFor))
            {
                _list.Add(subElement);
            }//foreach
            return _list;
        }//method
        //--------------------------------------------------------------
        private List<XElement> searchForElement(XDocument parent, string lookingFor)
        {
            List<XElement> _list = new List<XElement>();
            foreach (XElement subElement in parent.Descendants(lookingFor))
            {
                _list.Add(subElement);
            }//foreach
            return _list;
        }//method
        //--------------------------------------------------------------



    }//class

    //------------------------------------------------
    //------------------------------------------------
    //------------------------------------------------


}//namespace
