using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.IO;


/*
 * Change Log:
 * 4/20/13 Julian Nguyen: New class based on the function found in PhotoBombBackend.cs.
 */

namespace SoftwareEng
{
    /// <summary>
    /// This class handles the IO to the PB database.
    /// 
    /// This is only file IO.
    /// 
    /// </summary>
    class PB_Database_FileIO
    {

        /// <summary>
        /// TODO : 
        /// </summary>
        private Properties.Settings _settings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        public PB_Database_FileIO(Properties.Settings settings)
        {
            _settings = settings;
        }


        /// <summary>
        /// By Julian Nguyen
        /// Edited Last: 4/20/13 Julian Nguyen
        /// 
        /// This will open a XML file.
        /// If fails to, then it will return null.
        /// This will test if the file exists and if the file is locked.
        /// </summary>
        /// <param name="pathToXML">The path to XML to be opened.</param>
        /// <returns></returns>
        public XElement openXMLFile(FileInfo xmlFile)
        {
            if(!xmlFile.Exists || isFileLocked(xmlFile))
                return null; 

            try
            {
                return XDocument.Load(xmlFile.ToString()).Element(_settings.XMLRootElement);
            }
            catch (IOException)
            {
                return null;
            } 
        }

        /// <summary>
        ///  By Julian Nguyen, code idea from Ryan Moe.
        ///  Edited Last: 4/20/13 Julian Nguyen
        ///  
        /// </summary>
        /// <param name="rootXML">The XML obj to be saved to file.</param>
        /// <param name="xmlFileInfo">The path to where the xml will be saved. </param>
        public void saveXMLFile(XElement rootXML, FileInfo xmlFileInfo)
        {
            // TODO: add some checks here. 
            rootXML.Document.Save(xmlFileInfo.ToString());
        }


        /// <summary>
        /// By http://stackoverflow.com/questions/876473/is-there-a-way-to-check-if-a-file-is-in-use
        /// Edited Last: 4/20/13 Julian Nguyen
        /// 
        /// This will test if the file is in use.
        /// 
        /// </summary>
        /// <param name="file">The FileInfo to be tested.</param>
        /// <returns>If the file is locked or not. </returns>
        private bool isFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }


        /// <summary>
        /// By Julian Nguyen, code idea from Bill.
        /// Edited Last: 4/20/13 Julian Nguyen
        /// 
        /// This is copy file from one plase to another.
        /// 
        /// </summary>
        /// <param name="files">The List of files to copied. </param>
        /// <param name="dest">The dir were the file will be copied too. </param>
        /// <param name="overwrite">Will codpy over the files with the same name? </param>
        /// <returns>The list of Files that could not be copied or null if dest is not a dir.</returns>
        public List<FileInfo> copyFilesTo(List<FileInfo> files, DirectoryInfo dest, bool overwrite)
        {
            if(!dest.Exists)
                return null;

            List<FileInfo> failedToCopyList = new List<FileInfo>();
            String destPath = dest.ToString();

            foreach(FileInfo file in files)
            {
                if(!file.Exists)
                {
                    failedToCopyList.Add(file);
                }
                else 
                {
                    try
                    {
                        File.Copy(file.ToString(), destPath, overwrite); 
                    }
                    catch (IOException e)
                    {
                        failedToCopyList.Add(file);
                    }
                }
            }
            return failedToCopyList;
        }


        /// <summary>
        /// By Julian Nguyen, code idea from Bill.
        /// Edited Last: 4/20/13 Julian Nguyen
        /// 
        /// This will remove a list of files for the filesystem.
        /// 
        /// TDDO: maybe add a dir to test that the files are only being removed from one dir. 
        /// 
        /// </summary>
        /// <param name="files">The list of files to be removed</param>
        /// <returns>The list of Files that could not be removed.</returns>
        public List<FileInfo> removeFilesFrom(List<FileInfo> files)
        {
            List<FileInfo> failedToRemoveList = new List<FileInfo>();

            foreach(FileInfo file in files)
            {
                if(!file.Exists || isFileLocked(file))
                {
                    failedToRemoveList.Add(file);
                } 
                else 
                {
                    try 
                    {
                        File.Delete(file.ToString());
                    } 
                    catch (IOException e)
                    {
                        failedToRemoveList.Add(file);
                    }
                }
            } // End of loop. 

            return failedToRemoveList;
        }



        /// <summary>
        /// By Julian Nguyen
        /// Edited Last: 4/20/13 Julian Nguyen
        /// 
        /// This will get all the files in a dir.
        /// 
        /// </summary>
        /// <param name="dir">The dir where the files will come from.</param>
        /// <returns>The list of FileInfo of the files in the dir.</returns>
        public List<FileInfo> getFilesOfDir(DirectoryInfo dir)
        {
            if(!dir.Exists)
                return null;

            List<FileInfo> listOfFileInfo = new List<FileInfo>();

            FileInfo[] fileArray = dir.GetFiles();
            
            foreach(FileInfo fileInfo in fileArray)
            {
                listOfFileInfo.Add(fileInfo);
            }

            return listOfFileInfo;
        }

    } // End of the PB_Database class. 
}
