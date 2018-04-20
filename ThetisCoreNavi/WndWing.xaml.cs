/* 
 * WndWing.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls.Primitives;   // for ToggleButton
using System.Windows.Media; // for Brush
using ThetisCore.Task;
using Sysphonic.Common;

namespace ThetisCore.Navi
{
    /// <summary>Wing Window class.</summary>
    public partial class WndWing : Window
    {
        /// <summary>Location offset from the Main Window.</summary>
        protected double _offsetX = 0.0;
        protected double _offsetY = 0.0;

        /// <summary>Reference to the Information Item Panel related to this window.</summary>
        private IInfoItemPanel _infoItemPanel = null;

        public delegate void ResizeDelegate(double deltaHorz, double deltaVert);

        /// <summary>Constructor.</summary>
        public WndWing()
        {
            InitializeComponent();
        }

        /// <summary>Information Manager</summary>
        public IInfoItemPanel InfoItemPanel
        {
            get { return _infoItemPanel; }
        }

        /// <summary>Sets Information Item.</summary>
        /// <param name="panel">Information Item Panel.</param>
        public void ShowInfoItem(IInfoItemPanel panel)
        {
            _infoItemPanel = panel;
            InfoItem item = panel.GetInfoItem();

            txbTitle.Text = item.SrcTitle;
            txbUrl.Text = item.Link;
            txbChannel.Text = item.Channel;

            dkpView.Children.Clear();
            InfoItemViewer intoViewer = new InfoItemViewer(item);
            intoViewer.Width = dkpView.ActualWidth;
            intoViewer.Height = dkpView.ActualHeight;
            dkpView.Children.Add(intoViewer);

            if (item.Description == null || item.Description.Length <= 0)
                lblEmpty.Visibility = Visibility.Visible;
            else
                lblEmpty.Visibility = Visibility.Hidden;
        }

        /// <summary>Closing event handler.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void wndWing_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WndMain wndMain = (WndMain)App.Current.MainWindow;
            wndMain.NaviConfig.WingWidth = Width;
            wndMain.NaviConfig.WingHeight = Height;

            if (_infoItemPanel != null)
            {
                //_infoItemPanel.SetSelected(false);
                _infoItemPanel = null;
            }
        }

        /// <summary>Closed event handler.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void wndWing_Closed(object sender, EventArgs e)
        {
            WndMain wndMain = (WndMain)App.Current.MainWindow;
            wndMain.ClearWndWind();
        }

        /// <summary>Mouse Left Button Down event handler.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Mouse Event parameters.</param>
        private void wndWing_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        /// <summary>Mouse Left Button Up event handler.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Mouse Event parameters.</param>
        private void wndWing_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _UpdateLocationOffset();
        }

        /// <summary>Initialized Changed event handler.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void wndWing_Activated(object sender, EventArgs e)
        {
            _UpdateLocationOffset();
        }

        /// <summary>Updates location offset.</summary>
        public void FollowMain()
        {
            WndMain wndMain = (WndMain)App.Current.MainWindow;
            this.Left = wndMain.Left + _offsetX;
            this.Top = wndMain.Top + _offsetY;
        }

        /// <summary>Updates location offset.</summary>
        private void _UpdateLocationOffset()
        {
            WndMain wndMain = (WndMain)App.Current.MainWindow;
            _offsetX = this.Left - wndMain.Left;
            _offsetY = this.Top - wndMain.Top;
        }

        /// <summary>Mouse Left Button Down event handler of the URL column.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Mouse Event parameters.</param>
        private void txbUrl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
            _ShowBrowser();
            e.Handled = true;
        }

        /// <summary>Click event handler of the Close button.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>Click event handler of the Delete button.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            InfoItem item = _infoItemPanel.GetInfoItem();

            System.Windows.Forms.DialogResult ret =
                        System.Windows.Forms.MessageBox.Show(
                                "\"" + item.Title + "\"" + Properties.Resources.CONFIRM_DELETE,
                                App.Current.MainWindow.Title,
                                System.Windows.Forms.MessageBoxButtons.OKCancel,
                                System.Windows.Forms.MessageBoxIcon.Question
                            );
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                WndMain wndMain = (WndMain)App.Current.MainWindow;
                wndMain.RemoveFeedChild(item);
                wndMain.InfoManager.RemoveItems(new int[1] { item.Id });
                Close();
            }
        }

        /// <summary>Click event handler of the Unread Checkbox.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void chkUnread_Click(object sender, RoutedEventArgs e)
        {
            if (_infoItemPanel != null)
            {
                if ((bool)chkUnread.IsChecked)
                    _infoItemPanel.MarkUnread();
                else
                    _infoItemPanel.MarkRead();

                try
                {
                    ThetisCore.Lib.IIpcTaskService ipcTaskService = ThetisCore.Lib.IpcServiceAgent.GetTaskService();
                    if (ipcTaskService != null)
                        ipcTaskService.SetRead(_infoItemPanel.GetInfoItem().Id, !(bool)chkUnread.IsChecked);
                }
                catch (Exception ex)
                {
                    Log.AddError(ex.Message + "\n" + ex.StackTrace);
                }
            }
        }

        /// <summary>Size Changed event handler of the Docking Panel.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void dkpView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (dkpView.Children == null || dkpView.Children.Count <= 0)
                return;

            InfoItemViewer intoViewer = (InfoItemViewer) dkpView.Children[0];
            intoViewer.Width = e.NewSize.Width;
            intoViewer.Height = e.NewSize.Height;
        }

        /// <summary>Click event handler of the Link button.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void btnLink_Click(object sender, RoutedEventArgs e)
        {
            Close();
            _ShowBrowser();
        }

        /// <summary>Open the URL Link in the browser.</summary>
        private void _ShowBrowser()
        {
            if (txbUrl.Text != null && txbUrl.Text.Length > 0)
                System.Diagnostics.Process.Start(txbUrl.Text);
        }
    }
}
