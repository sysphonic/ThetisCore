using System;
using System.Collections;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;
using System.IO;
using System.Text;

namespace HTMLConverter {

  public partial class MainWindow : Window {

    public void convertContent(object sender, RoutedEventArgs e) {
      this.htmlToXamlTextBox.Text = HtmlToXamlConverter.ConvertHtmlToXaml(htmlToXamlTextBox.Text, true);
      MessageBox.Show("Content Conversion Complete!");
    }

    public void copyXAML(object sender, RoutedEventArgs e) {
      this.htmlToXamlTextBox.SelectAll();
      this.htmlToXamlTextBox.Copy();
    }

    public void convertContent2(object sender, RoutedEventArgs e) {
      this.xamlToHtmlTextBox.Text = HtmlFromXamlConverter.ConvertXamlToHtml(xamlToHtmlTextBox.Text);
      MessageBox.Show("Content Conversion Complete!");
    }

    public void copyHTML(object sender, RoutedEventArgs e) {
      this.xamlToHtmlTextBox.SelectAll();
      this.xamlToHtmlTextBox.Copy();
    }
  }
}