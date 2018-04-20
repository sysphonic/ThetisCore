/* 
 * CommonUtil.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Security.Cryptography;     // for MD5CryptoServiceProvider
using System.Runtime.InteropServices;

namespace Sysphonic.Common
{
    /// <summary>Common Utility Library class.</summary>
    public class CommonUtil
    {
        public static string Join(string[] ary, string sep)
        {
            StringBuilder ret = new StringBuilder();
            int idx = 0;
            foreach (string str in ary)
            {
                ret.Append(str);

                if (++idx < ary.Length)
                    ret.Append(sep);
            }
            return ret.ToString();
        }

        public static string GetHexStrFromByteArray(byte[] data)
        {
            if (data == null)
                return null;

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public static byte[] GetMD5HashFromFile(string fpath)
        {
            try
            {
                FileStream file = new FileStream(fpath, FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] ret = md5.ComputeHash(file);
                file.Close();
                return ret;
            }
            catch
            {
                return null;
            }
        }

        public static bool StartProcessWithCallback(
                            string fileName,
                            string args,
                            System.ComponentModel.ISynchronizeInvoke synchroInvoke,
                            EventHandler eventHandler,
                            string workingDir=null,
                            StringBuilder stdOutput=null
            )
        {
            Process proc = new Process();
            proc.StartInfo.FileName = fileName;
            proc.StartInfo.Arguments = args;

            proc.StartInfo.RedirectStandardInput = false;
            proc.StartInfo.RedirectStandardOutput = (stdOutput != null);
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;

            if (workingDir != null)
                proc.StartInfo.WorkingDirectory = workingDir;
            proc.SynchronizingObject = synchroInvoke;
            proc.EnableRaisingEvents = true;
            proc.Exited += eventHandler;
            try
            {
                proc.Start();

                if (stdOutput != null)
                {
                    string results = proc.StandardOutput.ReadToEnd();
                    proc.WaitForExit();

                    if (results != null
                        && results.Trim().Length > 0)
                    {
                        stdOutput.Append(results.Trim());
                    }
                }
                return true;
            }
            catch (Exception)
            {
              return false;
            }
        }

        /// <summary>Gets path of the Application directory.</summary>
        /// <returns>Path of the Application directory.</returns>
        public static string GetAppPath()
        {
            return System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        [DllImport("user32.dll")]
        private static extern Boolean SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);
        private const int SW_RESTORE = 9;

        /// <summary>Gets previous process.</summary>
        /// <remarks>http://www.atmarkit.co.jp/fdotnet/dotnettips/151winshow/winshow.html</remarks>
        /// <returns>Previous process.</returns>
        public static Process GetPreviousProcess()
        {
            Process curProcess = Process.GetCurrentProcess();
            Process[] allProcesses = Process.GetProcessesByName(curProcess.ProcessName);

            foreach (Process checkProcess in allProcesses)
            {
                if (checkProcess.Id != curProcess.Id)
                {
                    if (String.Compare(
                        checkProcess.MainModule.FileName,
                        curProcess.MainModule.FileName, true) == 0)
                    {
                        return checkProcess;
                    }
                }
            }
            return null;
        }

        /// <summary>Sets the specified external window to foreground.</summary>
        /// <remarks>http://www.atmarkit.co.jp/fdotnet/dotnettips/151winshow/winshow.html</remarks>
        /// <param name="hWnd">Handle of the target window.</param>
        public static void WakeupWindow(IntPtr hWnd)
        {
            if (IsIconic(hWnd))
            {
                ShowWindowAsync(hWnd, SW_RESTORE);
            }

            SetForegroundWindow(hWnd);
        }

        /// <summary>Gets 3 section formatted version string from 4 sections.</summary>
        /// <param name="productVer">4 or more section formatted version string.</param>
        /// <returns>3 section formatted version string like "x.x.x".</returns>
        public static string TrimVersion(string productVer)
        {
            string[] vers = productVer.Split('.');

            if (vers.Length <= 3)
                return productVer;

            return string.Format(@"{0}.{1}.{2}", vers[0], vers[1], vers[2]);
        }

        /// <summary>Escapes invalid characters in a file name.</summary>
        /// <param name="s">Target file name.</param>
        /// <returns>Escaped file name.</returns>
        public static string EscapeFileName(string s)
        {
            string valid = s;
            char[] invalidch = System.IO.Path.GetInvalidFileNameChars();

            foreach (char c in invalidch)
            {
                valid = valid.Replace(c, '_');
            }
            return valid;
        }

        public static void OpenStreamSafe(string fpath, string rxw, Action<MarshalByRefObject> action)
        {
            var autoResetEvent = new System.Threading.AutoResetEvent(false);
 
            while (true)
            {
                try
                {
                    if (rxw == "r")
                    {
                        using (StreamReader file = new StreamReader(fpath))
                        {
                            action(file);
                            break;
                        }
                    }
                    else
                    {
                        using (StreamWriter file = new StreamWriter(fpath))
                        {
                            action(file);
                            break;
                        }
                    }
                }
                catch (IOException)
                {
                    var fileSystemWatcher =
                        new FileSystemWatcher(Path.GetDirectoryName(fpath))
                                {
                                    EnableRaisingEvents = true
                                };
 
                    fileSystemWatcher.Changed +=
                        (o, e) =>
                            {
                                if(Path.GetFullPath(e.FullPath) == Path.GetFullPath(fpath))
                                {
                                    autoResetEvent.Set();
                                }
                            };
 
                    autoResetEvent.WaitOne();
                }
            }
        }
    }
}
