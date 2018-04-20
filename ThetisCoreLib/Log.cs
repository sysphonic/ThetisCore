/* 
 * Log.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sysphonic.Common
{
    /// <summary>Log class.</summary>
    public class Log
    {
        static Log() {
            log4net.Config.XmlConfigurator.Configure(
                    new System.IO.FileInfo(
                            ThetisCore.Lib.Def.COMMON_CONFIG_ROOT_DIR + "/log.xml"
                        )
                );
        }

        /// <summary>Logger instance.</summary>
        private static log4net.ILog _logger = 
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>Adds Debug log.</summary>
        /// <param name="log">Log string.</param>
        public static void AddDebug(string log)
        {
            _logger.Debug(log);
        }

        /// <summary>Adds Info log.</summary>
        /// <param name="log">Log string.</param>
        public static void AddInfo(string log)
        {
            _logger.Info(log);
        }

        /// <summary>Adds Warning log.</summary>
        /// <param name="log">Log string.</param>
        public static void AddWarn(string log)
        {
            _logger.Warn(log);
        }

        /// <summary>Adds Error log.</summary>
        /// <param name="log">Log string.</param>
        public static void AddError(string log)
        {
            _logger.Error(log);
        }

        /// <summary>Adds Fatal log.</summary>
        /// <param name="log">Log string.</param>
        public static void AddFatal(string log)
        {
            _logger.Fatal(log);
        }

    }
}
