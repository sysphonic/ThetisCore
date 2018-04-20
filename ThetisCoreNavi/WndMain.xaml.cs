/* 
 * WndMain.xaml.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
//#define ZEPTAIR

using System;
using System.IO;        // for Path
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Controls;          // for StackPanel
using System.Windows.Controls.Primitives;   // for ToggleButton
using System.Collections;
using System.Diagnostics;
using System.Runtime.Remoting;
using ThetisCore.Lib;
using ThetisCore.Task;      // for ItemInfo
//using ThetisBrowser;
using Sysphonic.Common;

/*
 *  To work arround the designer's error when loading, add your environment
 * the following variable and restart Visual Studio.
 *    CIDER_IMAGES_DIR = Root path of the ThetisCore Project (not the Solution root)
 */

namespace ThetisCore.Navi
{
    /// <summary>Main Window class</summary>
    public partial class WndMain : Window
    {
        /// <summary>Configuration for ThetisCoreNavi.</summary>
        private NaviConfig _naviConfig = null;

        /// <summary>Information Manager.</summary>
        private InfoManager _infoManager = null;

        /// <summary>Wing Window.</summary>
        private WndWing _wndWing = null;

#if ZEPTAIR
        /// <summary>Zeptair Distribution Folders Window.</summary>
        private WndZeptDistFolders _wndZeptDistFolders = null;
#endif

        /// <summary>Selected panel.</summary>
        private IInfoItemPanel _selPanel = null;

        /// <summary>Mutex to check if the same process exists.</summary>
        private static System.Threading.Mutex _mutex;

        /// <summary>Mutex to do operations on the Panel .</summary>
        private System.Threading.Mutex _mutexPanel = new System.Threading.Mutex();

        /// <summary>Currently selected Menu Button.</summary>
        private ToggleButton _curMenuButton = null;

        /// <summary>Timer to initialize.</summary>
        private System.Threading.Timer _initTimer = null;

        private Hashtable _errCountHash = new Hashtable();

        public enum MENU_MODE
        {
            MENU_MODE_UNKNOWN,
            MENU_MODE_FEED,
            MENU_MODE_ZEPTAIR
        };

        public delegate void UpdateGuiDelegate();
        public delegate void ResizeDelegate(double deltaHorz, double deltaVert);

        public void SetMenuMode(MENU_MODE mode)
        {
            switch (mode)
            {
                case MENU_MODE.MENU_MODE_FEED:
                    UpdateMenuPanel(btnMenuFeed);
                    viewFeed.Visibility = Visibility.Visible;
                    viewZeptair.Visibility = Visibility.Hidden;
                    btnReload.Visibility = Visibility.Visible;
                    btnCheckAll.Visibility = Visibility.Visible;
#if ZEPTAIR
                    if (_wndZeptDistFolders != null)
                    {
                        _wndZeptDistFolders.Close();
                        _wndZeptDistFolders = null;
                    }
#endif
                    break;
                case MENU_MODE.MENU_MODE_ZEPTAIR:
                    UpdateMenuPanel(btnMenuZeptair);
                    viewFeed.Visibility = Visibility.Hidden;
                    viewZeptair.Visibility = Visibility.Visible;
                    btnReload.Visibility = Visibility.Hidden;
                    btnCheckAll.Visibility = Visibility.Hidden;
                    if (_wndWing != null)
                    {
                        _wndWing.Close();
                        _wndWing = null;
                    }

#if ZEPTAIR
                    if (stpZeptair.Children.Count <= 0)
                    {
                        System.Resources.ResourceManager rm = new System.Resources.ResourceManager(
                            "ThetisCoreNavi",
                             System.Reflection.Assembly.GetExecutingAssembly());
                        MenuItemPanel[] panels = new MenuItemPanel[] {
//                              new MenuItemPanel(Properties.Resources.MENUITEM_ZEPT_HISTORY, new MouseButtonEventHandler(menuItemZeptDistFolders_Click)),
                                new MenuItemPanel(Properties.Resources.MENUITEM_ZEPT_DIST_FOLDERS, new MouseButtonEventHandler(menuItemZeptDistFolders_Click))
                              };
                        foreach (MenuItemPanel panel in panels)
                        {
                          panel.Width = stpZeptair.Width;
                          stpZeptair.Children.Add(panel);
                        }
                    }
#endif
/*
                    BrowserConfig browserConfig = BrowserConfig.Load();
                    if (browserConfig.Url == null
                        || browserConfig.Url.Length <= 0)
                    {
                        System.Windows.Forms.MessageBox.Show(
                                            Properties.Resources.ERR_ZEPTMENU_URL_NOT_SET,
                                            @"ERROR",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Error
                                        );
                        break;
                    }
                    RssTargetEntry targetEntry = new RssTargetEntry();
                    targetEntry.Url = browserConfig.Url.Split('?')[0].Replace(@"/rss/rss", @"/rss/zeptmenu");
                    if (browserConfig.UserName != null
                        && browserConfig.UserName.Length > 0)
                    {
                        targetEntry.SetAuth(browserConfig.UserName, browserConfig.Password);
                    }
                    ZeptMenuAgent.GetMenu(targetEntry);
*/
                    break;
                default:
                    break;
            }
        }

