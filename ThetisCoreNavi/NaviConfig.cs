/* 
 * NaviConfig.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System;
using System.IO;
using System.Xml.Serialization;
using Sysphonic.Common;

namespace ThetisCore.Navi
{
    /// <summary>Configuration class for ThetisCoreNavi.</summary>
    public class NaviConfig : ThetisCore.Lib.ConfigBase
    {
        /// <summary>Configuration file name.</summary>
        protected override string CONFIG_FILE_NAME { get { return @"navi.config"; } }

        /// <summary>Width of the Navi window.</summary>
        protected double _naviWidth = 280;

        /// <summary>Height of the Navi window.</summary>
        protected double _naviHeight = 380;

        /// <summary>Width of the Wing window.</summary>
        protected double _wingWidth = 400;

        /// <summary>Height of the Wing window.</summary>
        protected double _wingHeight = 350;


        /// <summary>Constructor</summary>
        public NaviConfig()
        {
        }

        /// <summary>Width of the Navi window.</summary>
        public double NaviWidth
        {
            get { return _naviWidth; }
            set { _naviWidth = value; }
        }

        /// <summary>Height of the Navi window.</summary>
        public double NaviHeight
        {
            get { return _naviHeight; }
            set { _naviHeight = value; }
        }

        /// <summary>Width of the Wing window.</summary>
        public double WingWidth
        {
            get { return _wingWidth; }
            set { _wingWidth = value; }
        }

        /// <summary>Height of the Wing window.</summary>
        public double WingHeight
        {
            get { return _wingHeight; }
            set { _wingHeight = value; }
        }

        /// <summary>Loads Configuration for ThetisCoreNavi.</summary>
        /// <returns>Configuration for ThetisCoreNavi.</returns>
        static public NaviConfig Load()
        {
            return (NaviConfig)ThetisCore.Lib.ConfigBase.Load(new NaviConfig());
        }
    }
}
