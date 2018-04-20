/* 
 * TaskMain.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
#define ZEPTAIR

using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Globalization;
using System.IO;
using Sysphonic.Common;
#if ZEPTAIR
using Zeptair.Lib.Service;
using Zeptair.Lib.Common;
using Zeptair.Lib.License;
#endif

namespace ThetisCore.Task
{
    /// <summary>Application Main class.</summary>
    class TaskMain : SynchronizeInvoker
    {
        /// <summary>Application title.</summary>
#if ZEPTAIR
        public static string APP_TITLE = @"ThetisCore";
//      public static string APP_TITLE = @"Zeptair Client";
#else
        public static string APP_TITLE = @"ThetisCore";
#endif

        /// <summary>Self instance.</summary>
        private static TaskMain _self = null;

        /// <summary>RSS Manager</summary>
        private RssManager _rssManager = null;

        /// <summary>Information Manager</summary>
        //private InfoManager _infoManager = null;

#if ZEPTAIR
        /// <summary>Distribution Manager</summary>
        private ZeptDistManager _zeptDistManager = null;
#endif

        /// <summary>Notify Icon on the Task Tray</summary>
        private NotifyIcon _notifyIcon = null;

        private ContextMenu _contextMenu = null;

        /// <summary>'RSS Settings' Menu Item</summary>
        private MenuItem _menuItemSettings = null;

#if ZEPTAIR
        /// <summary>Zeptair Client Configuration Parameters.</summary>
        Zeptair.Lib.Common.ConfParam _zeptConf = null;

        public Zeptair.Lib.Common.ConfParam ZeptConf(bool reload)
        {
            if (_zeptConf == null || reload)
                _zeptConf = Zeptair.Lib.Common.ConfParam.Load();
            return _zeptConf;
        }

        /// <summary>'Zeptair VPN Connect' Menu Item</summary>
        private MenuItem _menuItemZeptConnect = null;

        /// <summary>'Zeptair Settings' Menu Item</summary>
        private MenuItem _menuItemZeptSettings = null;

        /// <summary>'Zeptair VPN Disconnect' Menu Item</summary>
        private MenuItem _menuItemZeptDisconnect = null;

        /// <summary>Polling timer for Zeptair Service State</summary>
        private System.Windows.Forms.Timer tmrZeptSvcWatcher = null;

        /// <summary>Flag to keep whether Auth. Failed message has been shown.</summary>
        private bool _dispAuthFailed = false;

        private bool _hasValidZeptLicKey = false;
#endif

        /// <summary>Mutex to check if the same process exists.</summary>
        private static System.Threading.Mutex _mutex;


        /// <summary>Constructor</summary>
        private TaskMain()
        {
            _CheckProcessDuplicated();

#if ZEPTAIR
            Zeptair.Lib.Common.ConfParam zeptConf = ZeptConf(true);
#endif

            _contextMenu = new ContextMenu();

            int itemIdx = 0;
            MenuItem menuItemNavi = new MenuItem();
            menuItemNavi.Index = itemIdx;
            menuItemNavi.Text = Properties.Resources.MENU_NAVI_PANEL;
            menuItemNavi.Click += new System.EventHandler(menuItemNavi_Click);
            itemIdx++;

            _menuItemSettings = new MenuItem();
            _menuItemSettings.Index = itemIdx;
            _menuItemSettings.Text = Properties.Resources.MENU_RSS_SETTINGS;
            _menuItemSettings.Click += new System.EventHandler(menuItemSettings_Click);
            itemIdx++;

            MenuItem menuItemBrowser = new MenuItem();
            menuItemBrowser.Index = itemIdx;
#if ZEPTAIR
//          menuItemBrowser.Text = Properties.Resources.MENU_ZEPT_BROWSER;
            menuItemBrowser.Text = Properties.Resources.MENU_THETIS_BROWSER;
#else
            menuItemBrowser.Text = Properties.Resources.MENU_THETIS_BROWSER;
#endif
            menuItemBrowser.Click += new System.EventHandler(menuItemBrowser_Click);
            itemIdx++;

            MenuItem menuItemSeparator1 = new MenuItem();
            menuItemSeparator1.Index = itemIdx;
            itemIdx++;

#if ZEPTAIR
            _menuItemZeptConnect = new MenuItem();
            _menuItemZeptConnect.Index = itemIdx;
            _menuItemZeptConnect.Text = Properties.Resources.MENU_ZEPT_CONNECT;
            _menuItemZeptConnect.Click += new System.EventHandler(menuItemZeptConnect_Click);
            itemIdx++;

            _menuItemZeptSettings = new MenuItem();
            _menuItemZeptSettings.Index = itemIdx;
            _menuItemZeptSettings.Text = Properties.Resources.MENU_ZEPT_SETTINGS;
            _menuItemZeptSettings.Click += new System.EventHandler(menuItemZeptSettings_Click);
            if (zeptConf.HideSettingsMenu)
                _menuItemZeptSettings.Visible = false;
            itemIdx++;

            _menuItemZeptDisconnect = new MenuItem();
            _menuItemZeptDisconnect.Index = itemIdx;
            _menuItemZeptDisconnect.Text = Properties.Resources.MENU_ZEPT_DISCONNECT;
            _menuItemZeptDisconnect.Click += new System.EventHandler(menuItemZeptDisconnect_Click);

            _menuItemZeptDisconnect.Visible = false;    // Hide as default

            itemIdx++;
#endif
            MenuItem menuItemExit = new MenuItem();
            menuItemExit.Index = itemIdx;
            menuItemExit.Text = Properties.Resources.MENU_EXIT;
            menuItemExit.Click += new System.EventHandler(menuItemExit_Click);
            itemIdx++;

            _contextMenu.MenuItems.AddRange(
                    new MenuItem[] {
                        menuItemNavi,
                        _menuItemSettings,
//                        new MenuItem("-"),
//                        menuItemBrowser,
#if ZEPTAIR
                        new MenuItem("-"),
                        _menuItemZeptConnect,
                        _menuItemZeptSettings,
                        _menuItemZeptDisconnect,
#endif
                        new MenuItem("-"),
                        menuItemExit
                    }
                  );

            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = Properties.Resources.AppIcon;
            _notifyIcon.Text = TaskMain.APP_TITLE + @" v" + CommonUtil.TrimVersion(Application.ProductVersion);
            _notifyIcon.ContextMenu = _contextMenu;
            _notifyIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(notifyIcon_MouseDown);
            _notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(notifyIcon_MouseDoubleClick);
            _notifyIcon.Visible = true;

#if ZEPTAIR
            this.tmrZeptSvcWatcher = new System.Windows.Forms.Timer();
            this.tmrZeptSvcWatcher.Interval = 2000;
            this.tmrZeptSvcWatcher.Tick += new System.EventHandler(this.tmrZeptSvcWatcher_Tick);
            this.tmrZeptSvcWatcher.Enabled = true;
#endif
        }

        /// <summary>Initialize.</summary>
        public static void Init()
        {
            _self = new TaskMain();

            /*
             * IPC
             */
            RemotingConfiguration.Configure(Path.Combine(ThetisCore.Lib.Def.COMMON_CONFIG_ROOT_DIR, @"remotingTask.config"), false);

            IpcTaskService ipcTaskService = new IpcTaskService();
            ipcTaskService.RssTargetInfosUpdated += _self.ipcTaskService_RssTargetInfosUpdated;

            RemotingServices.Marshal(ipcTaskService, "ipcTaskService");

