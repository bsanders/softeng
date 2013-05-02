using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.ComponentModel;


namespace SoftwareEng
{
    /** ******************************************************************************************************
    /// <author>Alejandro Sosa</author>
    /// <summary>Delegate that conforms to the Dispatcher.BeginInvoke() method's requirements</summary>
    *** *****************************************************************************************************/ 
    public delegate void customEvent_callback();


    /** ******************************************************************************************************
    /// <author>Alejandro Sosa</author>
    /// <summary>Class can inherit from a non-sealed class. Used to define and raise a custom event</summary>
    *** *****************************************************************************************************/ 
    class PhotoBomberCustomObject : Label
    {
        private Timer EventTimer;
        private Point mouseInitialPosition;
        private bool isFrontFace;
        private bool mouseDown;
        const double mouseEnterTimer = 250.0;
        const double mouseLeaveTimer = 3000.0;

        public static readonly RoutedEvent PhotoBomberTileTriggerEventLeft = EventManager.RegisterRoutedEvent("PhotoBomberTileEventRight", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PhotoBomberCustomObject));

        public static readonly RoutedEvent PhotoBomberTileTriggerEventRight = EventManager.RegisterRoutedEvent("PhotoBomberTileEventLeft", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PhotoBomberCustomObject));
        
        public PhotoBomberCustomObject(): base()
        {
            mouseDown = false;

            //EventTimer = new Timer();

            //EventTimer.Elapsed += new ElapsedEventHandler(EventTimer_Elapsed);
        }

        public bool isFront
        {
            get
            {
                return isFrontFace;
            }
            set
            {
                isFrontFace = value;
            }
        }

        void EventTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            EventTimer.Stop();


            //if(isFrontFace == true)// || okToFlipBack == true)
            //{
            //    this.Dispatcher.BeginInvoke(new customEvent_callback(RaisePhotoBomberTileTriggerEvent), DispatcherPriority.Input, null);
            //}
            //else if (isFrontFace == false && this.Visibility == Visibility.Hidden)
            //{
            //    EventTimer.Interval = mouseLeaveTimer;
            //    EventTimer.Start();
            //    okToFlipBack = true;
            //}
            if (IsMouseOver == true)
            {
                this.Dispatcher.BeginInvoke(new customEvent_callback(RaisePhotoBomberTileLeftTriggerEvent), DispatcherPriority.Input, null);
            }
        }


        public event RoutedEventHandler OnPhotoBomberTileEventLeft
        {
            add { AddHandler(PhotoBomberTileTriggerEventLeft, value); }
            remove { RemoveHandler(PhotoBomberTileTriggerEventLeft, value); }
        }

        public event RoutedEventHandler OnPhotoBomberTileEventRight
        {
            add { AddHandler(PhotoBomberTileTriggerEventLeft, value); }
            remove { RemoveHandler(PhotoBomberTileTriggerEventLeft, value); }
        }


        void RaisePhotoBomberTileLeftTriggerEvent()
        {
            //okToFlipBack = false;
            RoutedEventArgs newEventArgs = new RoutedEventArgs(PhotoBomberCustomObject.PhotoBomberTileTriggerEventLeft);
            RaiseEvent(newEventArgs);
        }


        void RaisePhotoBomberTileRightTriggerEvent()
        {
            //okToFlipBack = false;
            RoutedEventArgs newEventArgs = new RoutedEventArgs(PhotoBomberCustomObject.PhotoBomberTileTriggerEventRight);
            RaiseEvent(newEventArgs);
        }

        //protected override void OnMouseEnter(MouseEventArgs e)
        //{
            
        //    EventTimer.Interval = mouseEnterTimer;
        //    EventTimer.Start();

        //}

        //protected override void OnMouseLeave(MouseEventArgs e)
        //{

        //    EventTimer.Stop();

        //}

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            //this.CaptureMouse();
            //mouseDown=true;

            mouseInitialPosition=e.GetPosition(this);

            //RaiseEvent(new RoutedEventArgs(ManipulationStartingEvent, this));


            //EventTimer.Interval = mouseEnterTimer;
            //EventTimer.Start();



            //Point currentPosition = (Point)e.GetPosition(this);

            //ErrorWindow debug = new ErrorWindow(currentPosition.ToString());

            //debug.Show();

            Mouse.SetCursor(Cursors.Hand);


        }


        //protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        //{
        //    ;
        //}

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {

            Mouse.SetCursor(Cursors.Arrow);


            //mouseDown=false;
            //this.ReleaseMouseCapture();

            //EventTimer.Stop();



            //Point currentPosition = (Point)e.GetPosition(this);

            //Vector someVector = Point.Subtract(currentPosition, mouseInitialPosition);

            //ErrorWindow debug = new ErrorWindow(someVector.ToString());

            //debug.Show();


        }




        protected override void OnMouseMove(MouseEventArgs e)
        {
            

            if (Mouse.LeftButton== MouseButtonState.Pressed)
            {
                Point currentPosition = (Point)e.GetPosition(this);

                //Vector someVector = (Vector)currentPosition;

                Vector someVector = Point.Subtract(currentPosition, mouseInitialPosition);

                //ErrorWindow debug = new ErrorWindow(someVector.ToString());

                //debug.Show();


                if (someVector.X > 25)
                {

                    RaisePhotoBomberTileRightTriggerEvent();
                    //this.ReleaseMouseCapture();
                    mouseInitialPosition.X = 0.0;
                }
                else if (someVector.X < -25)
                {
                    RaisePhotoBomberTileLeftTriggerEvent();
                    //this.ReleaseMouseCapture();
                    mouseInitialPosition.X = 0.0;
                }



            }

        }


    }
}
