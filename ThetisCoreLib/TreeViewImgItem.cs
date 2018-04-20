/* 
 * TreeViewImgItem.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Sysphonic.Common
{
    /// <summary>Custom TreeViewItem class with Image.</summary>
    public class TreeViewImgItem : TreeViewItem
    {
        protected string _type = null;
        protected TextBlock _text = null;
        protected Image _img = null;
        protected ImageSource _srcSelected = null;
        protected ImageSource _srcUnselected = null;
        protected object _additionalInfo = null;

        /// <summary>Constructor.</summary>
        public TreeViewImgItem(string type)
        {
            _type = type;

            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            Header = stack;

            _img = new Image();
            _img.Width = 14;
            _img.Height = 14;
            _img.VerticalAlignment = VerticalAlignment.Center;
            _img.Margin = new Thickness(0, 2, 3, 2);
            _img.Source = _srcSelected;
            stack.Children.Add(_img);

            _text = new TextBlock();
            _text.VerticalAlignment = VerticalAlignment.Center;
            stack.Children.Add(_text);
        }

        /// <summary>Type.</summary>
        public string Type
        {
            set { _type = value; }
            get { return _type; }
        }

        /// <summary>Text.</summary>
        public string Text
        {
            set { _text.Text = value; }
            get { return _text.Text; }
        }

        /// <summary>Additional Information.</summary>
        public object AdditionalInfo
        {
            set { _additionalInfo = value; }
            get { return _additionalInfo; }
        }

        /// <summary>Selected Image.</summary>
        public ImageSource SelectedImage
        {
            set
            {
                _srcSelected = value; 
                _img.Source = _srcSelected;
            }

            get { return _srcSelected; }
        }

        /// <summary>Unselected Image.</summary>
        public ImageSource UnselectedImage
        {
            set
            {
                _srcUnselected = value; 
            }
            get { return _srcUnselected; }
        }

        /// <summary>Selected event handler.</summary>
        /// <param name="args">Event parameters.</param>
        protected override void OnSelected(RoutedEventArgs args)
        {
            base.OnSelected(args);
            _img.Source = _srcSelected;
        }

        /// <summary>Unselected event handler.</summary>
        /// <param name="args">Event parameters.</param>
        protected override void OnUnselected(RoutedEventArgs args)
        {
            base.OnUnselected(args);

            if (_srcUnselected != null)
                _img.Source = _srcUnselected;
            else
                _img.Source = _srcSelected;
        }
    }
}
