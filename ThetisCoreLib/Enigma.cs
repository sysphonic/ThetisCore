/* 
 * Enigma.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System;

namespace ThetisCore.Lib
{
    /// <summary>Custom Encrypt / Decrypt class.</summary>
    public class Enigma
    {
        /// <summary>Encrypt parameter (IN)</summary>.
        private static char[] params_in =  {'b','e','d','a','f','c'};

        /// <summary>Encrypt parameter (OUT)</summary>.
        private static char[] params_out = { '@', ';', ':', '}', '(', '!' };


        /// <summary>Encrypts the argument.</summary>.
        /// <param name="text">Target string.</param>
        static public string Encrypt(string text)
        {
            if (text == null)
                return null;

            string ret = "";
            char[] chars = text.ToCharArray();
            
            for (int i=chars.Length-1; i>=0; i--)
            {
                int val = (int)chars[i]-0x20;
                val += 18;
                string strVal = val.ToString("X");
                strVal.PadRight(2, '0');
                ret += strVal;
            }
            for (int i=0; i<params_in.Length; i++) {
                ret = ret.Replace(params_in[i], params_out[i]);
            }
            return ret;
        }

        /// <summary>Dencrypts the argument.</summary>.
        /// <param name="text">Target code.</param>
        static public string Decrypt(string code)
        {
            if (code == null)
                return null;

            try
            {
                string ret = "";

                for (int i=0; i<params_in.Length; i++) {
                    code = code.Replace(params_out[i], params_in[i]);
                }
                for (int idx=0; idx < code.Length; idx+=2) {
                    string strVal = code.Substring(idx, 2);
                    int val = Int32.Parse(strVal, System.Globalization.NumberStyles.HexNumber);
                    val -= 18;
                    val += 0x20;
                    char[] chars = { (char)val };
                    strVal = new string(chars);
                    ret = strVal + ret;
                }

                return ret;

            } catch (Exception) {
                return code;
            }
        }
    }
}
