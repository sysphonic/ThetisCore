/* 
 * ResizeTopLeft.xaml.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace ThetisCore.Navi
{
    public partial class ResizeTopLeft : UserControl
    {
        public delegate void ResizeDelegate(double deltaHorz, double deltaVert);

        /// <summary>Configuration for ThetisCoreNavi.</summary>
        private FrameworkElement[] _hideElements = new FrameworkElement[] { };

        public ResizeTopLeft()
        {
            InitializeComponent();
        }

        public FrameworkElement[] HideElements
        {
            get { return _hideElements; }
            set { _hideElements = value; }
        }

        private Window GetWindow()
        {
            for (FrameworkElement elem = this; elem != null; elem = (FrameworkElement)elem.Parent)
			{
				if (elem.Parent is Window)
                    return (Window)(elem.Parent);
			}
            return null;
        }

        private void OnResizeDragStarted(object sender, DragStartedEventArgs e)
        {
			foreach (UIElement elem in _hideElements)
			{
            	elem.Visibility = Visibility.Hidden;
			}
        }

        private void OnResizeDragCompleted(object sender, DragCompletedEventArgs e)
        {
			foreach (UIElement elem in _hideElements)
			{
            	elem.Visibility = Visibility.Visible;
			}
        }

        private void OnResizeTopLeftDragDelta(object sender, DragDeltaEventArgs e)
        {
			Window wnd = GetWindow();
            using (var d = Dispatcher.DisableProcessing())
            {
                wnd.Dispatcher.BeginInvoke(
                        new ResizeDelegate(this.doResizeTopLeft),
                        System.Windows.Threading.DispatcherPriority.Normal,
                        new object[] {e.HorizontalChange, e.VerticalChange}
                      );
            }
        }

        private void doResizeTopLeft(double deltaHorz, double deltaVert)
        {
			Window wnd = GetWindow();

            double xAdjust = wnd.ActualWidth - deltaHorz;
            double yAdjust = wnd.ActualHeight - deltaVert;

            //make sure not to resize to negative width or heigth            
            xAdjust = (wnd.ActualWidth + xAdjust) > wnd.MinWidth ? xAdjust : wnd.MinWidth;
            yAdjust = (wnd.ActualHeight + yAdjust) > wnd.MinHeight ? yAdjust : wnd.MinHeight;

            wnd.Width = xAdjust;
            wnd.Height = yAdjust;

            wnd.Top += deltaVert;
            wnd.Left += deltaHorz;
        }
    }
}
