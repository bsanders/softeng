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
        Point mouseInitialPosition;
        Point mouseCurrentPosition;
        ErrorWindow debug;

        public static readonly RoutedEvent TypeOneTileTriggerEvent = EventManager.RegisterRoutedEvent("TypeOneTileEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(customLabel));

        public customLabel() : base()
        {
            lockOut = true;
        }

        public event RoutedEventHandler OnPhotoBomberTypeOneEvent
        {
            add { AddHandler(TypeOneTileTriggerEvent, value); }
            remove { RemoveHandler(TypeOneTileTriggerEvent, value); }
        }


        protected void RaisePhotoBomberTileTypeOneEvent()
        {
            if (lockOut == false)
            {
                RoutedEventArgs newEventArgs = new RoutedEventArgs(customLabel.TypeOneTileTriggerEvent);
                RaiseEvent(newEventArgs);
            }
        }


        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            lockOut= false;

            mouseInitialPosition = e.GetPosition(this);

            Mouse.SetCursor(Cursors.Hand);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            lockOut = true;

            Mouse.SetCursor(Cursors.Arrow);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (Mouse.LeftButton== MouseButtonState.Pressed)
            {
                mouseCurrentPosition = (Point)e.GetPosition(this);

                Vector someVector = Point.Subtract(mouseCurrentPosition, mouseInitialPosition);


                if (someVector.X > 25)
                {

                    RaisePhotoBomberTileTypeOneEvent();
                    mouseInitialPosition.X = 0.0;
                    lockOut = true;
                }
                else if (someVector.X < -25)
                {
                    RaisePhotoBomberTileTypeTwoEvent();
                    mouseInitialPosition.X = 0.0;
                    lockOut = true;
                }
            }
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


        protected void RaisePhotoBomberTileTypeTwoEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(preCustomLabel.TypeTwoTileTriggerEvent);
            RaiseEvent(newEventArgs);
        }

    }
}
