/*
 * Change Log:
 * Julian Nguyen(4/29/13)
 * This file was made with the class XmlLoader<T>
 * Julian Nguyen(4/23/13)
 * A constructor was added.
 * All functions are now not static.
 * All functions are now generic and not the class.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace SoftwareEng
{

    /// By Julian Nguyen
    /// Edited: Julian Nguyen(4/29/13)
    /// <summary>
    /// 
    /// </summary>
    class XmlLoader
    {
        ///
        /// By Julian Nguyen
        /// Edited: Julian Nguyen(4/23/13)
        /// <summary>
        /// A constructor. 
        /// </summary>
        public XmlLoader()
        {
            // Do nothing.
        }



        /// By Julian Nguyen
        /// Edited: Julian Nguyen(4/30/13)
        /// <summary>
        /// Load a list from file.
        /// </summary>
        /// <typeparam name="T">The type to be loaded.</typeparam>
        /// <param name="filePath">The path to file to loaded.</param>
        /// <param name="list">The file will be loaded into this list.</param>
        public void loadToList<T>(String @filePath, out List<T> list)
        {
            list = null;
            FileStream readFileStream = null;

            try
            {
                // Create a new file stream for reading the XML file
                readFileStream = new FileStream(@filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                XmlSerializer serializerObj = new XmlSerializer(typeof(List<T>));
                list = (List<T>)serializerObj.Deserialize(readFileStream);
            }
            catch
            {
                throw; // Just pass it on.
            }
            finally
            {
                // Try to do some house cleaning. 
                if (readFileStream != null)
                    readFileStream.Close();
            }
        }


        /// By Julian Nguyen
        /// Edited: Julian Nguyen(4/23/13)
        /// <summary>
        /// Will save the list to file.
        /// </summary>
        /// <typeparam name="T">The type that the list holds.</typeparam>
        /// <param name="filePath">The path and name where the list will be saved.</param>
        /// <param name="list">The list to be saved.</param>
        public void saveToFile<T>(String filePath, List<T> list)
        {
            TextWriter writeFileStream = null;
            try
            {
                // Create a new file stream to write the serialized object to a file
                writeFileStream = new StreamWriter(@filePath);
                XmlSerializer serializerObj = new XmlSerializer(typeof(List<T>));
                serializerObj.Serialize(writeFileStream, list);
            }
            catch
            {
                throw; // Just pass it on.
            }
            finally
            {
                // Try to do some house cleaning. 
                if (writeFileStream != null)
                    writeFileStream.Close();
            }
        }


    } // End of XmlLoader<T>.
}
