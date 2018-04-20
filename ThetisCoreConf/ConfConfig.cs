/* 
 * ConfConfig.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System;
using System.IO;
using System.Xml.Serialization;
using Sysphonic.Common;

namespace ThetisCore.Conf
{
    /// <summary>Configuration class for ThetisCoreConf.</summary>
    public class ConfConfig : ThetisCore.Lib.ConfigBase
    {
        /// <summary>Configuration file name.</summary>
        protected override string CONFIG_FILE_NAME { get { return @"conf.config"; } }

        /// <summary>Width of the Settings window.</summary>
        protected double _settingsWidth = 450;

        /// <summary>Height of the Settings window.</summary>
        protected double _settingsHeight = 400;


        /// <summary>Constructor</summary>
        public ConfConfig()
        {
        }

        /// <summary>Width of the Settings window.</summary>
        public double SettingsWidth
        {
            get { return _settingsWidth; }
            set { _settingsWidth = value; }
        }

        /// <summary>Height of the Settings window.</summary>
        public double SettingsHeight
        {
            get { return _settingsHeight; }
            set { _settingsHeight = value; }
        }

        /// <summary>Loads Configuration for ThetisCoreConf.</summary>
        /// <returns>Configuration for ThetisCoreConf.</returns>
        static public ConfConfig Load()
        {
            return (ConfConfig)ThetisCore.Lib.ConfigBase.Load(new ConfConfig());
        }
    }
}