        public MENU_MODE GetMenuMode()
        {
            if ((bool)btnMenuFeed.IsChecked)
                return MENU_MODE.MENU_MODE_FEED;

            if ((bool)btnMenuZeptair.IsChecked)
                return MENU_MODE.MENU_MODE_ZEPTAIR;

            return MENU_MODE.MENU_MODE_UNKNOWN;
        }

        /// <summary>Constructor.</summary>
        public WndMain()
        {
            InitializeComponent();

            _CheckProcessDuplicated();

            this.Width = _naviConfig.NaviWidth;
            this.Height = _naviConfig.NaviHeight;

            double workAreaWidth = SystemParameters.WorkArea.Width;
            double workAreaHeight = SystemParameters.WorkArea.Height;
            this.Top = workAreaHeight - this.Height;
            this.Left = workAreaWidth - this.Width;

            SetMenuMode(MENU_MODE.MENU_MODE_FEED); 
     
            resizeTopLeft.HideElements = new FrameworkElement[]{
                viewFeed, grdMenu, grdCtrlPanel
            };

            // IPC
            RemotingConfiguration.Configure(Path.Combine(ThetisCore.Lib.Def.COMMON_CONFIG_ROOT_DIR, @"remotingNavi.config"), false);

            IpcNaviService ipcNaviService = new IpcNaviService();
            ipcNaviService.InfoItemsUpdated += ipcNaviService_InfoItemsUpdated;
#if ZEPTAIR
            ipcNaviService.DistItemsUpdated += ipcNaviService_DistItemsUpdated;
#endif
            ipcNaviService.TaskProcClosing += ipcNaviService_TaskProcClosing;

            RemotingServices.Marshal(ipcNaviService, "ipcNaviService");
        }

        /// <summary>Checks if the same process exists.</summary>
        private void _CheckProcessDuplicated()
        {
            _mutex = new System.Threading.Mutex(false, "ThetisCoreNavi.sysphonic.com");

            if (_mutex.WaitOne(0, false) == false)
            {
                Process prevProcess = CommonUtil.GetPreviousProcess();
                if (prevProcess != null)
                {
                    CommonUtil.WakeupWindow(prevProcess.MainWindowHandle);
                    Environment.Exit(0);
                }
            }
        }

        /// <summary>Initialized event handler.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void wndMain_Initialized(object sender, EventArgs e)
        {
            _naviConfig = NaviConfig.Load();

            _infoManager = new InfoManager();
            _AddFeedItems(_infoManager.Load());

            this.Topmost = true;
        }

