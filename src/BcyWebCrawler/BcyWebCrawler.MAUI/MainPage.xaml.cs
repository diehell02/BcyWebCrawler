// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using BcyWebCrawler.Core;

namespace BcyWebCrawler.MAUI
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", Path.Combine(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "Incognito", new Random().Next().ToString())));
            InitializeComponent();
        }

        private void WebView2Loader_Loaded(object sender, EventArgs e)
        {
            DependencyInjection.ConfigureServices(sc => sc.AddSingleton<IWebLoader>(WebView2Loader));
            DependencyInjection.Build();
        }

        private async void Button_Clicked(object sender, EventArgs e)
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

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            string link = WebLinkTextBox.Text;
            OutputTextBox.Text = await WebView2Loader.GetContentAsync(link);
        }

        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            OutputTextBox.Text = await WebView2Loader.ScrollToEnd();
        }
    }
}
