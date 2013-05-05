/*
 * Change log:
 * Julian Nguyen(4/28/13)
 * This class was moved from Photobomb_controller(Photobomb) class. 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;

namespace SoftwareEng
{
    /// By: Ryan Moe
    /// Edited Julian Nguyen(4/28/13)
    /// <summary>
    /// Data class:
    /// Complex Image data for the GUI to use and display.
    /// </summary>
    public class ComplexPhotoData : INotifyPropertyChanged
    {
        //the name of the picture in the album, displayed by the gui

        // Image ID.
        private int _UID;
        private byte[] _hash;

        // File data. 
        private String _fullPath;
        private String _lgThumbPath;
        private String _extension;
        private int _refCount;

        private DateTime _addedDate;
        private DateTime _takenDate;
        private String _equipmentManufacturer;
        private String _equipmentModel;


        // In Album data.
        private int _idInAlbum;
        private String _caption;
        private String _name;
        

        Properties.Settings Settings = Properties.Settings.Default;

        //event for changing a property
        public event PropertyChangedEventHandler PropertyChanged;
        //... add more stuff here when we have more metadata


        public ComplexPhotoData()
        {
            UID = -1;
            idInAlbum = -1;
            hash = null;
            fullPath = String.Empty;
            extension = String.Empty;
            caption = String.Empty;
            name = String.Empty;
            refCount = 0;
            _takenDate = DateTime.MinValue;
            _addedDate = DateTime.MinValue;
            _equipmentManufacturer = String.Empty;
            _equipmentModel = String.Empty;
        }

        /// TODO:
        /// By ??
        /// Edited: Julian Nguyen(4/28/13)
        /// <summary>
        /// This will make a Thumbnail for the image.  
        /// </summary>
        /// <param name="source"></param>
        /// <param name="filename"></param>
        /// <param name="size"></param>
        /// <returns>The full Thumb Path.</returns>
        private string regenerateThumbnail(string source, string filename, int size)
        {
            Imazen.LightResize.ResizeJob resizeJob = new Imazen.LightResize.ResizeJob();
            string thumbSubDir = String.Empty;
            string fullThumbPath = String.Empty;

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
                    return String.Empty;
                }

                return fullThumbPath;
            }
            else
            {
                return String.Empty;
            }
        }



        /// By ??
        /// Edited: Julian Nguyen(4/28/13)
        /// <summary>
        /// Call this function when any property is set as part of implementing INotifyPropertyChanged
        /// </summary>
        /// <param name="name">name is the name of the property, E.G. changing UID would mean name = "UID"</param>
        protected void OnPropertyChanged(String name)
        {
            PropertyChangedEventHandler changedHandler = PropertyChanged;

            if (changedHandler != null)
            {
                changedHandler(this, new PropertyChangedEventArgs(name));
            }
        }


        // Getters and Setter. 

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


        public DateTime takenDate
        {
            get { return _takenDate; }
            set
            {
                if (!_takenDate.Equals(value))
                {
                    _takenDate = value;
                    OnPropertyChanged("takenDate");
                }
            }
        }

        public String equipmentManufacturer
        {
            get { return _equipmentManufacturer; }
            set
            {
                if (!_equipmentManufacturer.Equals(value))
                {
                    _equipmentManufacturer = value;
                    OnPropertyChanged("equipmentManufacturer");
                }
            }
        }

        public String equipmentModel
        {
            get { return _equipmentModel; }
            set
            {
                if (_equipmentModel.Equals(value))
                {
                    _equipmentModel = value;
                    OnPropertyChanged("equipmentModel");
                }
            }
        }


        public DateTime addedDate
        {
            get { return _addedDate; }
            set
            {
                if (_addedDate.Equals(value))
                {
                    _addedDate = value;
                    OnPropertyChanged("addedDate");
                }
            }
        }
            


        // Add a toXML function here?

    } // End of ComplexPhotoData. 
}
