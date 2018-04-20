/* 
 * InfoManager.cs
 * 
 * License: Modified BSD License (See LICENSE file)
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * URL: http://sysphonic.com/
 */
using System;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;
using Sysphonic.Common;
using ThetisCore.Task;

namespace ThetisCore.Navi
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

        /// <summary>Removes the specified Information Items.</summary>
        /// <param name="itemIds">Array of IDs of the Information Items.</param>
        public void RemoveItems(int[] itemIds)
        {
            if (itemIds == null || itemIds.Length <= 0)
                return;

            try
            {
                ThetisCore.Lib.IIpcTaskService ipcTaskService = ThetisCore.Lib.IpcServiceAgent.GetTaskService();
                if (ipcTaskService != null)
                    ipcTaskService.RemoveInfoItems(itemIds);
            }
            catch (Exception ex)
            {
                Log.AddError(ex.Message + "\n" + ex.StackTrace);
            }
        }

        /// <summary>Loads Information Items.</summary>
        /// <returns>Array of information items to append.</returns>
        public ArrayList Load()
        {
            _infoItems.Clear();
            
            if (!Directory.Exists(ThetisCore.Lib.Def.INFO_ROOT_DIR))
                return null;

            XmlSerializer reader = new XmlSerializer(typeof(InfoItem[]));

            try
            {
                string fpath = ThetisCore.Lib.Def.INFO_ITEMS_FILE_PATH;
                if (File.Exists(fpath))
                {
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
            }
            catch (Exception e)
            {
                Log.AddError(e.Message + "\n" + e.StackTrace);
            }
            return _infoItems;
        }
    }
}
