/* 
 * RssTargetInfo.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using Rss;
using ThetisCore.Lib;
using Sysphonic.Common;

namespace ThetisCore.Task
{
    /// <summary>RSS Target Information class.</summary>
    public class RssTargetInfo : RssTargetEntry
    {
        /// <summary>Constructor.</summary>
        public RssTargetInfo()
        {
        }

        /// <summary>Constructor.</summary>
        public RssTargetInfo(RssTargetEntry entry)
        {
            this._id = entry.Id;
            this._idx = entry.Idx;
            this._url = entry.Url;
            this._title = entry.Title;
            this._userName = entry.UserName;
            this._password = entry.Password;
            this._pollingInterval = entry.PollingInterval;
            this._dirTitle = entry.DirTitle;
            this._isZeptDist = entry.IsZeptDist;
        }

        /// <summary>Loads informations about all RSS targets.</summary>
        /// <returns>Array of the RSS Informations.</returns>
        static new public ArrayList Load()
        {
            ArrayList entries = RssTargetEntry.Load();

            if (entries == null)
                return null;

            ArrayList ret = new ArrayList();
            foreach (RssTargetEntry entry in entries)
            {
                ret.Add(new RssTargetInfo(entry));
            }
            return ret;
        }

        /// <summary>Throws the specified Information Items into Trash.</summary>
        public TrashBox TrashBox
        {
            get
            {
                if (!Directory.Exists(ThetisCore.Lib.Def.INFO_ROOT_DIR))
                    Directory.CreateDirectory(ThetisCore.Lib.Def.INFO_ROOT_DIR);

                string infoPath = this.CurPath;

                if (infoPath == null)
                    return null;

                if (!Directory.Exists(infoPath))
                    Directory.CreateDirectory(infoPath);

                return new TrashBox(infoPath);
            }
        }
        /// <summary>Finds generator of the specified Information Item from an Array.</summary>
        /// <param name="targetInfos">Array of RssTargetInfos.</param>
        /// <param name="generatorId">GeneratorId of the Information Item.</param>
        /// <returns>Generator RssTargetInfo.</returns>
        public static RssTargetInfo FindGenerator(IEnumerable targetInfos, string generatorId)
        {
            foreach (RssTargetInfo targetInfo in targetInfos)
            {
                if (targetInfo.Id == generatorId)
                {
                    return targetInfo;
                }
            }
            return null;
        }
    }
}
