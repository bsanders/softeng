﻿/**
 * By: Ryan Moe
 * 
 * This class is for threading functions.
 * It is a partial class of the main PhotoBomb class.
 * 
 * Make sure you know what you are doing when threading!!!
 ********************************************************************************
 * Changelog:
 * 4/5/13 Ryan Causey: Editing the addPhotosThreadData class to also contain the
 *                     observable collection needed so the run workercompleted can 
 *                     generate a ReadOnlyObservableCollection to give to the GUI 
 *                     callback.
 * Julian Nguyen(4/30/13)
 * ErrorReports constants numbers removed and replaced with ReportStatus enums.
 * Julian Nguyen(5/1/13)
 * setErrorReportToFAILURE() replaced setting an an ErrorReport to FAILURE and it's description.
 * Fun() with "Picture" in the name were changed to "Image"
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace SoftwareEng
{
    public partial class PhotoBomb
    {

        //---------------VARIABLES------------------
        private BackgroundWorker _addPhotosThread;



        /// By: Ryan Moe
        /// Edited Julian Nguyen(5/1/13)
        /// <summary>
        /// This is the working function of the backgroundworker for adding photos to the backend.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addPhotosThread_DoWork(object sender, DoWorkEventArgs e)
        {
            //get working variables
            addPhotosThreadData data = (addPhotosThreadData)e.Argument;
            BackgroundWorker worker = sender as BackgroundWorker;

            int picsToAddBeforeReporting = data.updateAmount;
            int picsAddedSinceReport = 0;

            //start our uid search from the first known empty uid.
            int initialSearchingLocation = util_getNextUID(_imagesRootXml, "picture", "uid", 1);

            //for each photo we are adding...
            for (int i = 0; i < data.photoUserPath.Count; ++i)
            {
                //if we didn't get a cancel command...
                if (data.errorReport.reportStatus != ReportStatus.FAILURE && !worker.CancellationPending)
                {
                    // Is all this unnecessary?  Leaving it here in case someday we want to prompt the user
                    // to provide a default image name for a set of importing photos, I guess?
                    String pictureName;
                    if (data.pictureNameInAlbum == null)
                    {
                        pictureName = String.Empty;
                    }
                    else if (data.pictureNameInAlbum.ElementAt(i) == String.Empty)
                    {
                        pictureName = String.Empty;
                    }
                    else
                    {
                        pictureName = data.pictureNameInAlbum.ElementAt(i);
                    }

                    //use backend function to add a single photo.
                    try
                    {
                        addNewImage_backend(data.errorReport,
                                        data.photoUserPath.ElementAt(i),
                                        data.photoExtension.ElementAt(i),
                                        data.albumUID,
                            //    pictureName,
                                        (initialSearchingLocation + i)
                                        );
                    }
                    catch (Exception)
                    {
                        setErrorReportToFAILURE(errorStrings.addImageFailure, ref data.errorReport);
                        break;
                    }

                    //report progress maybe.
                    ++picsAddedSinceReport;
                    if (picsAddedSinceReport >= picsToAddBeforeReporting)
                    {
                        worker.ReportProgress(picsToAddBeforeReporting);
                        picsAddedSinceReport = 0;
                    }
                }//if
                //something went wrong, break.
                else
                {
                    break;
                }
            }//for
            //done!
            e.Result = data;
        }

        

        /// By: Ryan Moe
        /// Edited: Ryan Causey(4/6/13)
        /// <summary>
        /// This gets called when addPhotosThread_DoWork() is complete.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addPhotosThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            addPhotosThreadData results = (addPhotosThreadData)e.Result;

            //ReadOnlyObservableCollection<ComplexPhotoData> photosToGui = new ReadOnlyObservableCollection<ComplexPhotoData>(results.photoCollection);

            results.guiCallback(results.errorReport, results.albumUID);
        }

        //------------------------------------------




    }//class

    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------

    /// By: Ryan Moe
    /// Edited: Ryan Causey(4/6/13)
    /// <summary>
    /// 
    /// </summary>
    public class addPhotosThreadData
    {
        public ErrorReport errorReport;
        public addNewPictures_callback guiCallback;
        public List<String> photoUserPath;
        public List<String> photoExtension;
        public Guid albumUID;
        public List<String> pictureNameInAlbum;
        public threadUpdateDelegate guiUpdateCallback;
        public int updateAmount;//number of photos to process before calling guiUpdate.
        //public ObservableCollection<ComplexPhotoData> photoCollection;
    }//data class

}//namespace
