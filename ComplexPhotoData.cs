using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;
using System.IO;

//--------------------------------
//
//
//Edited Last By: Ryan Causey
//Edited Date: 4/5/13


/*
 * More complex photo data returned by functions like getPhotoDataByUID().
 * 
 * Changelog:
 * m/dd/yy Class by Ryan Moe 
 * 4/5/13  Edited Last By: Ryan Causey
 * 4/19/13 Julian Nguyen: Removed from Photobomb.cs and added to this file. 
 *                        The public scope modifier was added to class.
 */

namespace SoftwareEng
{
    public class ComplexPhotoData : INotifyPropertyChanged
    {
        //the name of the picture in the album, displayed by the gui

        private int _UID;
        private int _idInAlbum;
        private byte[] _hash;
        private String _fullPath;
        private String _lgThumbPath;
        private String _extension;
        private String _caption;
        private String _name;
        private int _refCount;

        Properties.Settings Settings = Properties.Settings.Default;

        //event for changing a property
        public event PropertyChangedEventHandler PropertyChanged;
        //... add more stuff here when we have more metadata

        //
        // Public properties of this class.
        //



        public int UID
        {
            get
            {
                return _UID;
            }
            set
            {
                if (value != _UID)
                {
                    _UID = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("UID");
                }
            }
        }

        public int idInAlbum
        {
            get
            {
                return _idInAlbum;
            }
            set
            {
                if (value != _idInAlbum)
                {
                    _idInAlbum = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("idInAlbum");
                }
            }
        }

        public byte[] hash
        {
            get
            {
                return _hash;
            }
            set
            {
                if (value != _hash)
                {
                    _hash = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("hash");
                }
            }
        }

        public String fullPath
        {
            get
            {
                return _fullPath;
            }
            set
            {
                if (value != _fullPath)
                {
                    _fullPath = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("fullPath");
                }
            }
        }

        public String lgThumbPath
        {
            get
            {
                if (!File.Exists(_lgThumbPath))
                {
                    _lgThumbPath = regenerateThumbnail(fullPath, Path.GetFileName(fullPath), Settings.lrgThumbSize);
                }
                return _lgThumbPath;
            }
            set
            {
                if (value != _lgThumbPath)
                {
                    _lgThumbPath = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("lgThumbPath");
                }
            }
        }

        public String extension
        {
            get
            {
                return _extension;
            }
            set
            {
                if (value != _extension)
                {
                    _extension = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("extension");
                }
            }
        }

        public string caption
        {
            get
            {
                return _caption;
            }
            set
            {
                if (value != _caption)
                {
                    _caption = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("caption");
                }
            }
        }

        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("name");
                }
            }
        }

        public int refCount
        {
            get
            {
                return _refCount;
            }
            set
            {
                if (value != _refCount)
                {
                    _refCount = value;
                }
            }
        }

        public ComplexPhotoData()
        {
            UID = -1;
            idInAlbum = -1;
            hash = null;
            fullPath = "";
            extension = "";
            caption = "";
            name = "";
            refCount = 0;
        }

        private string regenerateThumbnail(string source, string filename, int size)
        {
            Imazen.LightResize.ResizeJob resizeJob = new Imazen.LightResize.ResizeJob();
            string thumbSubDir = "";
            string fullThumbPath = "";

            // Which sub directory of thumbs_db to put this in...
            if (size == Settings.lrgThumbSize)
            {
                thumbSubDir = Settings.lrgThumbDir;
            }

            // Specifies a maximum height resolution constraint to scale the image down to
            resizeJob.Height = size;
            resizeJob.Width = size;
            resizeJob.Mode = Imazen.LightResize.FitMode.Crop;

            //get the full path
            fullThumbPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Settings.OrgName,
                Settings.PhotoLibraryName,
                Settings.PhotoLibraryThumbsDir,
                thumbSubDir,
                filename);

            if (File.Exists(source))
            {
                // Actually processes the image, copying it to the new location, should go in a try/catch for IO
                // One of Build's overloads allows you to use file streams instead of filepaths.
                // If images have to be resized on-the-fly instead of stored, that may work as well.
                try
                {
                    resizeJob.Build(
                        source,
                        fullThumbPath,
                        Imazen.LightResize.JobOptions.CreateParentDirectory
                    );
                }
                catch (IOException)
                {
                    return "";
                }

                return fullThumbPath;
            }

            return "";
        }

        /*
         * Call this function when any property is set as part of implementing INotifyPropertyChanged
         * @Param: name is the name of the property, E.G. changing UID would mean name = "UID"
         */
        protected void OnPropertyChanged(String name)
        {
            PropertyChangedEventHandler changedHandler = PropertyChanged;

            if (changedHandler != null)
            {
                changedHandler(this, new PropertyChangedEventArgs(name));
            }
        }

        // Add a toXML function here?
    }
}
