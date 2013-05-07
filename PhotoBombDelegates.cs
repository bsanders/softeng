/**
 * BY: Ryan Moe.
 * This class is for organizing the callbacks (c# delegates)
 * used to connect the backend to the gui.  We are using
 * callbacks so that the gui does not have to wait for the
 * backend to return some value (and block the user) it can
 * keep going and when a callback happens it can display that data.
 * ******************************************************************************
 * Changelog:
 * 4/4/13 Ryan Causey: Edited the getAllPhotosInAlbum_callback delegate to support
 *                     new ReadOnlyObservableCollection required for new GUI.
 * 4/6/13 Ryan Causey: Adding new delegate for addNewPictures function to allow the user
 *                     to move around the program while importing photos and not have it crash
 *                     on import finish.
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

    /*
     * Created By: Ryan Causey
     * Created Date: 4/6/13
     * Last Edited By:
     * Last Edited Date:
     */
    /// <summary>
    /// callback to be use with addNewPictures method
    /// </summary>
    /// <param name="error">Error reports from the back end.</param>
    /// <param name="UIDofAlbum">UID of the album the pictures are being added too.</param>
    public delegate void addNewPictures_callback(ErrorReport error, Guid albumUID);

    //callback used with the getAllUserAlbumNames method.
    public delegate void getAllAlbumNames_callback(ErrorReport error, ReadOnlyObservableCollection<SimpleAlbumData> _albums);

    public delegate void getAllPhotosInAlbum_callback(ErrorReport error, ReadOnlyObservableCollection<ComplexPhotoData> _pictures);

    public delegate void sendAllPhotosInAlbum_callback(ErrorReport error, List<ComplexPhotoData> _pictures);

    public delegate void getPhotoByUID_callback(ErrorReport error, ComplexPhotoData picture);

    //put more advanced callbacks here, ex:
    //public delegate crazy_callback(Error error, string bob, List<int> _someList);

    public delegate void guiCreateAlbumDelegate(string input);

    //
    public delegate void threadUpdateDelegate(int processed);

    public delegate void greyScaleConverterDelegate(addNewPictures_callback returnAddress, ComplexPhotoData imageToBeConverted, Guid inThisAlbum);
    
}
