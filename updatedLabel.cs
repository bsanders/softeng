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
    public delegate void customEvent_callback();


    class customLabel : Label
    {
        private Timer EventTimer;
        private bool isFrontFace;
        const double mouseEnterTimer = 1000.0;
        const double mouseLeaveTimer = 3000.0;

        public static readonly RoutedEvent PhotoBomberTileTriggerEvent = EventManager.RegisterRoutedEvent("PhotoBomberTileEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(customLabel));

        
        public customLabel(): base()
        {
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
            this.Dispatcher.BeginInvoke(new customEvent_callback(RaisePhotoBomberTileTriggerEvent), DispatcherPriority.Input, null); 
        }


        public event RoutedEventHandler OnPhotoBomberTileEvent
        {
            add { AddHandler(PhotoBomberTileTriggerEvent, value); }
            remove { RemoveHandler(PhotoBomberTileTriggerEvent, value); }
        }


        void RaisePhotoBomberTileTriggerEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(customLabel.PhotoBomberTileTriggerEvent);
            RaiseEvent(newEventArgs);
        }



        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (isFrontFace == true)
            {
                EventTimer.Interval = mouseEnterTimer;
                EventTimer.Start();
            }
            else
            {
                EventTimer.Stop();
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (isFrontFace == false)
            {
                EventTimer.Interval = mouseLeaveTimer;
                EventTimer.Start();
            }
            else
            {
                EventTimer.Stop();
            }
        }
    }
}
