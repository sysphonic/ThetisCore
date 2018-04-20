/* 
 * WpfUtil.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System;
using System.Net;

namespace Sysphonic.Common
{
    /// <summary>WPF Utility Library class.</summary>
    public class WpfUtil
    {
        #region Image

        /// <summary>Loads an image from the specified URI.</summary>
        /// <param name="path">Path of the image.</param>
        /// <returns>BitmapImage object.</returns>
        public static System.Windows.Media.Imaging.BitmapImage LoadImage(string path)
        {
            ServicePointManager.ServerCertificateValidationCallback = ThetisCore.Lib.EasyTrustPolicy.CheckValidationResult;
            System.Windows.Media.Imaging.BitmapImage bmpImg = new System.Windows.Media.Imaging.BitmapImage();
            bmpImg.BeginInit();

            // Necessary to prevent from System.ArgumentException in PresentationCore.dll
            bmpImg.CreateOptions = System.Windows.Media.Imaging.BitmapCreateOptions.IgnoreColorProfile;

            bmpImg.UriSource = new Uri(path);
            bmpImg.EndInit();
            return bmpImg;
        }

        #endregion Image
    }
}
