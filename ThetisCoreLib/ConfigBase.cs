/* 
 * ConfigBase.cs
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
    /// <summary>Configuration Base class.</summary>
    public abstract class ConfigBase
    {
        /// <summary>Configuration file name.</summary>
        protected abstract string CONFIG_FILE_NAME { get; }

        /// <summary>Constructor</summary>
        public ConfigBase()
        {
        }

        /// <summary>Saves all Information Items.</summary>
        public void Save()
        {
            if (!Directory.Exists(Def.CONFIG_ROOT_DIR))
            {
                Directory.CreateDirectory(Def.CONFIG_ROOT_DIR);
            }

            XmlSerializer writer = new XmlSerializer(this.GetType());

            using (StreamWriter file = new StreamWriter(Path.Combine(Def.CONFIG_ROOT_DIR, CONFIG_FILE_NAME)))
            {
                writer.Serialize(file, this);
            }
        }

        /// <summary>Loads Configuration Manager.</summary>
        /// <param name="subObj">Instance of sub-class.</param>
        /// <param name="fileName">Config file name.</param>
        /// <returns>Loaded Configuration Manager.</returns>
        static protected ConfigBase Load(ConfigBase subObj)
        {
            string configPath = Path.Combine(Def.CONFIG_ROOT_DIR, subObj.CONFIG_FILE_NAME);

            if (!File.Exists(configPath))
                return subObj;

            XmlSerializer reader = new XmlSerializer(subObj.GetType());

            try
            {
                using (StreamReader file = new StreamReader(configPath))
                {
                    return (ConfigBase)reader.Deserialize(file);
                }
            }
            catch (Exception e)
            {
                Log.AddError(e.Message + "\n" + e.StackTrace);
            }
            return subObj;
        }
    }
}