        private void wndMain_Activated(object sender, EventArgs e)
        {
            if (_initTimer == null)
            {
                System.Threading.TimerCallback timerDelegate = new System.Threading.TimerCallback(this.Init);
                _initTimer = new System.Threading.Timer(timerDelegate, null, 0, 100);
            }
        }

        /// <summary>Information Manager</summary>
        public InfoManager InfoManager
        {
            get { return _infoManager; }
        }

        /// <summary>Configuration for ThetisCoreNavi.</summary>
        public NaviConfig NaviConfig
        {
            get { return _naviConfig; }
        }

        /// <summary>Wing Window.</summary>
        public WndWing WndWing
        {
            get { return _wndWing; }
        }

#if ZEPTAIR
        /// <summary>Zeptair Distribution Folders Window.</summary>
        public WndZeptDistFolders WndZeptDistFolders
        {
            get { return _wndZeptDistFolders; }
        }
#endif

        /// <summary>Reloads list of information items.</summary>
        private void _Reload()
        {
            if (_wndWing != null)
            {
                _wndWing.Close();
                _wndWing = null;
            }

            ClearFeed();
            _AddFeedItems(_infoManager.Load());
            _LoadThumbs();

            this.Focus();
        }

        /// <summary>Clears Feed view.</summary>
        public void ClearFeed()
        {
            _mutexPanel.WaitOne();

            stpFeed.Children.Clear();

            _mutexPanel.ReleaseMutex();
        }

        /// <summary>Clears Zeptair view.</summary>
        public void ClearZeptair()
        {
            stpZeptair.Children.Clear();
        }

        /// <summary>Forces to close.</summary>
        public void ForceClose()
        {
            this.Close();
        }

        /// <summary>InfoItemsUpdated event handler of IpcNaviService.</summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Arguments.</param>
        public void ipcNaviService_InfoItemsUpdated(object sender, ThetisCore.Lib.InfoItemsUpdatedEventArgs e)
        {
            _mutexPanel.WaitOne();

            stpFeed.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    new UpdateGuiDelegate(this._Reload)
                  );

            _mutexPanel.ReleaseMutex();
        }

#if ZEPTAIR
        /// <summary>DistItemsUpdated event handler of IpcNaviService.</summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Arguments.</param>
        public void ipcNaviService_DistItemsUpdated(object sender, ThetisCore.Lib.DistItemsUpdatedEventArgs e)
        {
            if (_wndZeptDistFolders == null)
                return;

            _wndZeptDistFolders.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    new UpdateGuiDelegate(_wndZeptDistFolders.Reload)
                  );
        }
