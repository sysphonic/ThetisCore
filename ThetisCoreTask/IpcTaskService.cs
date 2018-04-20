/* 
 * IpcTaskService.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
#define ZEPTAIR

using System;
using System.Runtime.Remoting;
using System.Security.Permissions;
using Sysphonic.Common;
using ThetisCore.Lib;

namespace ThetisCore.Task
{
    /// <summary>IPC-Remoting service of ThetisCore.Task process.</summary>
    public class IpcTaskService : MarshalByRefObject, ThetisCore.Lib.IIpcTaskService
    {
        /// <summary>Obtains a lifetime service object to control the lifetime policy for this instance.</summary>
        /// <remarks>
        /// You can create the Singleton object by overriding the InitializeLifetimeService method
        /// of MarshalByRefObject to return a null reference. This effectively keeps the object 
        /// in memory as long as the host application domain is running.
        /// http://msdn.microsoft.com/en-us/library/23bk23zc.aspx
        /// http://msdn.microsoft.com/en-us/library/system.marshalbyrefobject.initializelifetimeservice.aspx
        /// </remarks>
        /// <returns>Lease object.</returns>
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override Object InitializeLifetimeService()
        {
            /*
            ILease lease = (ILease)base.InitializeLifetimeService();
            if (lease.CurrentState == LeaseState.Initial)
            {
                lease.InitialLeaseTime = TimeSpan.FromMinutes(1);
                lease.SponsorshipTimeout = TimeSpan.FromMinutes(2);
                lease.RenewOnCallTime = TimeSpan.FromSeconds(2);
            }
            return lease;
            */
            return null;
        }

#if ZEPTAIR
        /// <summary>Clears Pending specification of the specified InfoItem.</summary>
        /// <param name="itemId">ID of the target InfoItem.</param>
        public void ClearPending(int itemId)
        {
            TaskMain.Instance.ZeptDistManager.ClearPending(itemId);
        }
#endif

        /// <summary>Removes InfoItems specified by IDs.</summary>
        /// <param name="itemIds">IDs of the target InfoItems.</param>
        public void RemoveInfoItems(int[] itemIds)
        {
            TaskMain.Instance.InfoManager.RemoveItems(itemIds, true);
#if ZEPTAIR
            TaskMain.Instance.ZeptDistManager.RemoveItems(itemIds);
#endif
        }

        /// <summary>Set or clear read flag to the specified InfoItem.</summary>
        /// <param name="itemId">ID of the target InfoItem.</param>
        /// <param name="read">true for read, false for unread.</param>
        public void SetRead(int itemId, bool read)
        {
            TaskMain.Instance.InfoManager.SetRead(itemId, read);
        }

        /// <summary>Request to update item informations.</summary>
        /// <param name="mode">Request mode.</param>
        public void DoUpdateItems(string mode)
        {
            Log.AddInfo("IpcTaskService.DoUpdateItems() : " + mode);

#if ZEPTAIR
            if (mode == @"zept_dist")
                TaskMain.Instance.RestartZeptDist(false);
            if (mode == @"zept_dist(reload_conf)")
                TaskMain.Instance.RestartZeptDist(true);
            else
#endif
                TaskMain.Instance.Restart();
        }

        /// <summary>Deletes completely specified Information Items.</summary>
        /// <param name="generatorId">Generator ID to find RssTargetInfo.</param>
        /// <param name="itemIds">Array of IDs of the Information Items.</param>
        public void RestoreTrash(string generatorId, int[] itemIds)
        {
            RssTargetInfo[] targetInfos = TaskMain.Instance.RssManager.TargetInfos;
            RssTargetInfo targetInfo = RssTargetInfo.FindGenerator(targetInfos, generatorId);
            if (targetInfo != null)
                targetInfo.TrashBox.Restore(itemIds);
        }

        /// <summary>Deletes completely specified Information Items.</summary>
        /// <param name="generatorId">Generator ID to find RssTargetInfo.</param>
        /// <param name="itemIds">Array of IDs of the Information Items.</param>
        public void BurnUpTrash(string generatorId, int[] itemIds)
        {
            RssTargetInfo[] targetInfos = TaskMain.Instance.RssManager.TargetInfos;
            RssTargetInfo targetInfo = RssTargetInfo.FindGenerator(targetInfos, generatorId);
            if (targetInfo != null)
                targetInfo.TrashBox.BurnUp(itemIds);
        }

        /// <summary>Event of updating RSS Target Informations.</summary>
        public event EventHandler<RssTargetInfosUpdatedEventArgs> RssTargetInfosUpdated;

        /// <summary>Fires event of updating RSS Target Informations.</summary>
        public void FireEventRssTargetInfosUpdated()
        {
            if (RssTargetInfosUpdated != null)
            {
                RssTargetInfosUpdated(this, new ThetisCore.Lib.RssTargetInfosUpdatedEventArgs());
            }
        }
    }
}
