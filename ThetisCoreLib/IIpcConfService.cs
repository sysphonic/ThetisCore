/* 
 * IIpcConfService.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System;

namespace ThetisCore.Lib
{
    /// <summary>Interface of the IPC-Remoting service of ThetisCore.Conf process.</summary>
    public interface IIpcConfService
    {
        /// <summary>Event of Task process closing.</summary>
        event EventHandler<TaskProcClosingEventArgs> TaskProcClosing;

        /// <summary>Fires event of updating list of the information items.</summary>
        void FireEventTaskProcClosing();
    }
}
