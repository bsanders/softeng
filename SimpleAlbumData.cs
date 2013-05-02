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

    /// BY: Ryan Moe
    /// Edited: Julian Nguyen(4/28/13)
    /// <summary>
    /// This is a data class that will be used to send a calling 
    /// gui a list of albums. This class is a SINGLE element of that list. 
    /// </summary>
    public class SimpleAlbumData : INotifyPropertyChanged
    {
        private String _albumName;
        private Guid _UID;
        private string _thumbnailPath;
        private int _thumbAlbumID;
        //add more information here if needed...
        //event for changing a property
        public event PropertyChangedEventHandler PropertyChanged;


        /// By ??
        /// Edited: Julian Nguyen(4/28/13)
        /// <summary>
        /// Class con. 
        /// initialize vars.
        /// </summary>
        public SimpleAlbumData()
        {
            _albumName = String.Empty;
            _UID = Guid.Empty;//-1;//indicates UID not set.
            _thumbnailPath = String.Empty;
            _thumbAlbumID = -1;

            

        }


        /// By ??
        /// Edited: Julian Nguyen(4/27/13)
        /// <summary>
        /// Call this function when any property is set as part of implementing INotifyPropertyChanged.
        /// </summary>
        /// <param name="name">The name of the property, E.G. changing UID would mean name = "UID"</param>
        protected void OnPropertyChanged(String name)
        {
            PropertyChangedEventHandler changedHandler = PropertyChanged;

            if (changedHandler != null)
            {
                changedHandler(this, new PropertyChangedEventArgs(name));
            }
        }

        // Setters and Getters


        public String albumName
        {
            set
            {
                if (value != _albumName)
                {
                    _albumName = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("albumName");
                }
            }
            get
            {
                return _albumName;
            }
        }

        public Guid UID
        {
            set
            {
                if (value != _UID)
                {
                    _UID = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("UID");
                }
            }
            get
            {
                return _UID;
            }
        }

        public string thumbnailPath
        {
            get
            {
                return _thumbnailPath;
            }
            set
            {
                if (value != _thumbnailPath)
                {
                    _thumbnailPath = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("thumbnailPath");
                }
            }
        }

        public int thumbAlbumID
        {
            set
            {
                if (value != _thumbAlbumID)
                {
                    _thumbAlbumID = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("thumbAlbumID");
                }
            }
            get
            {
                return _thumbAlbumID;
            }
        }


    } // End of SimpleAlbumData
}
