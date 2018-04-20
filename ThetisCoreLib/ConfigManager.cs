/* 
 * ConfigManager.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System;
using System.IO;
using System.Xml.Serialization;
using Sysphonic.Common;

namespace ThetisCore.Lib
{
    /// <summary>Configuration Manager class.</summary>
    public class ConfigManager : ConfigBase
    {
        /// <summary>Configuration file name.</summary>
        protected override string CONFIG_FILE_NAME { get { return @"settings.xml"; } }

        /// <summary>Max. of the Information Items.</summary>
        protected int _maxItemsOnPanel = 30;


        /// <summary>Constructor</summary>
        public ConfigManager()
        {
        }

        /// <summary>Max. of the Information Items.</summary>
        public int MaxItemsOnPanel
        {
            get { return _maxItemsOnPanel; }
            set { _maxItemsOnPanel = value; }
        }

        /// <summary>Loads Configuration.</summary>
        /// <returns>Configuration Manager.</returns>
        static public ConfigManager Load()
        {
            return (ConfigManager)ThetisCore.Lib.ConfigBase.Load(new ConfigManager());
        }
    }
}
