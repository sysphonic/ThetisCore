/* 
 * RssManager.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
#define ZEPTAIR

using System.Collections;
using Rss;

namespace ThetisCore.Task
{
    /// <summary>RSS Manager class. Manages RssWatchers.</summary>
    public class RssManager
    {
        /// <summary>Enumeration of the status about this class.</summary>
        protected enum Status { Activated, Deactivated }

        /// <summary>Status of this instance.</summary>
        protected Status _status = Status.Deactivated;

        /// <summary>Array of the RSS Watchers.</summary>
        protected ArrayList _watchers = null;

        /// <summary>Mutex to operate asynchronously.</summary>
        private static System.Threading.Mutex _mutex = new System.Threading.Mutex();

        /// <summary>Constructor</summary>
        public RssManager()
        {
            _watchers = new ArrayList();
        }

        /// <summary>Target Informations.</summary>
        public RssTargetInfo[] TargetInfos
        {
            get
            {
                _mutex.WaitOne();

                RssTargetInfo[] targetInfos = new RssTargetInfo[_watchers.Count];

                int idx = 0;
                foreach (RssWatcher watcher in _watchers)
                {
                    targetInfos[idx++] = watcher.TargetInfo;
                }

                _mutex.ReleaseMutex();

                return targetInfos;
            }
        }

        /// <summary>Creates a RSS Watcher for the specified RSS target.</summary>
        /// <param name="info">Target Information.</param>
        /// <returns>true if succeeded, false otherwise.</returns>
        public bool AddWatcher(RssTargetInfo info)
        {
            foreach (RssWatcher watcher in _watchers)
            {
                if (watcher.TargetInfo.Url.Equals(info.Url))
                    return false;
            }

            _mutex.WaitOne();

            RssWatcher newWatcher = new RssWatcher(this, info);
            _watchers.Add(newWatcher);

            if (_status.Equals(Status.Activated))
            {
                newWatcher.Start();
            }

            _mutex.ReleaseMutex();

            info.Save();
            return true;
        }

        /// <summary>Loads RssWatchers.</summary>
        /// <returns>true if succeeded, false otherwise.</returns>
        public bool LoadWatchers()
        {
            _watchers.Clear();

            ArrayList targetInfos = RssTargetInfo.Load();
            if (targetInfos == null || targetInfos.Count <= 0)
                return false;

            _mutex.WaitOne();

            foreach (RssTargetInfo targetInfo in targetInfos)
            {
                RssWatcher watcher = new RssWatcher(this, targetInfo);
                _watchers.Add(watcher);
            }

            _mutex.ReleaseMutex();

            return true;
        }

        /// <summary>Makes all RssWatchers start watching each URL.</summary>
        public void Start()
        {
            if (_status.Equals(Status.Activated))
                return;

#if ZEPTAIR
            Zeptair.Lib.Common.ConfParam zeptConf = TaskMain.Instance.ZeptConf(false);
            bool hasValidZeptLicKey = TaskMain.Instance.HasValidZeptLicKey;
#endif

            _mutex.WaitOne();

            foreach (RssWatcher watcher in _watchers)
            {
#if ZEPTAIR
                if (watcher.TargetInfo.IsZeptDist
                    && (!hasValidZeptLicKey || !zeptConf.AcceptCmd))
                    continue;
#endif

                watcher.Start();
            }

            _mutex.ReleaseMutex();

            _status = Status.Activated;
        }

        /// <summary>Makes all RssWatchers stop watching each URL.</summary>
        public void Stop()
        {
            if (_status.Equals(Status.Deactivated))
                return;

            _mutex.WaitOne();

            foreach (RssWatcher watcher in _watchers)
            {
                watcher.Stop();
            }

            _mutex.ReleaseMutex();

            _status = Status.Deactivated;
        }

#if ZEPTAIR
        /// <summary>Restarts Zeptair Dist. watchers.</summary>
        public void RestartZeptDist()
        {
            Zeptair.Lib.Common.ConfParam zeptConf = TaskMain.Instance.ZeptConf(false);

            _mutex.WaitOne();

            foreach (RssWatcher watcher in _watchers)
            {
                if (!watcher.TargetInfo.IsZeptDist)
                    continue;

                watcher.Stop();

                if (zeptConf.AcceptCmd)
                    watcher.Start();
            }

            _mutex.ReleaseMutex();
        }
#endif

        /// <summary>Posts new items to the Information Manager.</summary>
        /// <param name="items">Array of the Information Items to post.</param>
        /// <param name="targetInfo">Target Information.</param>
        public void PostItems(ArrayList items, RssTargetInfo targetInfo)
        {
            if (items == null || items.Count <= 0)
                return;

#if ZEPTAIR
            if (targetInfo.IsZeptDist)
                TaskMain.Instance.ZeptDistManager.PostItems(items, true);
            else
#endif
                TaskMain.Instance.InfoManager.PostItems(items, true);
        }

    }
}
