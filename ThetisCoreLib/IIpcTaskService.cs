/* 
 * IIpcTaskService.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
//#define ZEPTAIR

using System;

namespace ThetisCore.Lib
{
    /// <summary>Interface of the IPC-Remoting service of ThetisCore.Task process.</summary>
    public interface IIpcTaskService
    {
#if ZEPTAIR
        /// <summary>Clears Pending specification of the specified InfoItem.</summary>
        /// <param name="itemId">ID of the target InfoItem.</param>
        void ClearPending(int itemId);
#endif

        /// <summary>Removes InfoItems specified by IDs.</summary>
        /// <param name="itemIds">IDs of the target InfoItems.</param>
        void RemoveInfoItems(int[] itemIds);

        /// <summary>Set or clear read flag to the specified InfoItem.</summary>
        /// <param name="itemId">ID of the target InfoItem.</param>
        /// <param name="read">true for read, false for unread.</param>
        void SetRead(int itemId, bool read);

        /// <summary>Request to update item informations.</summary>
        /// <param name="mode">Request mode.</param>
        void DoUpdateItems(string mode);

        /// <summary>Deletes completely specified Information Items.</summary>
        /// <param name="generatorId">Generator ID to find RssTargetInfo.</param>
        /// <param name="itemIds">Array of IDs of the Information Items.</param>
        void RestoreTrash(string generatorId, int[] itemIds);

        /// <summary>Deletes completely specified Information Items.</summary>
        /// <param name="generatorId">Generator ID to find RssTargetInfo.</param>
        /// <param name="itemIds">Array of IDs of the Information Items.</param>
        void BurnUpTrash(string generatorId, int[] itemIds);

        /// <summary>Event of updating RSS Target Informations.</summary>
        event EventHandler<RssTargetInfosUpdatedEventArgs> RssTargetInfosUpdated;

        /// <summary>Fires event of updating RSS Target Informations.</summary>
        void FireEventRssTargetInfosUpdated();
    }
}
