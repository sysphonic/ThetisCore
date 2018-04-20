/* 
 * IInfoItemPanel.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System.Windows;
using ThetisCore.Task;

namespace ThetisCore.Navi
{
    public interface IInfoItemPanel
    {
        bool IsThumbLoaded();
        bool HasThumb();
        bool LoadThumb(System.EventHandler<RoutedEventArgs> onImageLoaded, System.EventHandler<ExceptionRoutedEventArgs> onImageFailed);

        /// <summary>Gets Information Item related to this panel.</summary>
        /// <returns>Information Item related to this panel.</returns>
        InfoItem GetInfoItem();

        /// <summary>Sets as selected or deselected.</summary>
        /// <param name="sel">Selected flag.</param>
        void SetSelected(bool sel);

        /// <summary>Gets if the mouse hovering on this panel.</summary>
        /// <returns>true if the mouse hovering on this panel, false otherwise.</returns>
        bool IsMouseHovering();

        /// <summary>Marks as read.</summary>
        void MarkRead();

        /// <summary>Marks as unread.</summary>
        void MarkUnread();

    }
}
