// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BcyWebCrawler.Core;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;

namespace BcyWebCrawler.UI
{
    internal class WebLoader : WebView2, IWebLoader
    {
        //TaskCompletionSource<string>? _downloadFileCompletionSource;
        TaskCompletionSource<string>? _contentCompletionSource;

        public WebLoader() : base()
        {
            NavigationCompleted += WebLoader_NavigationCompleted;
            NavigationStarting += WebLoader_NavigationStarting;
        }

        private void WebLoader_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
        }

        private async void WebLoader_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                string html = await ExecuteScriptAsync("document.documentElement.outerHTML;");
                html = Regex.Unescape(html);
                html = html.Substring(1, html.Length - 2);
                _contentCompletionSource?.TrySetResult(html);
            }
        }

        public async Task<string> DownloadFileAsync(string url)
        {
            await Task.CompletedTask;
            return string.Empty;
        }

        public async Task<string> GetContentAsync(string url)
        {
            Source = new Uri(url);
            _contentCompletionSource = new TaskCompletionSource<string>();
            return await _contentCompletionSource.Task;
        }

        public async Task<string> ScrollToEnd()
        {
            return string.Empty;
            //await ExecuteScriptAsync("window.scroll(0,100000);");
            //await Task.Delay(TimeSpan.FromSeconds(10));
            //string html = await ExecuteScriptAsync("document.documentElement.outerHTML;");
            //html = Regex.Unescape(html);
            //return html.Substring(1, html.Length - 2);
        }
    }
}
