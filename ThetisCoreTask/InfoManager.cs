/* 
 * InfoManager.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;
using Sysphonic.Common;

namespace ThetisCore.Task
{
    /// <summary>Information Manager class.</summary>
    public class InfoManager
    {
        /// <summary>Array of the Information Items</summary>
        protected ArrayList _infoItems = null;


        /// <summary>Constructor</summary>
        public InfoManager()
        {
            _infoItems = new ArrayList();
        }

        /// <summary>Posts new items to the Main Window.</summary>
        /// <param name="items">Array of the RSS Items to post.</param>
        /// <param name="wakeup">Flag to wake the Navi-Panel up.</param>
        public void PostItems(ArrayList items, bool wakeup)
        {
            if (items == null || items.Count <= 0)
                return;

            _Load();

            ThetisCore.Lib.ConfigManager configManager = ThetisCore.Lib.ConfigManager.Load();
            int curNum = _infoItems.Count;
            int exceedNum = (curNum + items.Count) - configManager.MaxItemsOnPanel;
            if (exceedNum > 0)
            {
                if (exceedNum <= curNum)
                {
                    int[] itemIds = new int[exceedNum];
                    for (int i = 0; i < exceedNum; i++)
                    {
                        itemIds[i] = ((InfoItem)_infoItems[curNum - exceedNum + i]).Id;
                    }
                    RemoveItems(itemIds, false);
                }
                else
                {
                    int[] itemIds = new int[curNum];
                    for (int i = 0; i < curNum; i++)
                    {
                        itemIds[i] = ((InfoItem)_infoItems[i]).Id;
                    }
                    RemoveItems(itemIds, false);

                    if (items.Count > configManager.MaxItemsOnPanel)
                        items.RemoveRange(0, items.Count - configManager.MaxItemsOnPanel);
                }
            }

            _infoItems.InsertRange(0, items);
            _Save();

            _infoItems.Clear();

            if (wakeup)
            {
                try
                {
                    ThetisCore.Lib.IIpcNaviService ipcNaviService = ThetisCore.Lib.IpcServiceAgent.GetNaviService();
                    if (ipcNaviService != null)
                        ipcNaviService.FireEventInfoItemsUpdated();
                    else
                        Process.Start("ThetisCoreNavi.exe");
                }
                catch (Exception ex)
                {
                    Log.AddError(ex.Message + "\n" + ex.StackTrace);
                }
            }
        }

        /// <summary>Removes the specified Information Items.</summary>
        /// <param name="itemIds">Array of IDs of the Information Items.</param>
        /// <param name="bLoadSave">Flag to Load and Save the file.</param>
        public void RemoveItems(int[] itemIds, bool bLoadSave)
        {
            if (bLoadSave)
                _Load();

            if (itemIds == null || itemIds.Length <= 0)
                return;

            RssTargetInfo[] targetInfos = TaskMain.Instance.RssManager.TargetInfos;

            foreach (int id in itemIds)
            {
                InfoItem item = null;

                foreach (InfoItem elem in _infoItems)
                {
                    if (elem.Id == id)
                    {
                        item = elem;
                        break;
                    }
                }
                if (item == null)
                    continue;

                _infoItems.Remove(item);

                RssTargetInfo targetInfo = RssTargetInfo.FindGenerator(targetInfos, item.GeneratorId);
                if (targetInfo != null)
                    targetInfo.TrashBox.Push(new InfoItem[1] { item });
            }
            if (bLoadSave)
                _Save();
        }

        /// <summary>Set or clear read flag to the specified InfoItem.</summary>
        /// <param name="itemId">ID of the target InfoItem.</param>
        /// <param name="read">true for read, false for unread.</param>
        public void SetRead(int itemId, bool read)
        {
            _Load();

            Log.AddInfo("InfoManager.SetRead() : " + itemId.ToString() + ", " + read.ToString());

            foreach (InfoItem item in _infoItems)
            {
                if (item.Id == itemId)
                {
                    item.IsRead = read;
                    break;
                }
            }

            _Save();

            _infoItems.Clear();
        }

        /// <summary>Saves all Information Items.</summary>
        protected void _Save()
        {
            if (_infoItems == null)
                return;

            if (!Directory.Exists(ThetisCore.Lib.Def.INFO_ROOT_DIR))
            {
                Directory.CreateDirectory(ThetisCore.Lib.Def.INFO_ROOT_DIR);
            }

            InfoItem[] array = new InfoItem[_infoItems.Count];
            _infoItems.CopyTo(array);

            try
            {
                XmlSerializer writer = new XmlSerializer(typeof(InfoItem[]));
                string fpath = ThetisCore.Lib.Def.INFO_ITEMS_FILE_PATH;
                CommonUtil.OpenStreamSafe(fpath, "w",
                       (f) =>
                       {
                           StreamWriter file = (StreamWriter)f;
                           try
                           {
                               writer.Serialize(file, array);
                           }
                           catch (IOException ioe)
                           {
                               Log.AddError(ioe.Message + "\n" + ioe.StackTrace);
                           }
                       }
                   );
            }
            catch (Exception e)
            {
                Log.AddError(e.Message + "\n" + e.StackTrace);
            }
        }

        /// <summary>Loads Information Items.</summary>
        protected void _Load()
        {
            _infoItems.Clear();

            if (!Directory.Exists(ThetisCore.Lib.Def.INFO_ROOT_DIR))
                return;

            XmlSerializer reader = new XmlSerializer(typeof(InfoItem[]));

            try
            {
                string fpath = ThetisCore.Lib.Def.INFO_ITEMS_FILE_PATH;
                if (!File.Exists(fpath)) {
					CommonUtil.CreateEmptyFile(fpath);
				}
                CommonUtil.OpenStreamSafe(fpath, "r",
                        (f) =>
                        {
                            StreamReader file = (StreamReader)f;
                            try
                            {
                                InfoItem[] array = (InfoItem[])reader.Deserialize(file);
                                if (array != null)
                                {
                                    foreach (InfoItem item in array)
                                        _infoItems.Add(item);
                                }
                            }
                            catch (IOException ioe)
                            {
                                Log.AddError(ioe.Message + "\n" + ioe.StackTrace);
                            }
                        }
                    );
            }
            catch (Exception e)
            {
                Log.AddError(e.Message + "\n" + e.StackTrace);
            }
        }
    }
}
