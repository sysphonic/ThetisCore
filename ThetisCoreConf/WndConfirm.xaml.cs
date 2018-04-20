/* 
 * WndConfirm.xaml.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System.Windows;

namespace ThetisCore.Conf
{
    /// <summary>Confirmation of the connection result dialog class.</summary>
    public partial class WndConfirm : Window
    {
        /// <summary>Constructor.</summary>
        public WndConfirm()
        {
            InitializeComponent();
        }

        /// <summary>Click event handler of the Yes button.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            if (tbxTitle.Text.Length > 0)
                ((WndTarget)Owner).tbxTitle.Text = tbxTitle.Text;
            Close();
        }

        /// <summary>Click event handler of the No button.</summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">Event parameters.</param>
        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