#endif

        /// <summary>TaskProcClosing event handler of IpcNaviService.</summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Arguments.</param>
        public void ipcNaviService_TaskProcClosing(object sender, ThetisCore.Lib.TaskProcClosingEventArgs e)
        {
            this.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    new UpdateGuiDelegate(this.ForceClose)
                  );
        }

        /// <summary>Inserts item to the head of the view.</summary>
        /// <param name="item">Array of Information Items to add.</param>
        private void _AddFeedItems(ArrayList items)
        {
            if (items == null || items.Count <= 0)
                return;

            _mutexPanel.WaitOne();

            for (int i=0; i< items.Count; i++)
            {
                InfoItemPanel_B panel = new InfoItemPanel_B((InfoItem)items[items.Count - i - 1]);
                panel.Width = stpFeed.Width;
                stpFeed.Children.Insert(0, panel);
            }

            _mutexPanel.ReleaseMutex();
        }

        /// <summary>Invokes initializing the instance.</summary>
        /// <param name="state">State object.</param>
        public void Init(Object state)
        {
            _initTimer.Dispose();

            _mutexPanel.WaitOne();

            stpFeed.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    new UpdateGuiDelegate(_Init)
                  );

            _mutexPanel.ReleaseMutex();
        }

        /// <summary>Initializes the instance.</summary>
        private void _Init()
        {
            _LoadThumbs();

            this.Topmost = false;
        }

        /// <summary>Loads thumbnail images.</summary>
        /// <param name="retryCnt">Retry counter.</param>
        private void _LoadThumbs(int retryCnt=0)
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                return;

            _mutexPanel.WaitOne();

            if (retryCnt <= 0)
                _errCountHash.Clear();

            //for (int i=0; _loadingThumbs && (i < stpFeed.Children.Count); i++)
            //{
            //    IInfoItemPanel infoPanel = (IInfoItemPanel)stpFeed.Children[i];
            try
            {
                foreach (IInfoItemPanel infoPanel in stpFeed.Children)
                {
                    //if (!_loadingThumbs)
                    //    break;

                    if (!infoPanel.HasThumb() || infoPanel.IsThumbLoaded())
                        continue;

                    InfoItem infoItem = infoPanel.GetInfoItem();
                    if (!IsReachableTarget(infoItem.GeneratorId))
                        continue;

                    infoPanel.LoadThumb(infoPanel_Loaded, infoPanel_ImageFailed);
                    System.Windows.Forms.Application.DoEvents();
                }
                _mutexPanel.ReleaseMutex();
            }
            catch (System.InvalidOperationException ex)
            {
                // The enumerator has been invalidated because changes were made to the collection.
                _mutexPanel.ReleaseMutex();
                if (++retryCnt <= 3)
                {
                    Log.AddWarn(String.Format(@"Retrying loading thumbnails .. #{0}", retryCnt));
                    _LoadThumbs(retryCnt);
                }
                else
                {
                    Log.AddError(ex.Message + "\n" + ex.StackTrace);
                }
            }
        }

        public bool IsReachableTarget(string generatorId)
        {
            int errCnt = 0;
            if (_errCountHash[generatorId] != null)
                errCnt = (int)_errCountHash[generatorId];
            return (errCnt < 2);
        }

        void infoPanel_Loaded(object sender, RoutedEventArgs e)
        {
            Image imgThumb = (Image)sender;
            IInfoItemPanel infoPanel = (IInfoItemPanel) ((Grid)imgThumb.Parent).Parent;
            InfoItem infoItem = infoPanel.GetInfoItem();

            _errCountHash[infoItem.GeneratorId] = 0;
        }

        public void infoPanel_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Image imgThumb = (Image)sender;
            IInfoItemPanel infoPanel = (IInfoItemPanel) ((Grid)imgThumb.Parent).Parent;
            InfoItem infoItem = infoPanel.GetInfoItem();

            int errCnt = 0;
            if (_errCountHash[infoItem.GeneratorId] != null)
                errCnt = (int)_errCountHash[infoItem.GeneratorId];

            _errCountHash[infoItem.GeneratorId] = ++errCnt;
        }

        /// <summary>Removes the specified item from Feed view.</summary>
        /// <param name="item">Information Item to remove.</param>
        public void RemoveFeedChild(InfoItem item)
        {
            _mutexPanel.WaitOne();

            foreach (UIElement obj in stpFeed.Children)
            {
                if (typeof(IInfoItemPanel).IsInstanceOfType(obj))
                {
                    if (((IInfoItemPanel)obj).GetInfoItem() == item)
                    {
                        stpFeed.Children.Remove(obj);
                        break;
                    }
                }
            }

            _mutexPanel.ReleaseMutex();
        }

        /// <summary>Click event handler of each information item.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        public void infoItem_Click(object sender, MouseButtonEventArgs e)
        {
            if (_selPanel != null)
                _selPanel.SetSelected(false);

            _selPanel = (IInfoItemPanel)sender;
            _selPanel.SetSelected(true);

            if (_wndWing == null)
            {
                _wndWing = new WndWing();
                _wndWing.Owner = this;
                _wndWing.Width = _naviConfig.WingWidth;
                _wndWing.Height = _naviConfig.WingHeight;

                System.Drawing.Rectangle rec = Screen.GetWorkingArea(new System.Drawing.Point(0, 0));
                double workAreaWidth = SystemParameters.WorkArea.Width;
            	double workAreaHeight = SystemParameters.WorkArea.Height;
                double marginLeft = this.Left;
                double marginRight = workAreaWidth - (this.Left + this.Width);
                if (marginLeft > marginRight)
                    _wndWing.Left = this.Left - _wndWing.Width;
                else
                    _wndWing.Left = this.Left + this.Width;
                _wndWing.Top = this.Top + 30.0;
                if (workAreaHeight - _wndWing.Top < _wndWing.Height)
                	_wndWing.Top = workAreaHeight - _wndWing.Height;
                _wndWing.Show();
            }

            if (_wndWing.InfoItemPanel != null
                && _wndWing.InfoItemPanel.GetInfoItem() == _selPanel.GetInfoItem())
            {
                _wndWing.Close();
                _wndWing = null;
            }
            else
            {
                _wndWing.chkUnread.IsChecked = false;

                _wndWing.ShowInfoItem(_selPanel);
                markRead(_selPanel, true);
            }
        }

        /// <summary>Click event handler of each menu item.</summary>
        /// <param name="panel">Information Item Panel.</param>
        /// <param name="isRead">Flag whether read or not.</param>
        public void markRead(IInfoItemPanel panel, bool isRead)
        {
            if (isRead)
                panel.MarkRead();
            else
                panel.MarkUnread();
            try
            {
                ThetisCore.Lib.IIpcTaskService ipcTaskService = ThetisCore.Lib.IpcServiceAgent.GetTaskService();
                if (ipcTaskService != null)
                    ipcTaskService.SetRead(panel.GetInfoItem().Id, isRead);
            }
            catch (Exception ex)
            {
                Log.AddError(ex.Message + "\n" + ex.StackTrace);
            }
        }

