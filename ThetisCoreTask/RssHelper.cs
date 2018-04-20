/* 
 * RssHelper.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using Rss;

namespace ThetisCore.Task
{
    /// <summary>RSS Helper class.</summary>
    class RssHelper
    {
        /// <summary>Gets index of specified RSS Channel in the collection.</summary>
        /// <param name="channels">Target collection.</param>
        /// <param name="channel">Target channel.</param>
        /// <returns>Index if found, -1 otherwise.</returns>
        public static int IndexOf(RssChannelCollection channels, RssChannel channel)
        {
            int idx = 0;

            foreach (RssChannel ch in channels)
            {
                if (ch.Title == channel.Title && ch.Link == channel.Link)
                {
                    return idx;
                }
                idx++;
            }
            return -1;
        }

        /// <summary>Gets if the specified RSS Channel is in the collection.</summary>
        /// <param name="channels">Target collection.</param>
        /// <param name="channel">Target channel.</param>
        /// <returns>true if found, false otherwise.</returns>
        public static bool Contains(RssChannelCollection channels, RssChannel channel)
        {
            return (IndexOf(channels, channel) >= 0);
        }

        /// <summary>Gets index of specified RSS Item in the collection.</summary>
        /// <param name="items">Target collection.</param>
        /// <param name="item">Target item.</param>
        /// <returns>Index if found, -1 otherwise.</returns>
        public static int IndexOf(RssItemCollection items, RssItem item)
        {
            int idx = 0;

            foreach (RssItem it in items)
            {
                if ((item.Guid == null || item.Guid.Name == null || item.Guid.Name.Length <= 0)
                    && (it.Guid == null || it.Guid.Name == null || it.Guid.Name.Length <= 0))
                {
                    if (item.Title == it.Title && item.Link == it.Link && item.PubDate == it.PubDate)
                        return idx;
                }
                else if ((item.Guid != null && it.Guid != null)
                    && (item.Guid.Name != null && item.Guid.Name.Length > 0)
                    && (item.Guid.Name == it.Guid.Name && item.PubDate == it.PubDate))
                    return idx;

                idx++;
            }
            return -1;
        }

        /// <summary>Gets if the specified RSS Item is in the collection.</summary>
        /// <param name="items">Target collection.</param>
        /// <param name="item">Target item.</param>
        /// <returns>true if found, false otherwise.</returns>
        public static bool Contains(RssItemCollection items, RssItem item)
        {
            return (IndexOf(items, item) >= 0);
        }

    }
}