#if ZEPTAIR
            Zeptair.Lib.Common.ConfParam zeptConf = TaskMain.Instance.ZeptConf(false);
            /*
             * License-Key Validation
             */
            LicKeyInfo licKeyInfo = LicKeyParser.Parse(zeptConf.LicenseKey);
            if (licKeyInfo == null || licKeyInfo.GetErrorMsg() != null)
            {
                _self._hasValidZeptLicKey = false;
 #if true
                _self._menuItemZeptConnect.Enabled = false;
 #else
                CommonUtil.StartProcessWithCallback(
                                Path.Combine(CommonUtil.GetAppPath(), @"ZeptairClientConf.exe"),
                                @"/default=tbiLicenseKey /edit",
                                _self,
                                new EventHandler(_self.zeptConfProcess_Exited)
                            );
 #endif
            }
            else
                _self._hasValidZeptLicKey = true;
#endif

            _self._rssManager = new RssManager();
            if (_self._rssManager.LoadWatchers())
                _self._rssManager.Start();
            //else
            //    Process.Start("ThetisCoreConf.exe");

#if ZEPTAIR
            _self._zeptDistManager = new ZeptDistManager();
            _self._zeptDistManager.Init(zeptConf.AcceptCmd);
#endif
        }

        /// <summary>Checks if the same process exists.</summary>
        private void _CheckProcessDuplicated()
        {
            _mutex = new System.Threading.Mutex(false, @"ThetisCoreTask.sysphonic.com");

            if (_mutex.WaitOne(0, false) == false)
            {
                System.Windows.Forms.MessageBox.Show(
                                    Properties.Resources.ERR_PROCESS_EXISTS,
                                    TaskMain.APP_TITLE,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information
                                );
                HideNotifyIcon();
                Environment.Exit(0);
            }
        }

        /// <summary>RssTargetInfosUpdated event handler of IpcNaviService.</summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Arguments.</param>
        public void ipcTaskService_RssTargetInfosUpdated(object sender, ThetisCore.Lib.RssTargetInfosUpdatedEventArgs e)
        {
            Restart();
        }

        /// <summary>Self instance.</summary>
        public static TaskMain Instance
        {
            get { return _self; }
        }

        /// <summary>Information Manager</summary>
        public InfoManager InfoManager
        {
            get { return new InfoManager(); /* _infoManager; */ }
        }

