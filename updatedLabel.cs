/**
 * Changelog  
 * 4/28/13 Alejandro Sosa: customLabel class created
 * 5/2/13  Alejandro Sosa: preCustomLabel class created
 * 5/7/13  Alejandro Sosa: comments added
 */

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
    /**
     * Both classes created by Alejandro Sosa  
     * <summary>Both classes created to create custom routed events </summary>
     * Any functions that have no author listed are
     * created by Alejandro Sosa 
     */


    public class customLabel : preCustomLabel
    {
        private bool lockOut;
        Point mouseInitialPosition;
        Point mouseCurrentPosition;

        public static readonly RoutedEvent TypeOneTileTriggerEvent = EventManager.RegisterRoutedEvent("TypeOneTileEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(customLabel));

        public customLabel()
            : base()
        {
            lockOut = true;
        }

        public event RoutedEventHandler OnPhotoBomberTypeOneEvent
        {
            add { AddHandler(TypeOneTileTriggerEvent, value); }
            remove { RemoveHandler(TypeOneTileTriggerEvent, value); }
        }

        /**************************************************************************************************************************
         * Author: Alejandro Sosa
         **************************************************************************************************************************/
        protected void RaisePhotoBomberTileTypeOneEvent()
        {
            if (lockOut == false)
            {
                RoutedEventArgs newEventArgs = new RoutedEventArgs(customLabel.TypeOneTileTriggerEvent);
                RaiseEvent(newEventArgs);
            }
        }

        /**************************************************************************************************************************
         * Author: Alejandro Sosa
         **************************************************************************************************************************/
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            lockOut = false;

            mouseInitialPosition = e.GetPosition(this);

            Mouse.SetCursor(Cursors.Hand);
        }

        /**************************************************************************************************************************
         * Author: Alejandro Sosa
         **************************************************************************************************************************/
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            lockOut = true;

            Mouse.SetCursor(Cursors.Arrow);
        }

        /**************************************************************************************************************************
         * Author: Alejandro Sosa
         **************************************************************************************************************************/
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
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


    //for some god awful reason, declaring two routed events throws an error
    // so creating a class that inherits from a control, is the only way to
    // add an extra event
    public class preCustomLabel : Label
    {
        static readonly RoutedEvent TypeTwoTileTriggerEvent = EventManager.RegisterRoutedEvent("TypeTwoTileEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(preCustomLabel));


        public event RoutedEventHandler OnPhotoBomberTypeTwoEvent
        {
            add { AddHandler(TypeTwoTileTriggerEvent, value); }
            remove { RemoveHandler(TypeTwoTileTriggerEvent, value); }
        }


        /**************************************************************************************************************************
         * Author: Alejandro Sosa
         **************************************************************************************************************************/
        protected void RaisePhotoBomberTileTypeTwoEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(preCustomLabel.TypeTwoTileTriggerEvent);
            RaiseEvent(newEventArgs);
        }

    }
}
