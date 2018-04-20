/* 
 * InfoItemViewer.xaml.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Sysphonic.Common;
using ThetisCore.Task;

namespace ThetisCore.Navi
{
    /// <summary>Information Item Viewer class.</summary>
    public partial class InfoItemViewer : UserControl
    {
        /// <summary>Constructor.</summary>
        public InfoItemViewer()
        {
            InitializeComponent();
        }

        /// <summary>Constructor.</summary>
        /// <param name="item">Information Item.</param>
        public InfoItemViewer(InfoItem item)
        {
            InitializeComponent();

            TextBox titleBox = new TextBox();
            titleBox.FontSize = 16.0;
            titleBox.Text = item.Title;
            titleBox.BorderBrush = Brushes.Transparent;
            titleBox.VerticalAlignment = VerticalAlignment.Center;
            titleBox.IsReadOnly = true;

            TextBox autorBox = new TextBox();
            autorBox.FontSize = 14.0;
            if (item.Author != null && item.Author.Length > 0)
                autorBox.Text = " by " + item.Author;
            else
                autorBox.Text = "";
            autorBox.BorderBrush = Brushes.Transparent;
            autorBox.Foreground = Brushes.Navy;
            autorBox.VerticalAlignment = VerticalAlignment.Bottom;
            autorBox.IsReadOnly = true;

            stpBasicInfo.Children.Add(titleBox);
            stpBasicInfo.Children.Add(autorBox);

            GridLengthConverter gridConv = new GridLengthConverter();

            if (item.Images != null && item.Images.Length > 0)
            {
                GridLength imgHeight = (GridLength)gridConv.ConvertFromString("100px");
                grdRowImg.Height = imgHeight;
                if (((WndMain)App.Current.MainWindow).IsReachableTarget(item.GeneratorId))
                imgThumb.Source = WpfUtil.LoadImage(item.Images[0]);
            }
            else
            {
                GridLength imgHeight = (GridLength)gridConv.ConvertFromString("0px");
                grdRowImg.Height = imgHeight;
            }

            FlowDocument descFlow = null;
            try
            {
                string xaml = HTMLConverter.HtmlToXamlConverter.ConvertHtmlToXaml(item.Description, true);
                descFlow = System.Windows.Markup.XamlReader.Load(new System.Xml.XmlTextReader(new System.IO.StringReader(xaml))) as FlowDocument;
            }
            catch (Exception)
            {
                descFlow = new FlowDocument(new Paragraph(new Run(item.Description)));
            }
            scvContent.Document = descFlow;
        }
    }
}
