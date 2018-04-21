/* 
 * TrashBox.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
//#define ZEPTAIR

using System;
using System.IO;
using System.Collections;
using System.Xml.Serialization;
using Sysphonic.Common;

namespace ThetisCore.Task
{
    /// <summary>Trash Box class.</summary>
    public class TrashBox
    {
        protected const int FULL_COUNT = 30;

        /// <summary>Trash File name which contains the disposed items.</summary>
        protected const string TRASH_FILE_NAME = @"trash.xml";

        /// <summary>Path of the Trash Box.</summary>
        protected string _path = null;

        /// <summary>Thrown away Information Items.</summary>
        protected ArrayList _infoItems = new ArrayList();


        /// <summary>Constructor.</summary>
        public TrashBox()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="infoPath">Information Path of the target.</param>
        public TrashBox(string infoPath)
        {
            _path = Path.Combine(infoPath, TRASH_FILE_NAME);
            this.Load();
        }

        /// <summary>Thrown away Information Items.</summary>
        public ArrayList InfoItems
        {
            get { return _infoItems; }
        }

        /// <summary>Loads content of this Trash Box.</summary>
        public void Load()
        {
            if (!File.Exists(_path))
                return;

            XmlSerializer reader = new XmlSerializer(typeof(InfoItem[]));
            using (StreamReader file = new StreamReader(_path))
            {
                try
                {
                    _infoItems = new ArrayList();

                    InfoItem[] items = (InfoItem[]) reader.Deserialize(file);
                    foreach (InfoItem item in items)
                        _infoItems.Add(item);
                        
                }
                catch (Exception ex)
                {
                    Log.AddError("  " + ex.Message + " : " + _path + "\n" + ex.StackTrace);
                }
            }

        }

        /// <summary>Loads all Trash Boxes.</summary>
        /// <returns>Array of the Trash Boxes.</returns>
        static public ArrayList LoadAll()
        {
            if (!Directory.Exists(ThetisCore.Lib.Def.INFO_ROOT_DIR))
                return null;

            ArrayList ret = new ArrayList();
            foreach (string folder in Directory.GetDirectories(ThetisCore.Lib.Def.INFO_ROOT_DIR))
            {
                TrashBox trashBox = new TrashBox(folder);
                ret.Add(trashBox);
            }
            return ret;
        }

        /// <summary>Throws the specified Information Items into Trash.</summary>
        /// <param name="items">Array of the Information Items.</param>
        public void Push(IEnumerable items)
        {
            if (items == null)
                return;

            this.Load();

            foreach (InfoItem item in items)
            {
                _infoItems.Add(item);
            }

            if (_infoItems.Count > FULL_COUNT)
                _infoItems.RemoveRange(0, _infoItems.Count - FULL_COUNT);

            XmlSerializer writer = new XmlSerializer(typeof(InfoItem[]));

            using (StreamWriter file = new StreamWriter(_path))
            {
                InfoItem[] array = new InfoItem[_infoItems.Count];
                _infoItems.CopyTo(array);
                writer.Serialize(file, array);
            }
        }

        /// <summary>Deletes completely specified Information Items.</summary>
        /// <param name="itemIds">Array of IDs of the Information Items.</param>
        public void BurnUp(IEnumerable itemIds)
        {
            if (itemIds == null)
                return;

            this.Load();

            foreach (int itemId in itemIds)
            {
                foreach (InfoItem refItem in _infoItems)
                {
                    if (itemId == refItem.Id)
                    {
                        _infoItems.Remove(refItem);
                        break;
                    }
                }
            }

            XmlSerializer writer = new XmlSerializer(typeof(InfoItem[]));

            using (StreamWriter file = new StreamWriter(_path))
            {
                InfoItem[] array = new InfoItem[_infoItems.Count];
                _infoItems.CopyTo(array);
                writer.Serialize(file, array);
            }
        }

        /// <summary>Restore specified Information Items.</summary>
        /// <param name="itemIds">Array of IDs of the Information Items.</param>
        public void Restore(IEnumerable itemIds)
        {
            if (itemIds == null)
                return;

            this.Load();

            ArrayList items = new ArrayList();

            foreach (int itemId in itemIds)
            {
                foreach (InfoItem refItem in _infoItems)
                {
                    if (itemId == refItem.Id)
                    {
                        items.Add(refItem);
                        _infoItems.Remove(refItem);
                        break;
                    }
                }
            }

            if (items.Count <= 0)
                return;

            XmlSerializer writer = new XmlSerializer(typeof(InfoItem[]));

            using (StreamWriter file = new StreamWriter(_path))
            {
                InfoItem[] array = new InfoItem[_infoItems.Count];
                _infoItems.CopyTo(array);
                writer.Serialize(file, array);
            }

            RssTargetInfo[] targetInfos = TaskMain.Instance.RssManager.TargetInfos;
            RssTargetInfo targetInfo = RssTargetInfo.FindGenerator(targetInfos, ((InfoItem)items[0]).GeneratorId);
            
#if ZEPTAIR
            if (targetInfo.IsZeptDist)
                TaskMain.Instance.ZeptDistManager.PostItems(items, true);
            else
#endif
                TaskMain.Instance.InfoManager.PostItems(items, true);
        }
    }
}
