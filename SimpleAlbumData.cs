using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;
using System.IO;

/*
 * BY: Ryan Moe
 * This is a data class that will be used
 * to send a calling gui a list of albums.
 * This class is a SINGLE element of that list.
 * Last Edited By: Ryan Causey
 * Last Edited Date: 3/31/13
 *  
 * 4/19/13 Julian Nguyen: The SimpleAlbumData class was removed from PhotoBomb.cs to this file.
 * 
 */

namespace SoftwareEng
{
    public class SimpleAlbumData : INotifyPropertyChanged
    {
         private String _albumName;
        private int _UID;
        private string _thumbnailPath;
        private int _thumbAlbumID;
        //add more information here if needed...
        //event for changing a property
        public event PropertyChangedEventHandler PropertyChanged;

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

        public int UID
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

        //initialize vars.
        public SimpleAlbumData()
        {
            _albumName = "";
            _UID = -1;//indicates UID not set.
            _thumbnailPath = "";
            _thumbAlbumID = -1;
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
    
    }
}
