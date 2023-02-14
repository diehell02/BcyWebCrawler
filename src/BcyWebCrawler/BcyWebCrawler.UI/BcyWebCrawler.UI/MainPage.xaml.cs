// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BcyWebCrawler.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.Extensions.DependencyInjection;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BcyWebCrawler.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            
            Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", Path.Combine(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "Incognito", new Random().Next().ToString())));
        }

        private void WebView2Loader_Loaded(object sender, RoutedEventArgs e)
        {
            DependencyInjection.ConfigureServices(sc => sc.AddSingleton<IWebLoader>(WebView2Loader));
            DependencyInjection.Build();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string link = PostLinkTextBox.Text;
            Post? post = await CrawlerFactory.GetPostCrawler().AnanyzeAsync(link);
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
