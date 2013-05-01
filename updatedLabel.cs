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
        private bool isFrontFace;
        //private bool okToFlipBack;
        const double mouseEnterTimer = 250.0;
        const double mouseLeaveTimer = 3000.0;

        public static readonly RoutedEvent PhotoBomberTileTriggerEvent = EventManager.RegisterRoutedEvent("PhotoBomberTileEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PhotoBomberCustomObject));

        
        public PhotoBomberCustomObject(): base()
        {
            //okToFlipBack = false;

            EventTimer = new Timer();

            EventTimer.Elapsed += new ElapsedEventHandler(EventTimer_Elapsed);
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
                this.Dispatcher.BeginInvoke(new customEvent_callback(RaisePhotoBomberTileTriggerEvent), DispatcherPriority.Input, null);
            }
        }


        public event RoutedEventHandler OnPhotoBomberTileEvent
        {
            add { AddHandler(PhotoBomberTileTriggerEvent, value); }
            remove { RemoveHandler(PhotoBomberTileTriggerEvent, value); }
        }


        void RaisePhotoBomberTileTriggerEvent()
        {
            //okToFlipBack = false;
            RoutedEventArgs newEventArgs = new RoutedEventArgs(PhotoBomberCustomObject.PhotoBomberTileTriggerEvent);
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
            EventTimer.Interval = mouseEnterTimer;
            EventTimer.Start();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            EventTimer.Stop();
        }


    }
}
