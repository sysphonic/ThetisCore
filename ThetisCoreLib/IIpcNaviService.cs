/* 
 * IIpcNaviService.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System;

namespace ThetisCore.Lib
{
    /// <summary>Interface of the IPC-Remoting service of ThetisCore.Navi process.</summary>
    public interface IIpcNaviService
    {
        /// <summary>Event of updating list of the information items.</summary>
        event EventHandler<InfoItemsUpdatedEventArgs> InfoItemsUpdated;

        /// <summary>Fires event of updating list of the information items.</summary>
        void FireEventInfoItemsUpdated();

        /// <summary>Event of updating list of the distribution items.</summary>
        event EventHandler<DistItemsUpdatedEventArgs> DistItemsUpdated;

        /// <summary>Fires event of updating list of the distribution items.</summary>
        void FireEventDistItemsUpdated();

        /// <summary>Event of Task process closing.</summary>
        event EventHandler<TaskProcClosingEventArgs> TaskProcClosing;

        /// <summary>Fires event of updating list of the information items.</summary>
        void FireEventTaskProcClosing();
    }
}
