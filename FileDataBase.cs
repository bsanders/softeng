using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography; //Namespace for SHA1
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;

namespace SoftwareEng
{
    class FileDataBase
    {
        private const String THUMBS_DB_PATH = @"thumbs_db\";



        private String _imageslibraryDirPath;
        private String _thumbslibraryDirPath;



        public FileDataBase()
        {
            _imageslibraryDirPath = String.Empty;
            _thumbslibraryDirPath = String.Empty;
        }


        /// By: Julian Nguyen
        /// Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imagelibraryDirPath"></param>
        public FileDataBase(String imagelibraryDirPath)
        {
            _imageslibraryDirPath = imagelibraryDirPath;
            _thumbslibraryDirPath = Path.Combine(imagelibraryDirPath, THUMBS_DB_PATH);

        }

        /// By: Julian Nguyen
        /// Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ReportStatus areDirectoriesThere()
        {
            if (!Directory.Exists(_imageslibraryDirPath))
                return ReportStatus.INVALID_IMAGE_DIR;


            if (!Directory.Exists(_thumbslibraryDirPath))
                return ReportStatus.INVALID_THUMB_DIR;

            return ReportStatus.SUCCESS;
        }


        /// By: Julian Nguyen
        /// Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imagelibraryDirPath"></param>
        /// <returns></returns>
        public ReportStatus setImagelibraryDirPath(String imagelibraryDirPath)
        {
            _imageslibraryDirPath = imagelibraryDirPath;
            _thumbslibraryDirPath = Path.Combine(imagelibraryDirPath, THUMBS_DB_PATH);
            return areDirectoriesThere();
        }


        public String[] getImages()
        {
            return Directory.GetFiles(_imageslibraryDirPath);
        }

        public String[] getThumbs()
        {
            return Directory.GetFiles(_thumbslibraryDirPath);
        }


        public ReportStatus purgeImages()
        {
            if (purgeDir(_imageslibraryDirPath))
                return ReportStatus.SUCCESS;

            return ReportStatus.FAILURE;
        }

        public ReportStatus purgeThumbs()
        {
            if (purgeDir(_thumbslibraryDirPath))
                return ReportStatus.SUCCESS;

            return ReportStatus.FAILURE;
        }


        /// By: Julian Nguyen
        /// Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFileNPath"></param>
        /// <returns></returns>
        public ReportStatus addToImageslibrary(String sourceFileNPath)
        {
            if (copyFile(sourceFileNPath, _imageslibraryDirPath))
                return ReportStatus.SUCCESS;

            return ReportStatus.FAILURE;
        }

        /// By: Julian Nguyen
        /// Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFileNPath"></param>
        /// <returns></returns>
        public ReportStatus removeFromImageslibrary(String sourceFileNPath)
        {
            if (removeFile(sourceFileNPath))
                return ReportStatus.SUCCESS;


            return ReportStatus.FAILURE;
        }

        /// By: Julian Nguyen
        /// Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFileNPath"></param>
        /// <returns></returns>
        public ReportStatus addToThumbslibrary(String sourceFileNPath)
        {
            if (copyFile(sourceFileNPath, _thumbslibraryDirPath))
                return ReportStatus.SUCCESS;


            return ReportStatus.SUCCESS;
        }

        /// By: Julian Nguyen
        /// Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFileNPath"></param>
        /// <returns></returns>
        public ReportStatus removeFromThumbslibrary(String sourceFileNPath)
        {
            if (removeFile(sourceFileNPath))
                return ReportStatus.SUCCESS;

            return ReportStatus.FAILURE;
        }


        /// By: Julian Nguyen
        /// Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFileNPath"></param>
        /// <param name="destFileNPath"></param>
        /// <returns></returns>
        private bool copyFile(String sourceFileNPath, String destFileNPath)
        {
            try
            {
                File.Copy(sourceFileNPath, destFileNPath, true);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        /// By: Julian Nguyen
        /// Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFileNPath"></param>
        /// <returns></returns>
        private bool removeFile(String sourceFileNPath)
        {
            try
            {
                File.Delete(sourceFileNPath);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }


        private bool purgeDir(String dirNPath)
        {
            try
            {
                // Remove the dir and then just add it back. 
                Directory.Delete(dirNPath, true);
                Directory.CreateDirectory(dirNPath);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }



        /// By: Julian Nguyen
        /// Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullFilePathList"></param>
        /// <returns></returns>
        public List<KeyValuePair<String, byte[]>> getFileHash(List<String> fullFilePathList) 
        {
            List<KeyValuePair<String, byte[]>> filesAndHashList = new List<KeyValuePair<string, byte[]>>();

            foreach (String filePath in fullFilePathList)
            {
                filesAndHashList.Add(new KeyValuePair<string, byte[]>(filePath, getFileHash(filePath)));
            }
            return filesAndHashList;
        }


        
        /// By: Bill Sanders
        /// Edited Julian Nguyen(5/1/13)
        /// BS: Should be parallelizeable 
        /// <summary>
        /// Compute the SHA1 Hash of a file
        /// </summary>
        /// <param name="fullFilePath">The path and file name of the file to SHA1</param>
        /// <returns>A byte array representation of the SHA1</returns>
        public byte[] getFileHash(string fullFilePath)
        {
            // fileHash will hold a byte array representation of the SHA1 hash.
            byte[] fileHash = null;
            FileStream fs = null;
            BufferedStream bs = null;

            // Read the file in as a stream
            try
            {
                fs = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read);
                bs = new BufferedStream(fs);
                // SHA1Managed is the .NET class that actually holds the hashing logic
                SHA1Managed sha1 = new SHA1Managed();
                fileHash = sha1.ComputeHash(bs);
            }
            // If there's some file error, just leave it set to null.
            catch (IOException)
            {
                fileHash = null;
            }
            finally
            {
                // Do some house cleaning. 
                if(fs != null)
                    fs.Close(); 
                if(bs != null)
                    bs.Close();
            }

            return fileHash;
        }


        //we init this once so that if the function is repeatedly called
        //it isn't stressing the garbage man
        private static Regex r = new Regex(":");

        /// By: http://stackoverflow.com/questions/180030/how-can-i-find-out-when-a-picture-was-actually-taken-in-c-sharp-running-on-vista
        /// Edited Julian Nguyen(5/4/13)
        /// <summary>
        /// retrieves the datetime WITHOUT loading the whole image
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public PropertyItem[] getImageProperty(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            Image myImage = Bitmap.FromStream(fs, false, false);

            return myImage.PropertyItems;

        }


        private Image openImage(String path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                fs.Close();
                return myImage;
            }
        }

    } // End of FileDataBase.
}
