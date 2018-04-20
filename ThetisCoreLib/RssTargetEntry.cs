/* 
 * RssTargetEntry.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Soap;
using Rss;
using Sysphonic.Common;

namespace ThetisCore.Lib
{
    /// <summary>RSS Target Entry class.</summary>
    public class RssTargetEntry : IComparable
    {
        /// <summary>Random Generator.</summary>
        private static Random _autoRand = new Random();

        /// <summary>Settings file name.</summary>
        protected const string SETTINGS_FILE_NAME = @"settings.xml";

        /// <summary>File name which contains the stored items.</summary>
        protected const string STORE_FILE_NAME = @"store.xml";

        /// <summary>Polling Interval default.</summary>
        public const int POLLING_INTERVAL_DEF = 15;

        /// <summary>Min. of Polling Interval.</summary>
        public const int POLLING_INTERVAL_MIN = 5;

        /// <summary>ID.</summary>
        protected string _id = null;

        /// <summary>Target Index.</summary>
        protected int _idx = -1;

        /// <summary>Target URL.</summary>
        protected string _url = null;

        /// <summary>RSS Title.</summary>
        protected string _title = null;

        /// <summary>User name for the HTTP Authentication.</summary>
        protected string _userName = null;

        /// <summary>Password for the HTTP Authentication.</summary>
        protected string _password = null;

        /// <summary>Polling Interval.</summary>
        protected int _pollingInterval = POLLING_INTERVAL_DEF;

        /// <summary>The original title of the name of the info directory.</summary>
        protected string _dirTitle = null;

        /// <summary>Zeptair Dist. flag.</summary>
        protected bool _isZeptDist = false;


        /// <summary>Constructor</summary>
        public RssTargetEntry()
        {
            _id = "RssTargetEntry_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + _autoRand.Next(999);
        }

        /// <summary>ID</summary>
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>Target URL</summary>
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        /// <summary>Information Index.</summary>
        public int Idx
        {
            get { return _idx; }
            set { _idx = value; }
        }

        /// <summary>RSS Title</summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>User name for the HTTP Authentication</summary>
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        /// <summary>Password for the HTTP Authentication</summary>
        [XmlIgnoreAttribute()]
        public string Password
        {
            get { return _password; }
//            set { _password = value; }
        }

        /// <summary>Encrypted Password for the HTTP Authentication</summary>
        public string EncPassword
        {
            get { return Enigma.Encrypt(_password); }
            set { _password = Enigma.Decrypt(value); }
        }

        /// <summary>Name of the base directory.</summary>
        [XmlIgnoreAttribute()]
        public string DirTitle
        {
            get { return _dirTitle; }
//          set { _dirTitle = value; }
        }

        /// <summary>Zeptair Dist. flag.</summary>
        public bool IsZeptDist
        {
            get { return _isZeptDist; }
            set { _isZeptDist = value; }
        }

        /// <summary>Sets parameters for the HTTP Authentication</summary>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        public void SetAuth(string userName, string password)
        {
            _userName = userName;
            _password = password;
        }

        /// <summary>Polling Interval.</summary>
        public int PollingInterval
        {
            get { return _pollingInterval; }
            set { _pollingInterval = value; }
        }

        /// <summary>Information path for this instance.</summary>
        public string NewPath
        {
            get
            {
                if (_title == null)
                    return null;
                else
                    return Path.Combine(ThetisCore.Lib.Def.INFO_ROOT_DIR, _EncForPath(_title));
            }
        }

        /// <summary>Current path before changing title.</summary>
        public string CurPath
        {
            get
            {
                if (_dirTitle == null)
                    return null;
                else
                    return Path.Combine(ThetisCore.Lib.Def.INFO_ROOT_DIR, _EncForPath(_dirTitle));
            }
        }

        /// <summary>Duplicates self entry.</summary>
        /// <returns>Clone instance of RssTargetEntry.</returns>
        private RssTargetEntry dup()
        {
            RssTargetEntry entry = new RssTargetEntry();
            entry.Id = this.Id;
            entry.Url = this.Url;
            entry.Idx = this.Idx;
            entry.Title = this.Title;
            entry.UserName = this.UserName;
            entry.EncPassword = this.EncPassword;
            entry.PollingInterval = this.PollingInterval;
            entry.IsZeptDist = this.IsZeptDist;
            return entry;
        }

        /// <summary>Saves the channel information including items.</summary>
        /// <param name="feedHistory">RSS Feed which contains history items.</param>
        public void SaveFeedHistory(RssFeed feedHistory)
        {
            string infoPath = this.CurPath;
            if (!Directory.Exists(infoPath))
            {
                Log.AddError("RssInfo.SaveFeedHistory() : Info path doesn't exist. " + infoPath);
                return;
            }

            using (FileStream file = File.Create(Path.Combine(infoPath, STORE_FILE_NAME)))
            {
                SoapFormatter formatter = new SoapFormatter();
                formatter.Serialize(file, feedHistory);
            }
            /*
            RssWriter writer = new RssWriter(infoPath + "/" + CHANNEL_FILE_NAME, Encoding.GetEncoding("UTF-8"));
            writer.Write(channel);
            writer.Close();
            */
        }

        /// <summary>Loads the stored items as a RSS Feed history.</summary>
        /// <returns>A RSS Feed instance which contains stored items.</returns>
        public RssFeed LoadFeedHistory()
        {
            string storedFile = Path.Combine(this.CurPath, STORE_FILE_NAME);
            if (!File.Exists(storedFile))
                return null;

            RssFeed feedHistory = null;
            using (FileStream file = File.Open(storedFile, FileMode.Open))
            {
                SoapFormatter formatter = new SoapFormatter();
                try
                {
                    feedHistory = (RssFeed)formatter.Deserialize(file);
                }
                catch (Exception e)
                {
                    Log.AddError("  " + e.Message + "\n" + e.StackTrace);
                }
            }
            /*
            RssFeed feed = RssFeed.Read(storedFile);
            channel = (RssChannel)feed.Channels[0];
            */
            /*
            RssReader reader = new RssReader(channelFile);
            RssChannel channel = (RssChannel) reader.Read();
            reader.Close();
            */

            return feedHistory;
        }

        /// <summary>Gets if the Title has been changed.</summary>
        /// <returns>true if the Title has been changed, false otherwise.</returns>
        protected bool _IsTitleChanged()
        {
            return (_dirTitle != null && _dirTitle != _title);
        }

        /// <summary>Updates parameters related to the Title.</summary>
        /// <returns>true if succeeded, false otherwise.</returns>
        protected bool _UpdateTitle()
        {
            string newPath = this.NewPath;
            string oldPath = this.CurPath;

            if (_IsTitleChanged())
            {
                while (Directory.Exists(newPath))
                    newPath += "_";

                if (oldPath != null && Directory.Exists(oldPath))
                {
                    DirectoryInfo di = new DirectoryInfo(oldPath);
                    try
                    {
                        di.MoveTo(newPath);
                    }
                    catch (IOException ex)
                    {
                        System.Windows.Forms.MessageBox.Show(
                                            ex.Message + "\n" + oldPath + "\n  ->\n" + newPath,
                                            @"Error",
                                            System.Windows.Forms.MessageBoxButtons.OK,
                                            System.Windows.Forms.MessageBoxIcon.Exclamation
                                        );
                        return false;
                    }
                }
            }
            else if (oldPath == null)
            {
                while (Directory.Exists(newPath))
                    newPath += "_";
            }
            _dirTitle = _DecForPath(new DirectoryInfo(newPath).Name);
            return true;
        }

        /// <summary>Saves information about this RSS target.</summary>
        /// <returns>true if succeeded, false otherwise.</returns>
        public bool Save()
        {
            if (!Directory.Exists(ThetisCore.Lib.Def.INFO_ROOT_DIR))
                Directory.CreateDirectory(ThetisCore.Lib.Def.INFO_ROOT_DIR);

            if (!_UpdateTitle())
                return false;

            string infoPath = this.CurPath;
                
            if (!Directory.Exists(infoPath))
                Directory.CreateDirectory(infoPath);

            XmlSerializer writer = new XmlSerializer(typeof(RssTargetEntry));

            using (StreamWriter file = new StreamWriter(Path.Combine(infoPath, SETTINGS_FILE_NAME)))
            {
                RssTargetEntry entry = this.dup();
                writer.Serialize(file, entry);
            }
            return true;
        }

        /// <summary>Loads informations about all RSS targets.</summary>
        /// <returns>Array of the RSS Informations.</returns>
        static public ArrayList Load()
        {
            if (!Directory.Exists(ThetisCore.Lib.Def.INFO_ROOT_DIR))
                return null;

            XmlSerializer reader = new XmlSerializer(typeof(RssTargetEntry));

            ArrayList ret = new ArrayList();
            foreach (string folder in Directory.GetDirectories(ThetisCore.Lib.Def.INFO_ROOT_DIR))
            {
                string filePath = Path.Combine(folder, SETTINGS_FILE_NAME);
                using (StreamReader file = new StreamReader(filePath))
                {
                    try
                    {
                        RssTargetEntry info = (RssTargetEntry)reader.Deserialize(file);
                        info._dirTitle = _DecForPath(new DirectoryInfo(folder).Name);
                        ret.Add(info);
                    }
                    catch (Exception ex)
                    {
                        Log.AddError("  " + ex.Message + " : " + filePath + "\n" + ex.StackTrace);
                    }
                }
            }

            ret.Sort();

            return ret;
        }

        /// <summary>Removes information directory related to this instance.</summary>
        public void RemoveDir()
        {
            string infoPath = this.CurPath;

            if (infoPath == null || !Directory.Exists(infoPath))
                return;

            Directory.Delete(infoPath, true);
        }

        /// <summary>Encodes the specified string for a folder name.</summary>
        /// <param name="name">Target String.</param>
        /// <returns>Encoded string.</returns>
        protected static string _EncForPath(string name)
        {
            name = name.Replace("%", "%25");
            name = name.Replace("\\", "%5C");
            name = name.Replace("/", "%2F");
            name = name.Replace(":", "%3A");
            name = name.Replace("*", "%2A");
            name = name.Replace("\"", "%22");
            name = name.Replace("<", "%3C");
            name = name.Replace(">", "%3E");
            name = name.Replace("|", "%7C");
            name = name.Replace("?", "%3F");
            return name;
        }

        /// <summary>Decodes the specified folder name.</summary>
        /// <param name="name">Target String.</param>
        /// <returns>Dencoded string.</returns>
        protected static string _DecForPath(string name)
        {
            name = name.Replace("%25", "%");
            name = name.Replace("%5C", "\\");
            name = name.Replace("%2F", "/");
            name = name.Replace("%3A", ":");
            name = name.Replace("%2A", "*");
            name = name.Replace("%22", "\"");
            name = name.Replace("%3C", "<");
            name = name.Replace("%3E", ">");
            name = name.Replace("%7C", "|");
            name = name.Replace("%3F", "?");
            return name;
        }

        private bool Match(string part, bool isRegexp)
        {
            if (this.Url == null)
                return false;

            if (isRegexp)
            {
                return Regex.Match(this.Url, part).Success;
            }
            else
            {
                return this.Url.Contains(part);
            }
        }

        public static RssTargetEntry[] GetRssTargetsFromUrl(string baseUrl, bool isRegexp)
        {
            RssTargetEntry targetThetisRss = null;
            RssTargetEntry targetZeptDist = null;

            ArrayList targetEntries = ThetisCore.Lib.RssTargetEntry.Load();

            if (targetEntries != null)
            {
                foreach (RssTargetEntry entry in targetEntries)
                {
                    if (entry.IsZeptDist)
                        targetZeptDist = entry;
                    else
                    {
                        if (entry.Url != null
                            && entry.Match(baseUrl, isRegexp))
                        {
                            targetThetisRss = entry;
                        }
                    }
                }

                /*
                 * If Thetis RSS not found, retry to find it
                 * by Zeptair Distribution target information.
                 */
                if (targetThetisRss == null
                    && targetZeptDist != null
                    && targetZeptDist.Url != null
                    && !targetZeptDist.Match(baseUrl, isRegexp))
                {
                    Match m = Regex.Match(targetZeptDist.Url, @"^(.+[/])feeds[/]");
                    if (m.Success)
                    {
                        baseUrl = m.Groups[1].Value;
                        foreach (RssTargetEntry entry in targetEntries)
                        {
                            if (!entry.IsZeptDist
                                && entry.Url != null
                                && entry.Url.StartsWith(baseUrl))
                            {
                                targetThetisRss = entry;
                                break;
                            }
                        }
                    }
                }
            }
            return new RssTargetEntry[]{
                targetThetisRss,
                targetZeptDist
            };
        }

        public static void SynchroTargets(string oldUrl, bool isRegexp, string newUrl, bool inheritSchemeIfExit, string userName, string password, bool synThetisRss, bool synZeptDist)
        {
            if (!newUrl.EndsWith(@"/"))
                newUrl += @"/";

            string rightPartFromHost = null;
            Match m = Regex.Match(newUrl, @"^[ ]*[^:]+://(.+)");
            if (m.Success)
            {
                rightPartFromHost = m.Groups[1].Value;
            }

            RssTargetEntry[] entries = RssTargetEntry.GetRssTargetsFromUrl(oldUrl, isRegexp);
            RssTargetEntry targetThetisRss = entries[0];
            RssTargetEntry targetZeptDist = entries[1];

            if (synThetisRss)
            {
                if (targetThetisRss == null)
                {
                    targetThetisRss = new RssTargetEntry();
                    targetThetisRss.Title = ThetisCore.Lib.Properties.Resources.THETIS_RSS;
                }
                else if (inheritSchemeIfExit)
                {
                    try
                    {
                        newUrl = String.Format(@"{0}://{1}", new Uri(targetThetisRss.Url).Scheme, rightPartFromHost);
                    }
                    catch (Exception) { }
                }
                targetThetisRss.Url = newUrl + @"feeds/index/";
                targetThetisRss.SetAuth(userName, password);

                targetThetisRss.Save();
            }
            if (synZeptDist)
            {
                if (targetZeptDist == null)
                {
                    targetZeptDist = new RssTargetEntry();
                    targetZeptDist.Title = ThetisCore.Lib.Properties.Resources.ZEPTAIR_DISTRIBUTION;
                    targetZeptDist.IsZeptDist = true;
                }
                else if (inheritSchemeIfExit)
                {
                    try
                    {
                        newUrl = String.Format(@"{0}://{1}", new Uri(targetZeptDist.Url).Scheme, rightPartFromHost);
                    }
                    catch (Exception) { }
                }
                targetZeptDist.Url = newUrl + @"feeds/zeptair_dist/";
                targetZeptDist.SetAuth(userName, password);

                targetZeptDist.Save();
            }
            if (synThetisRss || synZeptDist)
            {
                try
                {
                    ThetisCore.Lib.IIpcTaskService ipcTaskService = ThetisCore.Lib.IpcServiceAgent.GetTaskService();
                    if (ipcTaskService != null)
                        ipcTaskService.FireEventRssTargetInfosUpdated();
                }
                catch (Exception ex)
                {
                    Log.AddError(ex.Message + "\n" + ex.StackTrace);
                }
            }
        }

        #region IComparable Member

        public int CompareTo(object obj)
        {
             if(obj is RssTargetEntry) 
            {
                RssTargetEntry other = (RssTargetEntry) obj;
                return this.Idx.CompareTo(other.Idx);
            }
            else
            {
               throw new ArgumentException("Object is not a RssInfo");
            }    
        }

        #endregion
    }
}
