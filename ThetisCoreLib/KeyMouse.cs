/* 
 * KeyMouse.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System;
using System.Runtime.InteropServices;

namespace Sysphonic.Common
{
    public class KeyMouse
    {
        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public int type;  // 0 = INPUT_MOUSE, 1 = INPUT_KEYBOARD
            public MOUSEINPUT mi;
        }

        [DllImport("user32.dll")]
        static extern uint SendInput(
            uint nInputs,
            INPUT[] pInputs,
            int cbSize
            );

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;   // amount of wheel movement
            public int dwFlags;
            public int time;        // time stamp for the event
            public IntPtr dwExtraInfo;
        }

        // dwFlags
        const int MOUSEEVENTF_MOVED      = 0x0001 ;
        const int MOUSEEVENTF_LEFTDOWN   = 0x0002 ;
        const int MOUSEEVENTF_LEFTUP     = 0x0004 ;
        const int MOUSEEVENTF_RIGHTDOWN  = 0x0008 ;
        const int MOUSEEVENTF_RIGHTUP    = 0x0010 ;
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020 ;
        const int MOUSEEVENTF_MIDDLEUP   = 0x0040 ;
        const int MOUSEEVENTF_WHEEL      = 0x0080 ;
        const int MOUSEEVENTF_XDOWN      = 0x0100 ;
        const int MOUSEEVENTF_XUP        = 0x0200 ;
        const int MOUSEEVENTF_ABSOLUTE   = 0x8000 ;

        const int screen_length = 0x10000 ;  // for MOUSEEVENTF_ABSOLUTE

        public static void RightClick(int x, int y)
        {
            INPUT[] input = new INPUT[1] ;

            input[0].mi.dwFlags = MOUSEEVENTF_RIGHTDOWN ;
            input[0].mi.dx = x;
            input[0].mi.dy = y;
            input[0].mi.dwFlags = MOUSEEVENTF_RIGHTUP;

            SendInput(1, input, Marshal.SizeOf(input[0])) ;
        }
    }
}
