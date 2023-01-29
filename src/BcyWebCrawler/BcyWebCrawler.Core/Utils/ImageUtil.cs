// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace BcyWebCrawler.Core.Utils
{
    internal class ImageUtil
    {
        private static DateTime s_lastDownLoadTime = DateTime.MinValue;
        private static Regex s_resMediaTypeRegex = new(@"image/(\w+)");

        public static async Task<string> DownloadFileAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }
            if (DateTime.Now - s_lastDownLoadTime < TimeSpan.FromSeconds(10))
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
            s_lastDownLoadTime = DateTime.Now;
            string fileName = $"{Guid.NewGuid()}";
            StringBuilder filePathBuilder = new();
            filePathBuilder.Append($"{GetImageDirectory()}\\{fileName}");
            using (var handler = new HttpClientHandler())
            {
                handler.UseDefaultCredentials = true;
                using (var client = new HttpClient(handler))
                {
                    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    HttpRequestHeaders headers = client.DefaultRequestHeaders;
                    HttpRequestMessage httpRequestMessage = new(HttpMethod.Get, url);
                    using (HttpResponseMessage response = await client.SendAsync(httpRequestMessage))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            return string.Empty;
                        }
                        string? mediaType = response.Content.Headers.ContentType?.MediaType;
                        if (!string.IsNullOrEmpty(mediaType))
                        {
                            Match match = s_resMediaTypeRegex.Match(mediaType);
                            if (match.Success)
                            {
                                filePathBuilder.Append($".{match.Groups[1]}");
                            }
                        }
                        using (var fs = new FileStream(filePathBuilder.ToString(), FileMode.OpenOrCreate))
                        {
                            await response.Content.CopyToAsync(fs);
                        }
                    }
                }
            }
            return fileName;
        }

        private static string GetImageDirectory()
        {
            string directory = $"{AppDomain.CurrentDomain.BaseDirectory}\\Assert\\Images";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return directory;
        }
    }
}
