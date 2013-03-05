using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;

namespace SoftwareEng
{
    public partial class PhotoBomb
    {

        //---------------VARIABLES------------------
        private BackgroundWorker addPhotosThread;



        //---------------METHODS--------------------

        private void addPhotosThread_DoWork(object sender, DoWorkEventArgs e)
        {
            //get working variables
            addPhotosThreadData data = (addPhotosThreadData)e.Argument;
            BackgroundWorker worker = sender as BackgroundWorker;

            int initialSearchOffset = util_getNewPicUID(1);

            for (int i = 0; i < data.photoUserPath.Count; ++i)
            {
                if (data.errorReport.reportID != ErrorReport.FAILURE && !worker.CancellationPending)
                {
                    String pictureName;
                    if (data.pictureNameInAlbum == null)
                        pictureName = "";
                    else if (data.pictureNameInAlbum.ElementAt(i) == "")
                    {
                        pictureName = "";
                    }
                    else
                        pictureName = data.pictureNameInAlbum.ElementAt(i);

                    addNewPicture_backend(data.errorReport, data.photoUserPath.ElementAt(i), data.photoExtension.ElementAt(i), data.albumUID, pictureName, (initialSearchOffset + i));

                    worker.ReportProgress(1);
                }//if
                else
                {
                    break;
                }
            }//for
            //done!
            e.Result = data;
        }

        //------------------------------------------

        private void addPhotosThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            addPhotosThreadData results = (addPhotosThreadData)e.Result;

            results.guiCallback(results.errorReport);
        }

        //------------------------------------------




    }//class

    //--------------------------------------------------------------------------


    public class addPhotosThreadData
    {
        public ErrorReport errorReport;
        public generic_callback guiCallback;
        public List<String> photoUserPath;
        public List<String> photoExtension;
        public int albumUID;
        public List<String> pictureNameInAlbum;
        public threadUpdateDelegate guiUpdateCallback;
        public int updateAmount;//number of photos to process before calling guiUpdate.
    }//data class

}//namespace