#if ZEPTAIR
        public bool HasValidZeptLicKey
        {
            get { return _hasValidZeptLicKey; }
        }

        /// <summary>Zeptair Distribution Manager</summary>
        public ZeptDistManager ZeptDistManager
        {
            get { return _zeptDistManager; }
        }
#endif

        /// <summary>RSS Manager</summary>
        public RssManager RssManager
        {
            get { return _rssManager; }
        }

        /// <summary>Restarts.</summary>
        public void Restart()
        {
            _rssManager.Stop();
            _rssManager.LoadWatchers();
            _rssManager.Start();

            //_infoManager.Load();
        }

#if ZEPTAIR
        public void RestartZeptDist(bool reloadZeptConf)
        {
            if (reloadZeptConf)
            {
                Zeptair.Lib.Common.ConfParam zeptConf = this.ZeptConf(true);

                _zeptDistManager.StopAll();
                _zeptDistManager.Init(zeptConf.AcceptCmd);
            }

            _rssManager.RestartZeptDist();
        }
#endif

        /// <summary>Hides Notify Icon in the task tray.</summary>
        public void HideNotifyIcon()
        {
            if (_notifyIcon != null)
                _notifyIcon.Visible = false;
        }

        /// <summary>Click event handler of the Navi item in the menu of the task tray.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void menuItemNavi_Click(object sender, EventArgs e)
        {
            Process[] prcs = Process.GetProcessesByName("ThetisCoreNavi");

            if (prcs != null && prcs.Length > 0)
                CommonUtil.WakeupWindow(prcs[0].MainWindowHandle);
            else
                Process.Start(Path.Combine(CommonUtil.GetAppPath(), "ThetisCoreNavi.exe"));
        }

        /// <summary>Click event handler of the Settings item in the menu of the task tray.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void menuItemSettings_Click(object sender, EventArgs e)
        {
            Process[] prcs = Process.GetProcessesByName(@"ThetisCoreConf");

            if (prcs != null && prcs.Length > 0)
                CommonUtil.WakeupWindow(prcs[0].MainWindowHandle);
            else
            {
                string opt = @"";
#if ZEPTAIR
                if (!_hasValidZeptLicKey)
                    opt = @"/needLicKey";
#endif

                CommonUtil.StartProcessWithCallback(
                                Path.Combine(CommonUtil.GetAppPath(), @"ThetisCoreConf.exe"),
                                opt,
                                _self,
                                new EventHandler(_self.settingsProcess_Exited)
                            );

                _notifyIcon.ContextMenu = null;
            }
        }

        /// <summary>Event handler of Exit from RSS Settings.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void settingsProcess_Exited(object sender, EventArgs e)
        {
            _notifyIcon.ContextMenu = _contextMenu;
        }

