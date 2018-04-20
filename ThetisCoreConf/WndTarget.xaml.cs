/* 
 * WndTarget.xaml.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System;
using System.Windows;
using System.Windows.Controls;
using System.Net;
using System.Collections;
using Rss;
using Sysphonic.Common;
using ThetisCore.Task;

namespace ThetisCore.Conf
{
    /// <summary>Target Settings window.</summary>
    public partial class WndTarget : Window
    {
        /// <summary>Index of the current RSS Information (-1 when creating).</summary>
        int _infoIdx = -1;

        /// <summary>Zeptair Dist. flag.</summary>
        bool _isZeptDist = false;


        /// <summary>Constructor (For Add).</summary>
        /// <param name="isZeptDist">Zeptair Dist. flag.</param>
        public WndTarget(bool isZeptDist=false)
        {
            InitializeComponent();

            _isZeptDist = isZeptDist;
            if (_isZeptDist)
              tbxTitle.Text = ThetisCore.Lib.Properties.Resources.ZEPTAIR_DISTRIBUTION;

            grdAuth.Visibility = Visibility.Hidden;
            grdAuth.Height = 0;
            scbInterval.Value = RssTargetInfo.POLLING_INTERVAL_DEF;
        }

        /// <summary>Constructor (For Edit).</summary>
        /// <param name="info">Target RSS Information.</param>
        public WndTarget(RssTargetInfo info)
        {
            InitializeComponent();

            _infoIdx = info.Idx;
            _isZeptDist = info.IsZeptDist;

            tbxTitle.Text = info.Title;
            tbxUrl.Text = info.Url;
            if (info.UserName != null && info.UserName.Length > 0)
            {
                chkAuth.IsChecked = true;
                tbxUserName.Text = info.UserName;
                tbxPassword.Password = info.Password;
            }
            else
            {
                grdAuth.Visibility = Visibility.Hidden;
                grdAuth.Height = 0;
            }
            scbInterval.Minimum = RssTargetInfo.POLLING_INTERVAL_MIN;
            scbInterval.Value = info.PollingInterval;
        }

        /// <summary>Checks if the specified parameters are valid and acceptable.</summary>
        /// <returns>A RSS Information instance if the parameters are valid, null otherwise.</returns>
        private RssTargetInfo _CheckParams()
        {
            WndSettings wndSettings = (WndSettings)this.Owner;
            RssTargetInfo info = null;
            ArrayList targetList = null;
            if (_isZeptDist)
              targetList = wndSettings.ZeptDistTargetInfos;
            else
              targetList = wndSettings.RssTargetInfos;

            if (_infoIdx < 0)
            {
                info = new RssTargetInfo();
                info.IsZeptDist = _isZeptDist;
            }
            else
                info = (RssTargetInfo) targetList[_infoIdx];

            info.Title = tbxTitle.Text;
            if (info.Title.Length <= 0)
            {
                System.Windows.Forms.MessageBox.Show(
                                    Properties.Resources.SPECIFY_SETTINGS_TITLE,
                                    this.Title,
                                    System.Windows.Forms.MessageBoxButtons.OK,
                                    System.Windows.Forms.MessageBoxIcon.Exclamation
                                );
                return null;
            }

            if (_infoIdx >= 0)
            {
                int idx = 0;
                foreach (RssTargetInfo targetInfo in targetList)
                {
                    if (_infoIdx != idx && info.Title == targetInfo.Title)
                    {
                        System.Windows.Forms.MessageBox.Show(
                                            Properties.Resources.TITLE_EXISTS,
                                            this.Title,
                                            System.Windows.Forms.MessageBoxButtons.OK,
                                            System.Windows.Forms.MessageBoxIcon.Exclamation
                                        );
                        return null;
                    }
                    idx++;
                }
            }

            info.Url = tbxUrl.Text;
            if (info.Url.Length <= 0)
            {
                System.Windows.Forms.MessageBox.Show(
                                    Properties.Resources.SPECIFY_URL,
                                    this.Title,
                                    System.Windows.Forms.MessageBoxButtons.OK,
                                    System.Windows.Forms.MessageBoxIcon.Exclamation
                                );
                return null;
            }

            if (((bool)chkAuth.IsChecked) && tbxUserName.Text.Length <= 0)
            {
                System.Windows.Forms.MessageBox.Show(
                                    Properties.Resources.SPECIFY_USER_NAME,
                                    this.Title,
                                    System.Windows.Forms.MessageBoxButtons.OK,
                                    System.Windows.Forms.MessageBoxIcon.Exclamation
                                );
                return null;
            }
            if ((bool)chkAuth.IsChecked)
                info.SetAuth(tbxUserName.Text, tbxPassword.Password);

            int interval = 0;
            try
            {
                interval = Int32.Parse(tbxInterval.Text);
            }
            catch(Exception) {}

            info.PollingInterval = interval;
            if (info.PollingInterval < RssTargetInfo.POLLING_INTERVAL_MIN)
            {
                System.Windows.Forms.MessageBox.Show(
                                    Properties.Resources.ERROR_INVAID_INTERVAL,
                                    this.Title,
                                    System.Windows.Forms.MessageBoxButtons.OK,
                                    System.Windows.Forms.MessageBoxIcon.Exclamation
                                );
                return null;
            }
            return info;
        }

        /// <summary>Click event handler of the OK button.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            RssTargetInfo rssInfo = _CheckParams();

            if (rssInfo != null)
            {
                rssInfo.Idx = _infoIdx;
                ((WndSettings)Owner).UpdateTargetList(rssInfo);
                this.Close();
            }
        }

        /// <summary>Click event handler of the Cancel button.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>Click event handler of the URL Test button.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void btnUrlTest_Click(object sender, RoutedEventArgs e)
        {
            string url = tbxUrl.Text;
            string userName = tbxUserName.Text;
            string password = tbxPassword.Password;

            if (url.Length <= 0)
                return;

            WndProgress wndProgress = new WndProgress();
            wndProgress.Owner = this;
            wndProgress.lblStatus.Content = Properties.Resources.STATUS_TESTING_URL;
            wndProgress.Show();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = ThetisCore.Lib.EasyTrustPolicy.CheckValidationResult;
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(url);

                if (userName != null && userName.Length > 0)
                    webReq.Credentials = new NetworkCredential(userName, password);

                RssFeed feed = RssFeed.Read(webReq);
                if (feed != null && feed.Channels.Count > 0)
                {
                    wndProgress.Close();

                    WndConfirm wndConfirm = new WndConfirm();
                    wndConfirm.Owner = this;
                    wndConfirm.tbxTitle.Text = feed.Channels[0].Title;
                    wndConfirm.ShowDialog();
                    return;
                }
                
			}
			catch (WebException we)
			{
                Log.AddError("  " + we.Message + "\n" + we.StackTrace);
            }

            wndProgress.Close();
            System.Windows.Forms.MessageBox.Show(
                                Properties.Resources.ERROR_CANNOT_CONNECT,
                                this.Title,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Exclamation
                            );
        }

        /// <summary>Click event handler of the URL Test checkbox.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void chkAuth_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)chkAuth.IsChecked)
            {
                grdAuth.Visibility = Visibility.Visible;
                grdAuth.Height = 65;
            }
            else
            {
                grdAuth.Visibility = Visibility.Hidden;
                grdAuth.Height = 0;
            }
        }

        /// <summary>Value Changed event handler of the Polling Interval scrollbar.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void scbInterval_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (scbInterval.Value < RssTargetInfo.POLLING_INTERVAL_MIN)
                scbInterval.Value = RssTargetInfo.POLLING_INTERVAL_MIN;

            tbxInterval.Text = scbInterval.Value.ToString();
        }

        /// <summary>Text Changed event handler of Polling Interval.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void tbxInterval_TextChanged(object sender, TextChangedEventArgs e)
        {
            int interval = 0;
            try
            {
                interval = Int32.Parse(tbxInterval.Text);
                if (interval < RssTargetInfo.POLLING_INTERVAL_MIN)
                {
                    interval = RssTargetInfo.POLLING_INTERVAL_MIN;
                    tbxInterval.Text = interval.ToString();
                }
                scbInterval.Value = interval;
            }
            catch (Exception)
            {
                tbxInterval.Text = scbInterval.Value.ToString();
            }
        }

    }
}
