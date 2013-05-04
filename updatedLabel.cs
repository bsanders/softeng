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


    public class customLabel : preCustomLabel
    {
        private Timer EventTimer;
        private bool isFrontFace;
        private bool lockOut;

        public static readonly RoutedEvent TypeOneTileTriggerEvent = EventManager.RegisterRoutedEvent("TypeOneTileEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(customLabel));

        public customLabel() : base()
        {
            lockOut = true;
        }

        public bool isFront
        {
        }

        public event RoutedEventHandler OnPhotoBomberTileEvent
        {
            add { AddHandler(TypeOneTileTriggerEvent, value); }
            remove { RemoveHandler(TypeOneTileTriggerEvent, value); }
        }


        protected void RaisePhotoBomberTileTriggerEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(customLabel.TypeOneTileTriggerEvent);
            RaiseEvent(newEventArgs);
        }


        protected override void OnMouseLeftDown(MouseEventArgs e)
        {
            lockOut= true;
        }
    }

    public class preCustomLabel : Label
    {
        static readonly RoutedEvent TypeTwoTileTriggerEvent = EventManager.RegisterRoutedEvent("TypeTwoTileEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(preCustomLabel));


        public event RoutedEventHandler OnPhotoBomberTypeTwoEvent
        {
            add { AddHandler(TypeTwoTileTriggerEvent, value); }
            remove { RemoveHandler(TypeTwoTileTriggerEvent, value); }
        }


        protected void RaisePhotoBomberTileTriggerEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(preCustomLabel.TypeTwoTileTriggerEvent);
            RaiseEvent(newEventArgs);
        }

    }
}