#if ZEPTAIR
        private bool _DoStartZeptair()
        {
            ushort res_code = 0;
            if (!ClientServiceAgent.OnStart(ref res_code))
            {
                string msg = Properties.Resources.ERR_FAILED_TO_ACTIVATE_SVC;
                switch (res_code)
                {
                    case ClientServiceAgent.RET_ON_START_ERR_LICKEY:
                        msg += "\n" + Properties.Resources.ERR_ON_START_LICKEY;
                        break;
                    default:
                        break;
                }
                System.Windows.Forms.MessageBox.Show(
                                    msg,
                                    TaskMain.APP_TITLE,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error
                                );
                return false;
            }
            return true;
        }
#endif

        /// <summary>Click event handler of the Open Thetis item in the menu of the task tray.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void menuItemBrowser_Click(object sender, EventArgs e)
        {
            Process[] prcs = Process.GetProcessesByName(@"ThetisBrowser");

            if (prcs != null && prcs.Length > 0)
                CommonUtil.WakeupWindow(prcs[0].MainWindowHandle);
            else
                Process.Start(Path.Combine(CommonUtil.GetAppPath(), @"ThetisBrowser.exe"));
        }

#if ZEPTAIR
        /// <summary>Click event handler of the Zeptair Connect item in the menu of the task tray.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void menuItemZeptConnect_Click(object sender, EventArgs e)
        {
            Zeptair.Lib.Common.ConfParam zeptConf = ZeptConf(true);

            UpdateZeptMenus(true, zeptConf);

            if (zeptConf.ServerAddress == null
                || zeptConf.ServerAddress.Length <= 0
                || zeptConf.ServerPort <= 0)
            {
                bool ret = CommonUtil.StartProcessWithCallback(
                                Path.Combine(CommonUtil.GetAppPath(), @"ZeptairClientConf.exe"),
                                @"/default=tbiServerInfo",
                                this,
                                new EventHandler(zeptConfProcess_Exited)
                            );
                _notifyIcon.ContextMenu = null;
                if (!ret)
                  UpdateZeptMenus(false, zeptConf);
                return;
            }

            if (!_DoStartZeptair())
            {
                UpdateZeptMenus(false, zeptConf);
                return;
            }
        }

        /// <summary>Click event handler of the Zeptair Settings item in the menu of the task tray.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void menuItemZeptSettings_Click(object sender, EventArgs e)
        {
            Process[] prcs = Process.GetProcessesByName(@"ZeptairClientConf");

            if (prcs != null && prcs.Length > 0)
                CommonUtil.WakeupWindow(prcs[0].MainWindowHandle);
            else
            {
                string optDefault = @"";
                if (_hasValidZeptLicKey)
                    optDefault = @"/default=tbiServerInfo";
                else
                    optDefault = @"/default=tbiLicenseKey /edit";

                CommonUtil.StartProcessWithCallback(
                                Path.Combine(CommonUtil.GetAppPath(), @"ZeptairClientConf.exe"),
                                optDefault,
                                this,
                                new EventHandler(this.zeptConfProcess_Exited)
                            );
                _notifyIcon.ContextMenu = null;
            }
        }

        /// <summary>Event handler of Exit from Zeptair Client Configuration.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void zeptConfProcess_Exited(object sender, EventArgs e)
        {
            _notifyIcon.ContextMenu = _contextMenu;

            Zeptair.Lib.Common.ConfParam zeptConf = ZeptConf(true);

            UpdateZeptMenus(IsZeptSvcStarted(), zeptConf);

            /*
             * License-Key RE-Validation
             */
            LicKeyInfo licKeyInfo = LicKeyParser.Parse(zeptConf.LicenseKey);
            if (licKeyInfo == null || licKeyInfo.GetErrorMsg() != null)
            {
                _hasValidZeptLicKey = false;
#if true
                _menuItemZeptConnect.Enabled = false;
#else
                System.Windows.Forms.MessageBox.Show(
                                    Properties.Resources.ERR_CANNOT_CONFIRM_VALID_LIC_KEY,
                                    TaskMain.APP_TITLE,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error
                                );
                this.HideNotifyIcon();
                Environment.Exit(0);
#endif
            }
            else
            {
                _hasValidZeptLicKey = true;
                _menuItemZeptConnect.Enabled = true;
            }

            if (this.IsZeptSvcStarted())
            {
                if (zeptConf.ServerAddress == null
                    || zeptConf.ServerAddress.Length <= 0
                    || zeptConf.ServerPort <= 0)
                {
                    System.Windows.Forms.MessageBox.Show(
                                        Properties.Resources.ERR_ZEPT_SVC_START_CANCELED,
                                        TaskMain.APP_TITLE,
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error
                                    );
                    this.UpdateZeptMenus(false, zeptConf);
                    return;
                }

                if (!_DoStartZeptair())
                {
                    UpdateZeptMenus(false, zeptConf);
                    return;
                }
            }
        }

        /// <summary>Click event handler of the Zeptair Disconnect item in the menu of the task tray.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void menuItemZeptDisconnect_Click(object sender, EventArgs e)
        {
            Zeptair.Lib.Common.ConfParam zeptConf = ZeptConf(false);
            UpdateZeptMenus(false, zeptConf);

            if (!ClientServiceAgent.OnStop())
            {
                System.Windows.Forms.MessageBox.Show(
                                    Properties.Resources.ERR_FAILED_TO_DISABLE_SVC,
                                    TaskMain.APP_TITLE,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error
                                );
                UpdateZeptMenus(true, zeptConf);
            }
        }

        /// <summary>Gets flag whether Zeptair Client Sevice is (being) started.</summary>
        /// <return>Flag whether Zeptair Client Sevice is (being) started.</return>
        public bool IsZeptSvcStarted()
        {
            return _menuItemZeptDisconnect.Visible;
        }

        /// <summary>Update Menus about Zeptair Client.</summary>
        /// <param name="sw">Flag whether Zeptair Client Sevice is (being) started.</param>
        /// <param name="zeptConf">Zeptair Client Configuration parameters.</param>
        public void UpdateZeptMenus(bool sw, Zeptair.Lib.Common.ConfParam zeptConf)
        {
            _menuItemZeptConnect.Visible = !sw;
            if (zeptConf.HideSettingsMenu)
                _menuItemZeptSettings.Visible = false;
            else
                _menuItemZeptSettings.Visible = !sw;
            _menuItemZeptDisconnect.Visible = sw;
        }

        /// <summary>Tick event handler of the polling timer for Zeptair Service State.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void tmrZeptSvcWatcher_Tick(object sender, EventArgs e)
        {
            UpdateTaskTrayIcon();
        }

        /// <summary>Updates Task tray Icon.</summary>
        private void UpdateTaskTrayIcon()
        {
            int act_state = ClientServiceAgent.ACT_STATE_UNKNOWN;
            int icon_state = ClientServiceAgent.ICON_STATE_UNKNOWN;

            if (!ClientServiceAgent.GetCurrentState(ref act_state, ref icon_state))
                return;

            Zeptair.Lib.Common.ConfParam zeptConf = ZeptConf(false);

            if (act_state != ClientServiceAgent.ACT_STATE_AUTH_FAILED)
                _dispAuthFailed = false;

            switch (act_state)
            {
                case ClientServiceAgent.ACT_STATE_FOR_AUTH:
                    this.UpdateZeptMenus(true, zeptConf);
                    break;
                case ClientServiceAgent.ACT_STATE_ON:
                    this.UpdateZeptMenus(true, zeptConf);
                    break;
                case ClientServiceAgent.ACT_STATE_OFF:
                    this.UpdateZeptMenus(false, zeptConf);
                    break;
                case ClientServiceAgent.ACT_STATE_AUTH_FAILED:
                    this.UpdateZeptMenus(false, zeptConf);
                    if (!_dispAuthFailed)
                    {
                        _dispAuthFailed = true;
                        System.Windows.Forms.MessageBox.Show(
                                            Properties.Resources.ERR_ON_START_AUTH_FAILED,
                                            TaskMain.APP_TITLE,
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Error
                                        );
                    }
                    break;
                default:
                    break;
            }

            _notifyIcon.Icon = Properties.Resources.AppIcon;

            switch (icon_state)
            {
                case ClientServiceAgent.ICON_STATE_DISABLED:
                    _notifyIcon.Icon = Properties.Resources.AppIcon;
                    break;
                case ClientServiceAgent.ICON_STATE_ACTIVATED:
                    _notifyIcon.Icon = Properties.Resources.activated;
                    break;
                case ClientServiceAgent.ICON_STATE_STANDBY:
                    _notifyIcon.Icon = Properties.Resources.standby;
                    break;
                case ClientServiceAgent.ICON_STATE_UNDEFINED1:
                    _notifyIcon.Icon = Properties.Resources.undefined;
                    break;
                case ClientServiceAgent.ICON_STATE_UNDEFINED2:
                    _notifyIcon.Icon = Properties.Resources.undefined;
                    break;
                case ClientServiceAgent.ICON_STATE_UNAVAILABLE:
                    _notifyIcon.Icon = Properties.Resources.unavailable;
                    break;
                case ClientServiceAgent.ICON_STATE_UNKNOWN:
                    break;
                case ClientServiceAgent.ICON_STATE_RETRY1:
                    break;
                case ClientServiceAgent.ICON_STATE_RETRY2:
                    break;
                default:
                    break;
            }
        }
