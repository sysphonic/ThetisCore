/* 
 * MenuItemPanel.xaml.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Sysphonic.Common;
using ThetisCore.Task;

namespace ThetisCore.Navi
{
    /// <summary>Menu Item Panel.</summary>
    public partial class MenuItemPanel : UserControl, IInfoItemPanel
    {
        /// <summary>Background brush of the information (high-lighted).</summary>
        protected SolidColorBrush _bgBrushOn = null;

        /// <summary>Background brush of the information (normal).</summary>
        protected SolidColorBrush _bgBrushOff = null;

        /// <summary>Border brush of the selected information.</summary>
        protected SolidColorBrush _borderBrushSel = null;

        /// <summary>Selected flag.</summary>
        protected bool _selected = false;

        /// <summary>Constructor.</summary>
        /// <param name="title">Menu title.</param>
        /// <param name="mouseBtnEvtHandler">ex. new MouseButtonEventHandler(wndMain.infoItem_Click)</param>
        public MenuItemPanel(string title, MouseButtonEventHandler mouseBtnEvtHandler)
        {
            InitializeComponent();

            _bgBrushOn = new SolidColorBrush(Color.FromArgb(255, 145, 255, 251));
            _bgBrushOn.Opacity = 0.5;

            _bgBrushOff = new SolidColorBrush(Colors.Black);
            _bgBrushOff.Opacity = 0.25;

            _borderBrushSel = new SolidColorBrush(Colors.Lime);
            _borderBrushSel.Opacity = 0.8;

            HorizontalAlignment = HorizontalAlignment.Stretch;

            txbMenuItem.Text = title;

            WndMain wndMain = (WndMain)App.Current.MainWindow;

            MouseDown += mouseBtnEvtHandler;
            MouseEnter += new MouseEventHandler(flowDoc_MouseEnter);
            MouseLeave += new MouseEventHandler(flowDoc_MouseLeave);

            Cursor = Cursors.Hand;
            ForceCursor = true;
            Background = _bgBrushOff;
            Height = 50;
        }

        #region IItemInfoPanel Member

        public bool HasThumb()
        {
            return false;
        }

        public bool IsThumbLoaded()
        {
            return false;
        }

        public bool LoadThumb(System.EventHandler<RoutedEventArgs> onImageLoaded, System.EventHandler<ExceptionRoutedEventArgs> onImageFailed)
        {
            return true;
        }

        /// <summary>Gets Information Item related to this panel.</summary>
        /// <returns>Information Item related to this panel.</returns>
        public InfoItem GetInfoItem()
        {
            InfoItem dummy = new InfoItem();
            dummy.Title = txbMenuItem.Text;
            return dummy;
        }

        /// <summary>Sets as selected or deselected.</summary>
        /// <param name="sel">Selected flag.</param>
        public void SetSelected(bool sel)
        {
            _selected = sel;

            if (sel)
            {
                BorderThickness = new Thickness(2.0, 1.0, 2.0, 1.0);
                BorderBrush = _borderBrushSel;
            }
            else
            {
                BorderThickness = new Thickness(0.0);
                BorderBrush = null;
            }
        }

        /// <summary>Gets if the mouse hovering on this panel.</summary>
        /// <returns>true if the mouse hovering on this panel, false otherwise.</returns>
        public bool IsMouseHovering()
        {
            return (Background == _bgBrushOn);
        }

        /// <summary>Marks as read.</summary>
        public void MarkRead()
        {
        }

        /// <summary>Marks as unread.</summary>
        public void MarkUnread()
        {
        }

        #endregion

        /// <summary>Mouse Enter event handler for each Information Item.</summary>
        /// <param name="sender">Flow Document of the Information Item.</param>
        /// <param name="e">Mouse Event parameters.</param>
        public void flowDoc_MouseEnter(object sender, MouseEventArgs e)
        {
            Background = _bgBrushOn;
        }

        /// <summary>Mouse Leave event handler for each Information Item.</summary>
        /// <param name="sender">Flow Document of the Information Item.</param>
        /// <param name="e">Mouse Event parameters.</param>
        public void flowDoc_MouseLeave(object sender, MouseEventArgs e)
        {
            Background = _bgBrushOff;
        }
    }
}
