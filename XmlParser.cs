/**
 * By: Ryan Moe
 * Edited Last: 
 * 
 * These are some backend tools to parse xml files.
 **/

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
        //By: Ryan Moe
        //Edited Last: 
        //
        //Search an XElement's decendants.
        //PARAM 1 = the parent XElement to search from.
        //PARAM 2 = the children element's name to search the parent for.
        //RETURNS = a list of XElements that you were looking for.
        public List<XElement> searchForElements(XElement parent, string lookingFor)
        {
            List<XElement> _list = new List<XElement>();
            foreach (XElement subElement in parent.Elements(lookingFor))
            {
                _list.Add(subElement);
            }//foreach
            return _list;
        }//method
        //--------------------------------------------------------------
        //By: Ryan Moe
        //Edited Last: 
        //
        //Search an XDocuments's decendants.
        //PARAM 1 = the parent XDocument to search from.
        //PARAM 2 = the children element's name to search the parent for.
        //RETURNS = a list of XElements that you were looking for.
        public List<XElement> searchForElements(XDocument parent, string lookingFor)
        {
            List<XElement> _list = new List<XElement>();
            foreach (XElement subElement in parent.Elements(lookingFor))
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