#if ZEPTAIR
        /// <summary>Click event handler of each menu item.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        public void menuItemZeptDistFolders_Click(object sender, MouseButtonEventArgs e)
        {
            if (_wndZeptDistFolders == null)
            {
                _wndZeptDistFolders = new WndZeptDistFolders();
                _wndZeptDistFolders.Owner = this;
                _wndZeptDistFolders.Width = _naviConfig.WingWidth;
                _wndZeptDistFolders.Height = _naviConfig.WingHeight;

                System.Drawing.Rectangle rec = Screen.GetWorkingArea(new System.Drawing.Point(0, 0));
                double marginLeft = this.Left;
                double marginRight = rec.Width - (this.Left + this.Width);
                if (marginLeft > marginRight)
                    _wndZeptDistFolders.Left = this.Left - _wndZeptDistFolders.Width;
                else
                    _wndZeptDistFolders.Left = this.Left + this.Width;
                _wndZeptDistFolders.Top = this.Top + 30.0;
                _wndZeptDistFolders.Show();
            }

            if (_selPanel != null)
                _selPanel.SetSelected(false);

            _selPanel = (IInfoItemPanel) sender;
            _selPanel.SetSelected(true);
            _wndZeptDistFolders.Load(_selPanel.GetInfoItem().Title);
            _wndZeptDistFolders.Focus();
        }

        /// <summary>Clears the reference to the Zeptair Distribution Folders Window.</summary>
        public void ClearWndZeptDistFolders()
        {
            _wndZeptDistFolders = null;
        }
