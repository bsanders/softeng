﻿using System;
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
    public class customLabel : preCustomLabel
    {
        private Timer EventTimer;
        private bool isFrontFace;

        public static readonly RoutedEvent TypeOneTileTriggerEvent = EventManager.RegisterRoutedEvent("TypeOneTileEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(customLabel));

        
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

        public event RoutedEventHandler OnPhotoBomberTileEvent
        {
            add { AddHandler(TypeOneTileTriggerEvent, value); }
            remove { RemoveHandler(TypeOneTileTriggerEvent, value); }
        }


        void RaisePhotoBomberTileTriggerEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(customLabel.PhotoBomberTileTriggerEvent);
            RaiseEvent(newEventArgs);
        protected void RaisePhotoBomberTileTypeOneEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(customLabel.TypeOneTileTriggerEvent);
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

    public class preCustomLabel: Label
    {
        static readonly RoutedEvent TypeTwoTileTriggerEvent = EventManager.RegisterRoutedEvent("TypeTwoTileEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(preCustomLabel));


        public event RoutedEventHandler OnPhotoBomberTypeTwoEvent
        {
            add { AddHandler(TypeTwoTileTriggerEvent, value); }
            remove { RemoveHandler(TypeTwoTileTriggerEvent, value); }
        }


        protected void RaisePhotoBomberTileTypeTwoEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(preCustomLabel.TypeTwoTileTriggerEvent);
            RaiseEvent(newEventArgs);
        }
    
    }
}
