using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;
using System.IO;


//Simple photo data returned by functions like getAllPhotosInAlbum().
//By: Ryan Moe
//Change Log: 
//Edited Last By: Bill Sanders (4/4/13), dropped member variables entirely, renamed UID -> idInAlbum
//Edited Last By: Ryan Causey (4/1/13)
//Edited Last By: Julian Nguyen (4/19/13): Moved from PhotoBomb.cs

namespace SoftwareEng
{


    /// <summary>
    /// 
    /// </summary>
    class SimplePhotoData : INotifyPropertyChanged
    {

        //event for changing a property
        private string _Name;
        private int _idInAlbum;
        public event PropertyChangedEventHandler PropertyChanged;

        public String Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (value != _Name)
                {
                    _Name = value;
                    //call on property changed to update the GUI(hopefully)
                    OnPropertyChanged("Name");
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

        public SimplePhotoData()
        {
            idInAlbum = -1;
            Name = "";
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
