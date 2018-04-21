/* 
 * RssWatcher.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
//#define ZEPTAIR

using System;
using System.Collections;
using System.Threading;
using System.Net;
using Rss;
using Sysphonic.Common;

namespace ThetisCore.Task
{
    /// <summary>RSS Watcher class. Managed by the RssManager for each target.</summary>
    class RssWatcher
    {
        /// <summary>Reference to the RSS Manager.</summary>
        protected RssManager _manager = null;

        /// <summary>Target Information.</summary>
        protected RssTargetInfo _targetInfo = null;

        /// <summary>Last RSS Feed.</summary>
        protected RssFeed _lastFeed = null;

        /// <summary>RSS Feed which contains history items.</summary>
        protected RssFeed _feedHistory = null;

        /// <summary>Timer to keep the polling interval.</summary>
        protected Timer _watchTimer = null;

        /// <summary>Activated flag.</summary>
        protected bool _activated = false;

        /// <summary>Mutex to avoid firing timer event after disposing it.</summary>
        private static System.Threading.Mutex _mutex = new System.Threading.Mutex();


        /// <summary>Constructor.</summary>
        /// <param name="info">Target Information.</param>
        public RssWatcher(RssManager manager, RssTargetInfo targetInfo)
        {
            _manager = manager;
            _targetInfo = targetInfo;
            _feedHistory = _targetInfo.LoadFeedHistory();
        }

        /// <summary>RSS Target Information.</summary>
        public RssTargetInfo TargetInfo
        {
            get { return _targetInfo; }
            set { _targetInfo = value; }
        }

        /// <summary>Starts watching the target URL.</summary>
        public void Start()
        {
            if (_watchTimer != null)
                return;

            int updateInterval = _targetInfo.PollingInterval * 60 * 1000;

            TimerCallback timerDelegate = new TimerCallback(this.DoRead);
            _watchTimer = new Timer(timerDelegate, null, 0, updateInterval);

            _activated = true;
        }

        /// <summary>Stops watching the target URL.</summary>
        public void Stop()
        {
            _activated = false;

            if (_watchTimer == null)
                return;

            _watchTimer.Dispose();
            _watchTimer = null;

            _mutex.WaitOne();
            _mutex.ReleaseMutex();
        }

        /// <summary>Does read the target URL.</summary>
        /// <param name="state"></param>
        public void DoRead(Object state)
        {
            _mutex.WaitOne();

            if (!_activated)
            {
                _mutex.ReleaseMutex();
                return;
            }

            Log.AddInfo(@"RssWatcher.DoRead() : " + this._targetInfo.Title);

            string url = _targetInfo.Url;
#if ZEPTAIR
            if (_targetInfo.IsZeptDist)
            {
                Zeptair.Lib.Common.ConfParam zeptConf = TaskMain.Instance.ZeptConf(false);
                if (zeptConf.SpecifyAdminNames
                    && zeptConf.AdminNames != null
                    && zeptConf.AdminNames.Length > 0)
                {
                    if (url.IndexOf('?') >= 0)
                        url += @"&";
                    else
                        url += @"?";
                    url += @"admins=" + zeptConf.AdminNames;
                }
            }
#endif

            RssFeed feed = null;
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = ThetisCore.Lib.EasyTrustPolicy.CheckValidationResult;
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(url);

                if (_targetInfo.UserName != null && _targetInfo.UserName.Length > 0)
                    webReq.Credentials = new NetworkCredential(_targetInfo.UserName, _targetInfo.Password);

                if (_lastFeed == null)
                    feed = RssFeed.Read(webReq);
                else
                    feed = RssFeed.Read(webReq, _lastFeed);

                _lastFeed = feed;
            }
            catch (WebException we)
            {
                Log.AddError("  " + we.Message + "\n" + we.StackTrace);
            }

            if (feed != null && !feed.Cached)
            {
                bool modified = false;
                ArrayList postArray = new ArrayList();

                if (_feedHistory == null)
                {
                    _feedHistory = feed;
                    foreach (RssChannel channel in feed.Channels)
                        foreach (RssItem item in channel.Items)
                            postArray.Add(InfoItem.Create(item, channel.Title, _targetInfo));
                    modified = true;
                }
                else
                {
                    foreach (RssChannel channel in feed.Channels)
                    {
                        int historyIdx = RssHelper.IndexOf(_feedHistory.Channels, channel);

                        if (historyIdx < 0)
                        {
                            _feedHistory.Channels.Add(channel);
                            foreach (RssItem item in channel.Items)
                                postArray.Add(InfoItem.Create(item, channel.Title, _targetInfo));

                            modified = true;
                        }
                        else
                        {
                            RssItemCollection historyItems = _feedHistory.Channels[historyIdx].Items;

                            foreach (RssItem item in channel.Items)
                            {
                                if (!RssHelper.Contains(historyItems, item))
                                {
                                    historyItems.Add(item);
                                    postArray.Add(InfoItem.Create(item, channel.Title, _targetInfo));
                                    modified = true;
                                }
                            }
                        }
                    }
                }

                if (modified)
                {
                    _feedHistory.LastModified = feed.LastModified;
                    _targetInfo.SaveFeedHistory(_feedHistory);

                    _manager.PostItems(postArray, _targetInfo);
                    postArray.Clear();
                }
                postArray = null;
            }

            _mutex.ReleaseMutex();
        }
    }
}
