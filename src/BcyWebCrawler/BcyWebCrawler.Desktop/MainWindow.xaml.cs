using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using BcyWebCrawler.Core;
using Microsoft.Extensions.DependencyInjection;

namespace BcyWebCrawler.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", Path.Combine(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "Incognito", new Random().Next().ToString())));
            InitializeComponent();
        }

        private void WebView2Loader_Loaded(object sender, RoutedEventArgs e)
        {
            DependencyInjection.ConfigureServices(sc => sc.AddSingleton<IWebLoader>(WebView2Loader));
            DependencyInjection.Build();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string link = PostLinkTextBox.Text;
            Post? post = await CrawlerFactory.GetProductCrawler().AnanyzeAsync(link);
            if (post is null)
            {
                OutputTextBox.Text = "Crawl Failed";
            }
            else
            {
                OutputTextBox.Text = "Crawl Succeed";
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string link = WebLinkTextBox.Text;
            OutputTextBox.Text = await WebView2Loader.GetContentAsync(link);
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OutputTextBox.Text = await WebView2Loader.ScrollToEnd();
        }
    }
}
