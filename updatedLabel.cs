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
            //isFrontFace = false;

            //if (isFront == true)
            //{
            //    mouseEventTimer = 1000.0;
            //}
            //else
            //{
            //    mouseEventTimer = 3000.0;
            //}

            EventTimer = new Timer();

            EventTimer.Elapsed += new ElapsedEventHandler(EventTimer_Elapsed);

            //EventTimer.SynchronizingObject = this as ISynchronizeInvoke;

            
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

            //RaisePhotoBomberTileTriggerEvent();
            //this.Visibility = Visibility.Hidden;
            //ErrorWindow debugWindow = new ErrorWindow("Timer Elapsed");

            //debugWindow.Show();
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
            //RaisePhotoBomberTileTriggerEvent();

            
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
            //EventTimer.Interval = mouseEventTimer;
            //EventTimer.Start();
            
        }
    }
}
