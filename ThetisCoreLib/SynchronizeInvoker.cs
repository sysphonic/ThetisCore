/* 
 * SynchronizeInvoker.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System;
using System.ComponentModel;

namespace Sysphonic.Common
{
    public class SynchronizeInvoker : ISynchronizeInvoke
    {
        private class AsyncResult : IAsyncResult
        {
            private System.Threading.ManualResetEvent m_handle = new System.Threading.ManualResetEvent(false);
            private int m_completed = 0;
            public object m_return = null;
            public Exception m_exception = null;

            public AsyncResult(Delegate method, object[] args)
            {
                RunDelegateAsync(method, args);
            }

            public object AsyncState
            {
                get { return this; }
            }

            public System.Threading.WaitHandle AsyncWaitHandle
            {
                get { return m_handle; }
            }

            public bool CompletedSynchronously
            {
                get { return false; }
            }

            public bool IsCompleted
            {
                get { return m_completed == 1; }
            }

            private void RunDelegateAsync(Delegate method, object[] args)
            {
                System.Threading.WaitCallback del = delegate(object unused)
                {
                    try
                    {
                        object temp = method.DynamicInvoke(args);
                        System.Threading.Interlocked.Exchange(ref m_return, temp);
                    }
                    catch (Exception ex)
                    {
                        System.Threading.Interlocked.Exchange(ref m_exception, ex);
                    }

                    System.Threading.Interlocked.Exchange(ref m_completed, 1);
                    m_handle.Set();
                };

                System.Threading.ThreadPool.QueueUserWorkItem(del);
            }
        }

        object ISynchronizeInvoke.Invoke(Delegate method, object[] args)
        {
            return method.DynamicInvoke(args);
        }

        bool ISynchronizeInvoke.InvokeRequired
        {
            get { return true; }
        }

        IAsyncResult ISynchronizeInvoke.BeginInvoke(Delegate method, object[] args)
        {
            return new AsyncResult(method, args);
        }

        object ISynchronizeInvoke.EndInvoke(IAsyncResult result)
        {
            var r = (AsyncResult)result;
            try
            {
                r.AsyncWaitHandle.WaitOne();
            }
            finally
            {
                r.AsyncWaitHandle.Close();
            }

            if (r.m_exception != null)
            {
                throw new Exception("Error during BeginInvoke", r.m_exception);
            }
            return r.m_return;
        }
    }
}
