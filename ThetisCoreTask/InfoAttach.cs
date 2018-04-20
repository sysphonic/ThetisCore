/* 
 * InfoAttach.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using Rss;
using ThetisCore.Lib;
using Sysphonic.Common;

namespace ThetisCore.Task
{
    /// <summary>Attachment of Information Item.</summary>
    [Serializable()]
    public class InfoAttach
    {
        protected int _id = 0;
        protected string _name = null;
        protected string _title = null;
        protected string _timestamp = null;
        protected string _digestMd5 = null;
        protected string _status = null;
        protected string _url = null;
        protected int _length = 0;
        protected string _type = null;

        /// <summary>Status expressions.</summary>
        public const string STATUS_DOWNLOADED = @"downloaded";
        public const string STATUS_FAILED = @"failed";
        public const string STATUS_ABORTED = @"aborted";
        public const string STATUS_ERROR = @"error";
        public const string STATUS_COMPLETED = @"completed";

        /// <summary>Constructor</summary>
        public InfoAttach()
        {
        }

        /// <summary>Creates an instance from a RSS Enclosure.</summary>
        /// <param name="enclosure">RSS Enclosure.</param>
        /// <returns>Attachment Information.</returns>
        public static InfoAttach Create(RssEnclosure enclosure)
        {
            InfoAttach attach = new InfoAttach();
            attach._type = enclosure.Type;
            attach._url = enclosure.Url.AbsoluteUri;
            attach._length = enclosure.Length;
            attach._id = enclosure.Id;
            attach._name = enclosure.Name;
            attach._title = enclosure.Title;
            attach._timestamp = enclosure.Timestamp;
            attach._digestMd5 = enclosure.DigestMd5;

            return attach;
        }

        public string GetFilePath(string dirName)
        {
            string attachDir = Path.Combine(ThetisCore.Lib.Def.ZEPT_DIST_ROOT_DIR, dirName);
            if (!Directory.Exists(attachDir))
            {
                Directory.CreateDirectory(attachDir);
            }

            return Path.Combine(attachDir, _name);
        }

        public bool IsValidDownloaded(string dirName)
        {
            string attachPath = GetFilePath(dirName);
            return (File.Exists(attachPath)
                && _digestMd5 == CommonUtil.GetHexStrFromByteArray(CommonUtil.GetMD5HashFromFile(attachPath))
                && _length == new FileInfo(attachPath).Length);
        }

        /// <summary>Id of the Attachment.</summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>Name of the Attachment.</summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>Title of the Attachment.</summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>Timestamp.</summary>
        public string Timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

        /// <summary>URL of the Attachment.</summary>
        public string DigestMd5
        {
            get { return _digestMd5; }
            set { _digestMd5 = value; }
        }

        /// <summary>URL of the Attachment.</summary>
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        /// <summary>MIME-Type of the Attachment.</summary>
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>Length of the Attachment.</summary>
        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        /// <summary>Status.</summary>
        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }

    }
}