#endif

        /// <summary>Clears the reference to the Wing Window.</summary>
        public void ClearWndWind()
        {
            _wndWing = null;
        }

        /// <summary>Mouse Left Button Down event handler.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Mouse Event parameters.</param>
        private void wndMain_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        /// <summary>Updates status of Menu buttons.</summary>
        /// <param name="clickedBtn">Clicked ToggleButton.</param>
        private void UpdateMenuPanel(ToggleButton clickedBtn)
        {
            if (_curMenuButton != null)
            {
                _curMenuButton.IsChecked = false;
                _curMenuButton.BorderBrush = null;
            }
            _curMenuButton = clickedBtn;
            _curMenuButton.IsChecked = true;
        }

        /// <summary>Click event handler of the Feed Menu button.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void btnMenuFeed_Click(object sender, RoutedEventArgs e)
        {
          SetMenuMode(MENU_MODE.MENU_MODE_FEED);
        }

        /// <summary>Click event handler of the Zeptair Menu button.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void btnMenuZeptair_Click(object sender, RoutedEventArgs e)
        {
          SetMenuMode(MENU_MODE.MENU_MODE_ZEPTAIR);
        }

        /// <summary>Closing event handler.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void wndMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_wndWing != null)
            {
                _wndWing.Close();
                _wndWing = null;
            }

            _naviConfig.NaviWidth = Width;
            _naviConfig.NaviHeight = Height;
            _naviConfig.Save();
        }

        /// <summary>Click event handler of the Close button.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (_wndWing != null)
            {
                _wndWing.Close();
                _wndWing = null;
            }

            Close();
        }

        /// <summary>Click event handler of the Reload button.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>Click event handler of the Check All button.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void btnCheckAll_Click(object sender, RoutedEventArgs e)
        {
            _mutexPanel.WaitOne();

            bool allRead = true;
            foreach (IInfoItemPanel infoPanel in stpFeed.Children)
            {
                if (!infoPanel.GetInfoItem().IsRead)
                {
                    allRead = false;
                    break;
                }
            }
            bool isRead = (!allRead);
            foreach (IInfoItemPanel infoPanel in stpFeed.Children)
            {
                if (infoPanel.GetInfoItem().IsRead != isRead)
                    markRead(infoPanel, isRead);
            }
            _mutexPanel.ReleaseMutex();

            if (_wndWing != null)
                _wndWing.chkUnread.IsChecked = !isRead;
        }

        /// <summary>Location Changed event handler.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void wndMain_LocationChanged(object sender, EventArgs e)
        {
            if (_wndWing != null)
                _wndWing.FollowMain();
#if ZEPTAIR
            if (_wndZeptDistFolders != null)
                _wndZeptDistFolders.FollowMain();
#endif
        }

        /// <summary>Common procedure of Size Changed event for StackPanels.</summary>
        /// <param name="stp">Target StackPanel.</param>
        private void _onSizeChanged(StackPanel stp)
        {
            double width = stp.Width;
            if (Double.IsNaN(width))
                return;

            foreach (UIElement panel in stp.Children)
            {
                if (panel.GetType() == typeof(InfoItemPanel_A))
                    ((InfoItemPanel_A)panel).Width = width;
                else if (panel.GetType() == typeof(InfoItemPanel_B))
                    ((InfoItemPanel_B)panel).Width = width;
                else if (panel.GetType() == typeof(MenuItemPanel))
                    ((MenuItemPanel)panel).Width = width;
            }
        }

        /// <summary>Size Changed event handler.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void stpFeed_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _onSizeChanged((StackPanel)e.Source);
        }

        /// <summary>Size Changed event handler.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void stpZeptair_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _onSizeChanged((StackPanel)e.Source);
        }

        /// <summary>Key Down event handler.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void viewFeed_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Return:
                case Key.Space:
                    {
                        IInfoItemPanel newSelPanel = null;

                        _mutexPanel.WaitOne();
                        foreach (IInfoItemPanel infoPanel in stpFeed.Children)
                        {
                            if (infoPanel.IsMouseHovering())
                            {
                                newSelPanel = infoPanel;
                                break;
                            }
                        }
                        _mutexPanel.ReleaseMutex();

                        if (newSelPanel != null)
                        {
                            if (newSelPanel.GetType() == typeof(InfoItemPanel_A)
                                || newSelPanel.GetType() == typeof(InfoItemPanel_B))
                                infoItem_Click(newSelPanel, null);
#if ZEPTAIR
                            else if (newSelPanel.GetType() == typeof(MenuItemPanel))
                                menuItemZeptDistFolders_Click(newSelPanel, null);
#endif
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
