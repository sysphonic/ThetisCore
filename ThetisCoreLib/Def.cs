/* 
 * Def.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
#define NO_INSTALL

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sysphonic.Common;

namespace ThetisCore.Lib
{
    /// <summary>Common Definition class.</summary>
    public class Def
    {
        public const string PROC_ID_THETISCORE_CONF = @"ThetisCoreConf.sysphonic.com";

        /// <summary>Registry Path.</summary>
        private const string REG_PATH = @"Software\Sysphonic\ZeptairClient";

        /// <summary>Package sub directory.</summary>
        private const string PACKAGE_DIR_NAME = @"Sysphonic";

        /// <summary>Information file name.</summary>
        public const string INFO_ITEMS_FILE_NAME = @"infoItems.xml";

        /// <summary>Distribution file name.</summary>
        public const string DIST_ITEMS_FILE_NAME = @"distItems.xml";

        /// <summary>Sub directory of application data.</summary>
        public const string APP_DATA_FOLDER_NAME = @"ThetisCore";

        /// <summary>Path to application data.</summary>
        public static string COMMON_APP_DATA_DIR
        {
            get
            {
#if NO_INSTALL
                return Path.Combine(CommonUtil.GetAppPath(), @"data_common");
#else
				return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
#endif
			}
		}

        /// <summary>Path to application data.</summary>
        public static string APP_DATA_DIR
        {
            get
            {
#if NO_INSTALL
                return Path.Combine(CommonUtil.GetAppPath(), @"data");
#else
				return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
#endif
			}
		}

        /// <summary>Substring of path to application data.</summary>
        public static string APP_DATA_SUB_DIR
        {
            get
            {
                string ret = null;
#if NO_INSTALL
                ret = "";
#else
                if (PACKAGE_DIR_NAME == null || PACKAGE_DIR_NAME.Length == 0)
                    ret = APP_DATA_FOLDER_NAME;
                else
                    ret = Path.Combine(PACKAGE_DIR_NAME, APP_DATA_FOLDER_NAME);
#endif
                return ret;
            }
        }

        /// <summary>Root path of the informations.</summary>
        public static string INFO_ROOT_DIR
        {
            get
            {
                return String.Format(@"{0}/{1}/{2}",
                        APP_DATA_DIR,
                        APP_DATA_SUB_DIR, "info"
                    );
            }
        }

        /// <summary>Root path of the Zeptair Distribution.</summary>
        public static string ZEPT_DIST_ROOT_DIR
        {
            get
            {
                return String.Format(@"{0}/{1}/{2}",
                        APP_DATA_DIR,
                        APP_DATA_SUB_DIR, "dist"
                    );
            }
        }

        /// <summary>Information file path.</summary>
        public static string INFO_ITEMS_FILE_PATH
        {
            get
            {
                return String.Format(@"{0}/{1}", INFO_ROOT_DIR, INFO_ITEMS_FILE_NAME);
            }
        }

        /// <summary>Distribution file path.</summary>
        public static string DIST_ITEMS_FILE_PATH
        {
            get
            {
                return String.Format(@"{0}/{1}", ZEPT_DIST_ROOT_DIR, DIST_ITEMS_FILE_NAME);
            }
        }

        /// <summary>Root path of the Configuration for User.</summary>
        public static string CONFIG_ROOT_DIR
        {
            get
            {
                return String.Format(@"{0}/{1}/{2}",
                        APP_DATA_DIR,
                        APP_DATA_SUB_DIR, "config"
                    );
            }
        }

        /// <summary>Root path of the Common Configuration.</summary>
        public static string COMMON_CONFIG_ROOT_DIR
        {
            get
            {
                return String.Format(@"{0}/{1}/{2}",
                        COMMON_APP_DATA_DIR,
                        APP_DATA_SUB_DIR, "config"
                    );
            }
        }

        /// <summary>Install Path.</summary>
        public static string INSTALL_DIR
        {
            get
            {
#if NO_INSTALL
                return CommonUtil.GetAppPath();
#else
                Microsoft.Win32.RegistryKey regkey =
                    Microsoft.Win32.Registry.LocalMachine.OpenSubKey(REG_PATH, false);

                if (regkey == null)
                    return null;

                return (string)regkey.GetValue(@"InstallDir");
#endif
            }
        }
    }
}
