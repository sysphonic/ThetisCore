/* 
 * InfoItemPanel_B.xaml.cs
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
    /// <summary>Grid Panel for Information Item.</summary>
    public partial class InfoItemPanel_B : UserControl, IInfoItemPanel
    {
        /// <summary>Information Item related to this panel.</summary>
        protected InfoItem _item = null;

        /// <summary>Background brush of the information (high-lighted).</summary>
        protected SolidColorBrush _bgBrushOn = null;

        /// <summary>Background brush of the information (normal).</summary>
        protected SolidColorBrush _bgBrushOff = null;

        /// <summary>Border brush of the selected information.</summary>
        protected SolidColorBrush _borderBrushSel = null;

        /// <summary>Selected flag.</summary>
        protected bool _selected = false;

        /// <summary>Constructor.</summary>
        /// <param name="item">Information Item related to this panel.</param>
        public InfoItemPanel_B(InfoItem item)
        {
            InitializeComponent();

            _bgBrushOn = new SolidColorBrush(Color.FromArgb(255, 145, 255, 251));
            _bgBrushOn.Opacity = 0.5;

            _bgBrushOff = new SolidColorBrush(Colors.Black);
            _bgBrushOff.Opacity = 0.25;

            _borderBrushSel = new SolidColorBrush(Colors.Lime);
            _borderBrushSel.Opacity = 0.8;

            _item = item;
            HorizontalAlignment = HorizontalAlignment.Stretch;

            if (item.Images != null && item.Images.Length > 0)
            {
//              imgThumb.Source = WpfUtil.LoadImage(item.Images[0]);
            }
            else
            {
                GridLengthConverter gridConv = new GridLengthConverter();
                GridLength imgWidth = (GridLength)gridConv.ConvertFromString("0px");
                colImg.Width = imgWidth;
            }
            txbItem.Text = item.SrcTitle + "\n" + item.Title;
            if (item.IsRead)
                txbItem.Foreground = Brushes.White;
            else
                txbItem.Foreground = Brushes.Yellow;

            WndMain wndMain = (WndMain)App.Current.MainWindow;

            MouseDown += new MouseButtonEventHandler(wndMain.infoItem_Click);
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
            return (_item.Images != null && _item.Images.Length > 0);
        }

        public bool IsThumbLoaded()
        {
            return (imgThumb.Source != null);
        }

        public bool LoadThumb(System.EventHandler<RoutedEventArgs> onImageLoaded, System.EventHandler<ExceptionRoutedEventArgs> onImageFailed)
        {
            if (HasThumb())
            {
                if (onImageFailed != null)
                    imgThumb.ImageFailed += onImageFailed;
                if (onImageLoaded != null)
                    imgThumb.Loaded += new RoutedEventHandler(onImageLoaded);
                imgThumb.Source = WpfUtil.LoadImage(_item.Images[0]);
                return true;
            }
            else
                return false;
        }

        /// <summary>Gets Information Item related to this panel.</summary>
        /// <returns>Information Item related to this panel.</returns>
        public InfoItem GetInfoItem()
        {
            return _item;
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
            _item.IsRead = true;
            txbItem.Foreground = Brushes.White;
        }

        /// <summary>Marks as unread.</summary>
        public void MarkUnread()
        {
            _item.IsRead = false;
            txbItem.Foreground = Brushes.Yellow;
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
