﻿/**
 * BY: Ryan Moe.
 * This class is for organizing the callbacks (c# delegates)
 * used to connect the backend to the gui.  We are using
 * callbacks so that the gui does not have to wait for the
 * backend to return some value (and block the user) it can
 * keep going and when a callback happens it can display that data.
 **/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SoftwareEng
{
    //use this callback if the only thing going back
    //to the gui is the error object.
    public delegate void generic_callback(ErrorReport error);

    //callback used with the getAllUserAlbumNames method.
    public delegate void getAllAlbumNames_callback(ErrorReport error, ReadOnlyObservableCollection<SimpleAlbumData> _albums);

    public delegate void getAllPhotosInAlbum_callback(ErrorReport error, List<SimplePhotoData> _pictures);

    public delegate void getPhotoByUID_callback(ErrorReport error, ComplexPhotoData picture);

    //put more advanced callbacks here, ex:
    //public delegate crazy_callback(Error error, string bob, List<int> _someList);

    public delegate void guiCreateAlbumDelegate(string input);

    //
    public delegate void threadUpdateDelegate(int processed);

}
