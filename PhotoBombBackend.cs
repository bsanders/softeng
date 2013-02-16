/**
 * This is the backend for the PhotoBomb program.
 * These functions are NOT to be used by a frontend (gui).
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SoftwareEng
{
    partial class PhotoBomb
    {
        
        //xml parsing utils.
        private XmlParser xmlParser;
        //------------------------------------------------------
        //The XML in memory for the albumbs.
        //Add new vars here if we get more xmls.
        private XDocument _albumsXDocs;
        private XDocument _picturesXDocs;

    }
}