#endif

        /// <summary>Click event handler of the Exit item in the menu of the task tray.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void menuItemExit_Click(object sender, EventArgs e)
        {
            ThetisCore.Lib.IIpcNaviService ipcNaviService = ThetisCore.Lib.IpcServiceAgent.GetNaviService();
            if (ipcNaviService != null)
                ipcNaviService.FireEventTaskProcClosing();

            ThetisCore.Lib.IIpcConfService ipcConfService = ThetisCore.Lib.IpcServiceAgent.GetConfService();
            if (ipcConfService != null)
                ipcConfService.FireEventTaskProcClosing();

            if (_rssManager != null)
                _rssManager.Stop();

            HideNotifyIcon();
            Application.Exit();
        }

        /// <summary>Mouse Down event handler of the notify icon.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Mouse Event parameters.</param>
        private void notifyIcon_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                if (_notifyIcon.ContextMenu == null)
                {
                    string[] subProcs = new string[] { @"ThetisCoreConf", @"ZeptairClientConf" };
                    foreach (string subProc in subProcs)
                    {
                        Process[] prcs = Process.GetProcessesByName(subProc);

                        if (prcs != null && prcs.Length > 0)
                        {
                            CommonUtil.WakeupWindow(prcs[0].MainWindowHandle);
                            break;
                        }
                    }
                }
                else
                {
                    Sysphonic.Common.KeyMouse.RightClick(e.X, e.Y);
                    //_notifyIcon.ContextMenu.Show(Application.OpenForms[0], new System.Drawing.Point(e.X, e.Y));
                }
            }
        }

        /// <summary>Mouse Double Click event handler of the notify icon.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Mouse Event parameters.</param>
        private void notifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
/*
            Process[] prcs = Process.GetProcessesByName("ThetisCoreNavi");

            if (prcs != null && prcs.Length > 0)
                CommonUtil.WakeupWindow(prcs[0].MainWindowHandle);
            else
                Process.Start("ThetisCoreNavi.exe");

*/
        }
    }
}
