/* 
 * InfoItemPanel_A.xaml.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Sysphonic.Common;
using ThetisCore.Task;


namespace ThetisCore.Navi
{
    /// <summary>Grid Panel for Information Item.</summary>
    public partial class InfoItemPanel_A : UserControl, IInfoItemPanel
    {
        /// <summary>Information Item related to this panel.</summary>
        protected InfoItem _item = null;

        /// <summary>Background brush of the information (high-lighted).</summary>
        protected SolidColorBrush _bgBrushOn = null;

        /// <summary>Background brush of the information (normal).</summary>
        protected SolidColorBrush _bgBrushOff = null;

        /// <summary>Border brush of the selected information.</summary>
        protected SolidColorBrush _borderBrushSel = null;

        /// <summary>Selected flag.</summary>
        protected bool _selected = false;

        /// <summary>Constructor.</summary>
        /// <param name="item">Information Item related to this panel.</param>
        public InfoItemPanel_A(InfoItem item)
        {
            InitializeComponent();

            _bgBrushOn = new SolidColorBrush(Color.FromArgb(255, 145, 255, 251));
            _bgBrushOn.Opacity = 0.5;

            _bgBrushOff = new SolidColorBrush(Colors.Black);
            _bgBrushOff.Opacity = 0.25;

            _borderBrushSel = new SolidColorBrush(Colors.Lime);
            _borderBrushSel.Opacity = 0.8;

            _item = item;
            HorizontalAlignment = HorizontalAlignment.Stretch;

            if (item.Images != null && item.Images.Length > 0)
            {
//              imgThumb.Source = WpfUtil.LoadImage(item.Images[0]);
            }
            else
            {
                GridLengthConverter gridConv = new GridLengthConverter();
                GridLength imgWidth = (GridLength)gridConv.ConvertFromString("0px");
                colImg.Width = imgWidth;
            }
            txbItem.Text = item.SrcTitle + "\n" + item.Title;
            if (item.IsRead)
                txbItem.Foreground = Brushes.White;
            else
                txbItem.Foreground = Brushes.Yellow;

            WndMain wndMain = (WndMain)App.Current.MainWindow;

            MouseDown += new MouseButtonEventHandler(wndMain.infoItem_Click);
            MouseEnter += new MouseEventHandler(flowDoc_MouseEnter);
            MouseLeave += new MouseEventHandler(flowDoc_MouseLeave);

            Cursor = Cursors.Hand;
            ForceCursor = true;
            Background = _bgBrushOff;
            Height = 50;

/*
            Orientation = Orientation.Horizontal;
            Cursor = Cursors.Hand;
            ForceCursor = true;
            Background = _bgBrushOff;
            Height = 50;
            CanVerticallyScroll = false;

            if (item.Images != null && item.Images.Length > 0)
            {
                Image image = new Image();
                image.Source = CommonUtil.LoadImage(item.Images[0]);
                image.Width = 45;
                image.Height = 35;
                image.HorizontalAlignment = HorizontalAlignment.Left;
                image.VerticalAlignment = VerticalAlignment.Center;
                Children.Add(image);
            }

            WndMain wndMain = (WndMain)App.Current.MainWindow;

            HorizontalAlignment = HorizontalAlignment.Stretch;

            _txbContent = new TextBox();
            _txbContent.HorizontalAlignment = HorizontalAlignment.Stretch;
            _txbContent.TextWrapping = TextWrapping.Wrap;
            _txbContent.Text = item.SrcTitle + "\n" + item.Title;
            _txbContent.Background = Brushes.Transparent;
            _txbContent.BorderBrush = Brushes.Transparent;
            _txbContent.Foreground = Brushes.White;
            _txbContent.Focusable = false;
            _txbContent.FontSize = 12;
            Children.Add(_txbContent);

            MouseDown += new MouseButtonEventHandler(wndMain.infoItem_Click);
            MouseEnter += new MouseEventHandler(flowDoc_MouseEnter);
            MouseLeave += new MouseEventHandler(flowDoc_MouseLeave);
*/
/*
            Table table = new Table();
            table.Padding = new Thickness(0.0, 2.5, 0.0, 2.5);

            table.RowGroups.Add(new TableRowGroup());
            table.RowGroups[0].Rows.Add(new TableRow());
            table.RowGroups[0].Rows.Add(new TableRow());

            Paragraph titlePara = new Paragraph(new Run(item.Title));
            Paragraph descPara = new Paragraph(new Run(item.Description));

            if (item.Images != null && item.Images.Length > 0)
            {
                Image image = new Image();
                image.Source = CommonUtil.LoadImage(item.Images[0]);
                image.Width = 45;
                image.Height = 35;
                image.HorizontalAlignment = HorizontalAlignment.Left;
                image.VerticalAlignment = VerticalAlignment.Top;
                //image.Stretch = Stretch.Fill;
                //image.StretchDirection = StretchDirection.Both;

                BlockUIContainer imgBlock = new BlockUIContainer(image);
                Figure imgFig = new Figure(imgBlock);
                imgFig.Width = new FigureLength(50);
                imgFig.Height = new FigureLength(40);
                imgFig.Padding = new Thickness(0.0);
                imgFig.Margin = new Thickness(0.0);
                Paragraph imgPara = new Paragraph(imgFig);

                GridLengthConverter gridConv = new GridLengthConverter();
                GridLength glImage = (GridLength)gridConv.ConvertFromString("50px");
                TableColumn imageCol = new TableColumn();
                imageCol.Width = glImage;

                table.Columns.Add(imageCol);   // Image
                table.Columns.Add(new TableColumn());   // Title / Description

                TableCell imageCell = new TableCell(imgPara);
                imageCell.RowSpan = 2;
                table.RowGroups[0].Rows[0].Cells.Add(imageCell);
                table.RowGroups[0].Rows[0].Cells.Add(new TableCell(titlePara));
                table.RowGroups[0].Rows[1].Cells.Add(new TableCell(descPara));
            }
            else
            {
                table.Columns.Add(new TableColumn());   // Title / Description

                table.RowGroups[0].Rows[0].Cells.Add(new TableCell(titlePara));
                table.RowGroups[0].Rows[1].Cells.Add(new TableCell(descPara));
            }

            FlowDocument flowDoc = new FlowDocument(table);
            flowDoc.PagePadding = new Thickness(5.0, 5.0, 5.0, 5.0);
            flowDoc.FontSize = 10.0;
            if (_item.IsRead)
                flowDoc.Foreground = Brushes.White;
            else
                flowDoc.Foreground = Brushes.Yellow;
            flowDoc.Background = _bgBrushOff;

            Cursor = Cursors.Hand;
            ForceCursor = true;
            Height = 50;
            Document = flowDoc;
            VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

            WndMain wndMain = (WndMain)App.Current.MainWindow;
            flowDoc.MouseDown += new MouseButtonEventHandler(wndMain.infoItem_Click);
            flowDoc.MouseEnter += new MouseEventHandler(flowDoc_MouseEnter);
            flowDoc.MouseLeave += new MouseEventHandler(flowDoc_MouseLeave);
*/
        }

        #region IItemInfoPanel Member

        public bool HasThumb()
        {
            return (_item.Images != null && _item.Images.Length > 0);
        }

        public bool IsThumbLoaded()
        {
            return (imgThumb.Source != null);
        }

        public bool LoadThumb(System.EventHandler<RoutedEventArgs> onImageLoaded, System.EventHandler<ExceptionRoutedEventArgs> onImageFailed)
        {
            if (HasThumb())
            {
                if (onImageFailed != null)
                    imgThumb.ImageFailed += onImageFailed;
                if (onImageLoaded != null)
                    imgThumb.Loaded += new RoutedEventHandler(onImageLoaded);
                imgThumb.Source = WpfUtil.LoadImage(_item.Images[0]);
                return true;
            }
            else
                return false;
        }

        /// <summary>Gets Information Item related to this panel.</summary>
        /// <returns>Information Item related to this panel.</returns>
        public InfoItem GetInfoItem()
        {
            return _item;
        }

        /// <summary>Sets as selected or deselected.</summary>
        /// <param name="sel">Selected flag.</param>
        public void SetSelected(bool sel)
        {
            _selected = sel;

            if (sel)
            {
                BorderThickness = new Thickness(2.0, 1.0, 2.0, 1.0);
                BorderBrush = _borderBrushSel;
            }
            else
            {
                BorderThickness = new Thickness(0.0);
                BorderBrush = null;
            }
        }

        /// <summary>Gets if the mouse hovering on this panel.</summary>
        /// <returns>true if the mouse hovering on this panel, false otherwise.</returns>
        public bool IsMouseHovering()
        {
            return (Background == _bgBrushOn);
        }

        /// <summary>Marks as read.</summary>
        public void MarkRead()
        {
            _item.IsRead = true;
            txbItem.Foreground = Brushes.White;
        }

        /// <summary>Marks as unread.</summary>
        public void MarkUnread()
        {
            _item.IsRead = false;
            txbItem.Foreground = Brushes.Yellow;
        }

        #endregion

        /// <summary>Mouse Enter event handler for each Information Item.</summary>
        /// <param name="sender">Flow Document of the Information Item.</param>
        /// <param name="e">Mouse Event parameters.</param>
        public void flowDoc_MouseEnter(object sender, MouseEventArgs e)
        {
            Background = _bgBrushOn;
        }

        /// <summary>Mouse Leave event handler for each Information Item.</summary>
        /// <param name="sender">Flow Document of the Information Item.</param>
        /// <param name="e">Mouse Event parameters.</param>
        public void flowDoc_MouseLeave(object sender, MouseEventArgs e)
        {
            Background = _bgBrushOff;
        }
    }
}
