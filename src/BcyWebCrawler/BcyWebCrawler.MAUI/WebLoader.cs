// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BcyWebCrawler.Core;

namespace BcyWebCrawler.MAUI
{
    internal class WebLoader : WebView, IWebLoader
    {
        TaskCompletionSource<string>? _contentCompletionSource;

        public WebLoader() : base()
        {
            Navigated += WebLoader_Navigated;
        }

        private async void WebLoader_Navigated(object? sender, WebNavigatedEventArgs e)
        {
            if (e.Result == WebNavigationResult.Success)
            {
                string html = await EvaluateJavaScriptAsync("document.documentElement.outerHTML;");
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
            Eval("window.scroll(0,100000);");
            await Task.Delay(TimeSpan.FromSeconds(10));
            string html = await EvaluateJavaScriptAsync("document.documentElement.outerHTML;");
            html = Regex.Unescape(html);
            return html.Substring(1, html.Length - 2);
        }
    }
}
