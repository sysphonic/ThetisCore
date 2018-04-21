/* 
 * InfoItem.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
//#define ZEPTAIR

using System;
using System.Collections;
using System.Text.RegularExpressions;
using Rss;
using ThetisCore.Lib;

namespace ThetisCore.Task
{
    /// <summary>Information Item class.</summary>
    [Serializable()]
    public class InfoItem
    {
        private static Random _autoRand = new Random();
        protected int _id = 0;
        protected string _generatorId = null;
        protected string _title = null;
        protected string _link = null;
        protected string _guid = null;
        protected string _description = null;
        protected string _author = null;
        protected string[] _categories = null;
        protected string _comments = null;
        protected DateTime _pubDate = DateTime.MinValue;
        protected string _srcTitle = null;
        protected string _channel = null;
        protected bool _isRead = false;
        protected string[] _images = null;
        protected InfoAttach[] _attachments = null;
#if ZEPTAIR
        protected InfoCommand _command = null;
#endif


        /// <summary>Constructor</summary>
        public InfoItem()
        {
            _id = _autoRand.Next();
        }

        /// <summary>Creates an instance from a RSS Item.</summary>
        /// <param name="rssItem">RSS Item as source.</param>
        /// <param name="channel">The title of the channel.</param>
        /// <param name="targetEntry">RSS Target Entry.</param>
        public static InfoItem Create(RssItem rssItem, string channel, RssTargetEntry targetEntry)
        {
            InfoItem item = new InfoItem();
            item._generatorId = targetEntry.Id;
            item._author = rssItem.Author;
            if (rssItem.Categories != null)
            {
                string[] categories = new string[rssItem.Categories.Count];
                int idx = 0;
                foreach (RssCategory cat in rssItem.Categories)
                    categories[idx++] = cat.Name;
                item._categories = categories;
            }
            item._comments = rssItem.Comments;
            item._description = rssItem.Description;
            item._link = rssItem.Link.ToString();
            item._guid = rssItem.Guid.Name;
            item._pubDate = rssItem.PubDate;
            item._title = rssItem.Title;
            item._srcTitle = targetEntry.Title;                                 
            item._channel = channel;

            if (rssItem.Enclosures != null)
            {
                ArrayList imageList = new ArrayList();
                ArrayList attachList = new ArrayList();
                foreach (RssEnclosure enclosure in rssItem.Enclosures)
                {
                    if (enclosure.Type != null
                        && enclosure.Type.IndexOf("image/") >= 0
                        && !targetEntry.IsZeptDist)
                    {
                        imageList.Add(enclosure.Url.ToString());
                    }
                    else
                    {
                        attachList.Add(InfoAttach.Create(enclosure));
                    }
                }
                if (imageList.Count > 0)
                    item._images = (string[])imageList.ToArray(typeof(string));
                if (attachList.Count > 0)
                    item._attachments = (InfoAttach[])attachList.ToArray(typeof(InfoAttach));
            }

#if ZEPTAIR
            if (rssItem.Description != null && rssItem.Description.Length > 0)
              item._command = InfoCommand.Create(rssItem.Description);
#endif

            return item;
        }

        public void Merge(InfoItem precedingItem)
        {
            if (_attachments != null && precedingItem.Attachments != null)
            {
                foreach (InfoAttach attach in _attachments)
                {
                    foreach (InfoAttach precAttach in precedingItem.Attachments)
                    {
                        if (attach.Id == precAttach.Id
                            && attach.Timestamp == precAttach.Timestamp)
                        {
                            attach.Status = precAttach.Status;
                            break;
                        }
                    }
                }
            }

#if ZEPTAIR
            if (_command != null && precedingItem.Command != null)
            {
                InfoCommand precCmd = precedingItem.Command;
                if (_command.Id == precCmd.Id
                    && _command.Timestamp == precCmd.Timestamp)
                    _command = precCmd;
            }
#endif
        }

        /// <summary>Gets the name of the Distribution Package Directory.</summary>
        /// <returns>Name of the Distribution Package Directory.</returns>
        public string GetPackageDirName()
        {
            Match m = Regex.Match(this.Guid, @"^[a-zA-Z]+#[0-9]+");
            if (m.Success)
                return m.Groups[0].Value;
            else
                return null;
        }

        /// <summary>Checks whether this distribution has been finished or not.</summary>
        /// <returns>true if finished, false otherwise.</returns>
        public bool IsFinished()
        {
            if (_attachments != null)
            {
                foreach (InfoAttach attach in _attachments)
                {
                    if (attach.Status != InfoAttach.STATUS_COMPLETED)
                        return false;
                }
            }

#if ZEPTAIR
            if (_command != null)
            {
                if (!_command.IsFinished)
                    return false;
            }
#endif

            return true;
        }

        /// <summary>Id of the item.</summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>Generator ID.</summary>
        public string GeneratorId
        {
            get { return _generatorId; }
            set { _generatorId = value; }
        }

        /// <summary>Title of the item.</summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>URL of the item.</summary>
        public string Link
        {
            get { return _link; }
            set { _link = value; }
        }

        /// <summary>GUID of the item.</summary>
        public string Guid
        {
            get { return _guid; }
            set { _guid = value; }
        }

        /// <summary>Item synopsis.</summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>Email address of the author of the item.</summary>
        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }

        public string[] GetAuthorParams()
        {
            if (_author == null || _author.Length <= 0)
                return null;
            
            Match m = Regex.Match(_author, @"^(.+):([^:]+)$");
            if (m.Success)
                return new string[2] { m.Groups[1].Value, m.Groups[2].Value };
            else
                return null;
        }

        /// <summary>Provide information regarding the location of the subject matter of the channel in a taxonomy.</summary>
        public string[] Categories
        {
            get { return _categories; }
            set { _categories = value; }
        }

        /// <summary>Indicates when the item was published.</summary>
        public DateTime PubDate
        {
            get { return _pubDate; }
            set { _pubDate = value; }
        }

        /// <summary>URIs of attached Images.</summary>
        public string[] Images
        {
            get { return _images; }
            set { _images = value; }
        }

        /// <summary>Attachment files.</summary>
        public InfoAttach[] Attachments
        {
            get { return _attachments; }
            set { _attachments = value; }
        }

#if ZEPTAIR
        /// <summary>Command Information.</summary>
        public InfoCommand Command
        {
            get { return _command; }
            set { _command = value; }
        }
#endif

        /// <summary>Source Title.</summary>
        public string SrcTitle
        {
            get { return _srcTitle; }
            set { _srcTitle = value; }
        }

        /// <summary>The title of the channel.</summary>
        public string Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }

        /// <summary>Read flag.</summary>
        public bool IsRead
        {
            get { return _isRead; }
            set { _isRead = value; }
        }

    }
}
