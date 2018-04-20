/* 
 * IpcServiceAgent.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System;
using System.Diagnostics;
using Sysphonic.Common;

namespace ThetisCore.Lib
{
    /// <summary>IPC Service agent.</summary>
    public class IpcServiceAgent
    {
        /// <summary>Gets IpcTaskService interface.</summary>
        public static IIpcTaskService GetTaskService()
        {
            Process[] prcs = Process.GetProcessesByName("ThetisCoreTask");

            if (prcs != null && prcs.Length > 0)
            {
                try
                {
                    return (IIpcTaskService)Activator.GetObject(typeof(ThetisCore.Lib.IIpcTaskService), "ipc://thetisCoreTask.sysphonic.com/ipcTaskService");
                }
                catch (System.Runtime.Remoting.RemotingException ex)
                {
                    Log.AddError(ex.Message + "\n" + ex.StackTrace);
                }
            }
            else
            {
/*
                System.Windows.Forms.MessageBox.Show(
                        "Please retry after starting Main task (Task tray icon).",
                        "ThetisCore",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation
                    );
*/
            }
            return null;
        }

        /// <summary>Gets IpcNaviService interface.</summary>
        public static IIpcNaviService GetNaviService()
        {
            Process[] prcs = Process.GetProcessesByName("ThetisCoreNavi");

            if (prcs != null && prcs.Length > 0)
            {
                try
                {
                    return (IIpcNaviService)Activator.GetObject(typeof(ThetisCore.Lib.IIpcNaviService), "ipc://thetisCoreNavi.sysphonic.com/ipcNaviService");
                }
                catch (System.Runtime.Remoting.RemotingException ex)
                {
                    Log.AddError(ex.Message + "\n" + ex.StackTrace);
                }
            }
            return null;
        }

        /// <summary>Gets IpcConfService interface.</summary>
        public static IIpcConfService GetConfService()
        {
            Process[] prcs = Process.GetProcessesByName("ThetisCoreConf");

            if (prcs != null && prcs.Length > 0)
            {
                try
                {
                    return (IIpcConfService)Activator.GetObject(typeof(ThetisCore.Lib.IIpcConfService), "ipc://thetisCoreConf.sysphonic.com/ipcConfService");
                }
                catch (System.Runtime.Remoting.RemotingException ex)
                {
                    Log.AddError(ex.Message + "\n" + ex.StackTrace);
                }
            }
            return null;
        }
    }
}
