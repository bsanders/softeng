/*********************************************************************************
 * This is the code behind file for the ViewImage window
 * 
 * *******************************************************************************
 * Changelog:
 * 4/6/13 Ryan Causey: Trying to get this to databind to the image the user wishes to view.
 * 4/7/13 Ryan Causey: Databinding is working, including fallback values! I am the greatest!
 *                     Image now scales to window size.
 *                     Implemented the "next" button functionality on the imageView.
 *                     Implemented the "previous" button functionality on the imageView.
 *                     Got the sliding animation on the lower dockbar to work.
 *                     Hooked up slide show button. Currently does not automatically play at any rate
 *                     but still puts window into slideshow mode.
 *                     Implemented slide show so that it cycles through pictures at a rate of 5 seconds.
 *                     It is still not hooked up to the slider, however.
 *                     Fixed bug 53 where the slideshow view wouldn't work.
 *                     Hooked up the slideshow speed slider.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SoftwareEng
{
    /// <summary>
    /// Interaction logic for ViewImage.xaml
    /// </summary>
    public partial class ViewImage : Window, INotifyPropertyChanged
    {
        private ReadOnlyObservableCollection<ComplexPhotoData> _picturesCollection;
        private CollectionView _imageCollection;
        private ComplexPhotoData _currentPicture;
        private Guid _currentAlbum;
        //event for changing a property
        public event PropertyChangedEventHandler PropertyChanged;
        //timer for slideshow
        DispatcherTimer slideShowTimer = new DispatcherTimer();
        private greyScaleConverterDelegate MainWindowScaleFunction;


        //property for data binding
        public ComplexPhotoData currentPicture
        {
            get
            {
                return _currentPicture;
            }
            set
            {
                if (value != _currentPicture)
                {
                    _currentPicture = value;
                    OnPropertyChanged("currentPicture");
                }
            }
        }

        // A default constructor, just used to instantiate the object.  DON'T ACTUALLY USE THIS
        public ViewImage()
        {
            // noop
        }

        ///// <summary>
        ///// Constructor for ViewImage
        ///// </summary>
        ///// <param name="picturesCollectionFromAlbum">The collection of pictures from the album</param>
        ///// <param name="imageUID">the UID of the current picture</param>
        //public ViewImage(ReadOnlyObservableCollection<ComplexPhotoData> picturesCollectionFromAlbum, int imageUID)
        //{
        //    _picturesCollection = picturesCollectionFromAlbum;
        //    currentPicture = _picturesCollection.FirstOrDefault(photo => photo.UID == imageUID);
        //    InitializeComponent();
        //    //set the timer's default values.
        //    slideShowTimer.Interval = new TimeSpan(0, 0, 5);
        //    slideShowTimer.Tick += new EventHandler(slideShowTimer_Tick);
        //}

        /// <summary>
        /// Constructor for ViewImage
        /// </summary>
        /// <param name="picturesCollectionFromAlbum">The collection of pictures from the album</param>
        /// <param name="imageUID">the UID of the current picture</param>
        /// <param name="slideShowStart">True if you want to skip viewing the photo and go directly into a slideshow, defaults to false</param>
        public ViewImage(ReadOnlyObservableCollection<ComplexPhotoData> picturesCollectionFromAlbum, int imageUID, Boolean slideShowStart = false)
        {
            _picturesCollection = picturesCollectionFromAlbum;
            currentPicture = _picturesCollection.FirstOrDefault(photo => photo.UID == imageUID);
            InitializeComponent();
            //set the timer's default values.
            slideShowTimer.Interval = new TimeSpan(0, 0, 5);
            slideShowTimer.Tick += new EventHandler(slideShowTimer_Tick);

            this.Height = 500.0;
            this.Width = 600.0;

            if (slideShowStart)
            {
                enterSlideShowState();
            }
        }



        public ViewImage(CollectionView bindToThisView, int imageUID, Guid albumID, greyScaleConverterDelegate greyscaleFunction, Boolean slideShowStart = false)
        {
            _imageCollection = bindToThisView;
            currentPicture = _imageCollection.CurrentItem as ComplexPhotoData;
            _currentAlbum = albumID;
            MainWindowScaleFunction = greyscaleFunction;
            //currentPicture = _picturesCollection.FirstOrDefault(photo => photo.UID == imageUID);
            InitializeComponent();
            //set the timer's default values.
            slideShowTimer.Interval = new TimeSpan(0, 0, 5);
            slideShowTimer.Tick += new EventHandler(slideShowTimer_Tick);

            this.Height = 500.0;
            this.Width = 600.0;

            MainWindowScaleFunction = greyscaleFunction;

            if (slideShowStart)
            {
                enterSlideShowState();
            }
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/7/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// Function to transition to next image.
        /// </summary>
        private void getNextImage()
        {
            _imageCollection.MoveCurrentToNext();

            if (_imageCollection.IsCurrentAfterLast == true)
            {
                _imageCollection.MoveCurrentToFirst();
            }
            currentPicture = _imageCollection.CurrentItem as ComplexPhotoData;

            ////get the current index
            //int index = _picturesCollection.IndexOf(currentPicture);
            ////set the current picture to the next one
            //if (!(index < _picturesCollection.Count - 1))
            //{
            //    index = 0;
            //    currentPicture = _picturesCollection.ElementAt(index);
            //}
            //else
            //{
            //    currentPicture = _picturesCollection.ElementAt(++index);
            //}

            
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/7/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// Function to transition to previous image.
        /// </summary>
        private void getPreviousImage()
        {
            _imageCollection.MoveCurrentToPrevious();
            if (_imageCollection.IsCurrentBeforeFirst == true)
            {
                _imageCollection.MoveCurrentToLast();
            }
            currentPicture = _imageCollection.CurrentItem as ComplexPhotoData;
            //get the current index
            //int index = _picturesCollection.IndexOf(currentPicture);
            ////set the current picture to the next one
            //if (index == 0)
            //{
            //    index = _picturesCollection.Count - 1;
            //    currentPicture = _picturesCollection.ElementAt(index);
            //}
            //else
            //{
            //    currentPicture = _picturesCollection.ElementAt(--index);
            //}
            
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        private void exitButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        private void DockPanel_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount == 2)
                {
                    toggleWindowState();
                }
                else
                {
                    this.DragMove();
                }
            }
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        private void maximizeToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            toggleWindowState();
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Minimized;
            }
        }

        /**************************************************************************************************************************
        **************************************************************************************************************************/
        private void toggleWindowState()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/7/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// Function to enter the slideshow state.
        /// </summary>
        private void enterSlideShowState()
        {
            //hide the slideshow button
            this.slideShowDockButton.Visibility = Visibility.Collapsed;
            //show the cancel slideshow button
            this.exitSlideShowDockButton.Visibility = Visibility.Visible;
            this.WindowState = WindowState.Maximized;
            this.applicationDockBar.Visibility = Visibility.Collapsed;
            this.emptyDockButton.Visibility = Visibility.Hidden;
            this.slideShowSpeedStackPanel.Visibility = Visibility.Visible;
            slideShowTimer.Start();
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/7/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// Function to exit the slidwshow state
        /// </summary>
        private void exitSlideShowState()
        {
            //show the slideshow button
            this.slideShowDockButton.Visibility = Visibility.Visible;
            //hide the cancel slideshow button
            this.exitSlideShowDockButton.Visibility = Visibility.Collapsed;
            this.WindowState = WindowState.Normal;
            this.applicationDockBar.Visibility = Visibility.Visible;
            this.emptyDockButton.Visibility = Visibility.Collapsed;
            this.slideShowSpeedStackPanel.Visibility = Visibility.Collapsed;
            slideShowTimer.Stop();
        }

        private void setSlideShowSpeed()
        {
            //get the slider value and set the speed to it
            slideShowTimer.Interval = new TimeSpan(0, 0, (int)this.slideshowSpeedSlider.Value);
        }

        //thumbar functions for resizing the window
        #region thumbarResizefunctions

        /*
         *Created By: Alejandro Sosa
         *Last Edited By: Ryan Causey
         *Last Edited Date: 4/1/13
         *This handles the resizing via dragging from the bottom edge of the window, making sure the window height
         *does not go below a certain minimum
         */
        private void bottomThumb_DragDeltaHandler(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            //resize from bottom
            if (this.Height > this.ViewImageWindowGrid.MinHeight)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative height.
                if ((this.Height + e.VerticalChange) > this.ViewImageWindowGrid.MinHeight)
                {
                    this.Height += e.VerticalChange;
                }
            }
            else
            {
                this.Height = this.ViewImageWindowGrid.MinHeight + 1;
            }

        }

        /*
         *Created By: Alejandro Sosa
         *Last Edited By: Ryan Causey
         *Last Edited Date: 4/1/13
         *This handles the resizing via dragging from the top edge of the window, making sure the window height
         *does not go below a certain minimum
         */
        private void topThumb_DragDeltaHandler(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

            //resize from top
            if (this.Height > this.ViewImageWindowGrid.MinHeight)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative height.
                if ((this.Height - e.VerticalChange) > this.ViewImageWindowGrid.MinHeight)
                {
                    this.Height -= e.VerticalChange;
                    this.Top += e.VerticalChange;
                }
            }
            else
            {
                this.Height = MinHeight + 1;
                this.Top -= e.VerticalChange;
            }
        }

        /*
         *Created By: Alejandro Sosa
         *Last Edited By: Ryan Causey
         *Last Edited Date: 4/1/13
         *This handles the resizing via dragging from the right edge of the window, making sure the window width
         *does not go below a certain minimum
         */
        private void rightThumb_DragDeltaHandler(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            //resize from right
            if (this.Width > this.ViewImageWindowGrid.MinWidth)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative width.
                if ((this.Width + e.HorizontalChange) > this.ViewImageWindowGrid.MinWidth)
                {
                    this.Width += e.HorizontalChange;
                }
            }
            else
            {
                this.Width = this.ViewImageWindowGrid.MinWidth + 1;
            }
        }

        /*
         *Created By: Alejandro Sosa
         *Last Edited By: Ryan Causey
         *Last Edited Date: 4/1/13
         *This handles the resizing via dragging from the left edge of the window, making sure the window width
         *does not go below a certain minimum
         */
        private void leftThumb_DragDeltaHandler(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            //resize from Left
            if (this.Width > this.ViewImageWindowGrid.MinWidth)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative width.
                if ((this.Width - e.HorizontalChange) > this.ViewImageWindowGrid.MinWidth)
                {
                    this.Width -= e.HorizontalChange;
                    this.Left += e.HorizontalChange;
                }

            }
            else
            {
                this.Width = this.ViewImageWindowGrid.MinWidth + 1;
            }
        }

        /*
         *Created By: Alejandro Sosa
         *Last Edited By: Ryan Causey
         *Last Edited Date: 4/1/13
         *This handles the resizing via dragging from the bottom right corner of the window, making sure the window height
         *and width do not go below a certain minimum
         */
        private void bottomRightThumb_DragDeltaHandler(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            //resize from bottom
            if (this.Height > this.ViewImageWindowGrid.MinHeight)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative height.
                if ((this.Height + e.VerticalChange) > this.ViewImageWindowGrid.MinHeight)
                {
                    this.Height += e.VerticalChange;
                }
            }
            else
            {
                this.Height = this.ViewImageWindowGrid.MinHeight + 1;
            }

            //resize from right
            if (this.Width > this.ViewImageWindowGrid.MinWidth)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative width.
                if ((this.Width + e.HorizontalChange) > this.ViewImageWindowGrid.MinWidth)
                {
                    this.Width += e.HorizontalChange;
                }
            }
            else
            {
                this.Width = this.ViewImageWindowGrid.MinWidth + 1;
            }
        }

        /*
         *Created By: Alejandro Sosa
         *Last Edited By: Ryan Causey
         *Last Edited Date: 4/1/13
         *This handles the resizing via dragging from the top right corner of the window, making sure the window height
         *and width do not go below a certain minimum
         */
        private void topRightThumb_DragDeltaHandler(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            //resize from top
            if (this.Height > this.ViewImageWindowGrid.MinHeight)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative height.
                if ((this.Height - e.VerticalChange) > this.ViewImageWindowGrid.MinHeight)
                {
                    this.Height -= e.VerticalChange;
                    this.Top += e.VerticalChange;
                }

            }
            else
            {
                this.Height = this.ViewImageWindowGrid.MinHeight + 1;
            }

            //resize from right
            if (this.Width > this.ViewImageWindowGrid.MinWidth)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative width.
                if ((this.Width + e.HorizontalChange) > this.ViewImageWindowGrid.MinWidth)
                {
                    this.Width += e.HorizontalChange;
                }
            }
            else
            {
                this.Width = this.ViewImageWindowGrid.MinWidth + 1;
            }
        }

        /*
         *Created By: Alejandro Sosa
         *Last Edited By: Ryan Causey
         *Last Edited Date: 4/1/13
         *This handles the resizing via dragging from the bottom left corner of the window, making sure the window height
         *and width do not go below a certain minimum
         */
        private void bottomLeftThumb_DragDeltaHandler(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            //resize from bottom
            if (this.Height > this.ViewImageWindowGrid.MinHeight)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative height.
                if ((this.Height + e.VerticalChange) > this.ViewImageWindowGrid.MinHeight)
                {
                    this.Height += e.VerticalChange;
                }
            }
            else
            {
                this.Height = MinHeight + 1;
            }

            //resize from left
            if (this.Width > this.ViewImageWindowGrid.MinWidth)
            {
                //Handles an error case where trying to drag the window too small with a jerking motion would give
                //a negative width.
                if ((this.Width - e.HorizontalChange) > this.ViewImageWindowGrid.MinWidth)
                {
                    this.Width -= e.HorizontalChange;
                    this.Left += e.HorizontalChange;
                }
            }
            else
            {
                this.Width = this.ViewImageWindowGrid.MinWidth + 1;
            }
        }

        #endregion



        /*
         * Created By: Ryan Causey
         * Created Date: 4/7/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// On click handler for the next button on the ViewItem dock.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event args</param>
        private void nextDockButton_Click(object sender, RoutedEventArgs e)
        {
            //setSlideShowSpeed();
            //need to call the next function here.
            getNextImage();
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/7/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// On click handler for the previous button on the ViewItem dock.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event args</param>
        private void prevDockButton_Click(object sender, RoutedEventArgs e)
        {
            //setSlideShowSpeed();
            getPreviousImage();
        }

        private void dockHitBox_MouseEnter(object sender, MouseEventArgs e)
        {
            mainWindowDock.Height = Double.NaN;
        }

        private void mainWindowDock_MouseLeave(object sender, MouseEventArgs e)
        {
            mainWindowDock.Height = 1;
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/7/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// On click handler for slide show button.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event args</param>
        private void slideShowDockButton_Click(object sender, RoutedEventArgs e)
        {
            enterSlideShowState();
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/7/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// On click handler for the exit slide show button.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event args</param>
        private void exitSlideShowDockButton_Click(object sender, RoutedEventArgs e)
        {
            exitSlideShowState();
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/7/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// Tick handler for the slideShowTimer. Will cycle to next picture on tick.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void slideShowTimer_Tick(Object sender, EventArgs e)
        {
            getNextImage();
        }

        /*
         * Created By: Ryan Causey
         * Created Date: 4/7/13
         * Last Edited By:
         * Last Edited Date:
         */
        /// <summary>
        /// Handler for when the value of the slideshow speed slider changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void slideshowSpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setSlideShowSpeed();

            if (slideShowSpeedStackPanel.Visibility == Visibility.Visible)
            {
                slideShowTimer.Start();
            }
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

        private void greyscaleDockButton_Click(object sender, RoutedEventArgs e)
        {

        }


        private void convertImageToGreyScale()
        {
            ;
        }
    }

    public class GreyScaleConverter
    {
        private BitmapImage _sourceImage;
        private FormatConvertedBitmap _greyScaleImage;

        public GreyScaleConverter() { }

        public void toGrayScale(String sourcePath)
        {
            _sourceImage = new BitmapImage();
            _sourceImage.BeginInit();

            _sourceImage.UriSource = new Uri(@sourcePath, UriKind.Absolute);

            _sourceImage.EndInit();


            _greyScaleImage = new FormatConvertedBitmap();

            _greyScaleImage.BeginInit();

            _greyScaleImage.Source = _sourceImage;

            _greyScaleImage.DestinationFormat = PixelFormats.Gray32Float;
        
        }





    }
}
