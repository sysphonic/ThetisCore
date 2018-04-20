/* 
 * IpcConfService.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System;
using System.Runtime.Remoting;
using System.Security.Permissions;

namespace ThetisCore.Conf
{
    /// <summary>IPC-Remoting service of ThetisCore.Conf process.</summary>
    public class IpcConfService : MarshalByRefObject, ThetisCore.Lib.IIpcConfService
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

        /// <summary>Event of Task process closing.</summary>
        public event EventHandler<ThetisCore.Lib.TaskProcClosingEventArgs> TaskProcClosing;

        /// <summary>Fires event of updating list of the information items.</summary>
        public void FireEventTaskProcClosing()
        {
            if (TaskProcClosing != null)
            {
                TaskProcClosing(this, new ThetisCore.Lib.TaskProcClosingEventArgs());
            }
        }
    }
}
