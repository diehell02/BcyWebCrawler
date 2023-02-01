// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private const string ASSERT = "Assert";
        private const string IMAGES = "Images";

        private static DateTime s_lastDownLoadTime = DateTime.MinValue;
        private static readonly Regex s_resMediaTypeRegex = new(@"image/(\w+)");

        private static readonly string s_imageDirectory;

        static ImageUtil()
        {
            s_imageDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ASSERT, IMAGES);
            if (!Directory.Exists(s_imageDirectory))
            {
                Directory.CreateDirectory(s_imageDirectory);
            }
        }

        public static async Task<string> DownloadFileAsync(string url, string relativePath, string? fileName = null)
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
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = $"{Guid.NewGuid()}";
            }
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
                        if (!string.IsNullOrWhiteSpace(mediaType))
                        {
                            Match match = s_resMediaTypeRegex.Match(mediaType);
                            if (match.Success)
                            {
                                fileName = $"{fileName}.{match.Groups[1]}";
                            }
                        }
                        string directory = Path.Combine(s_imageDirectory, relativePath);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                        string filePath = Path.Combine(directory, fileName);
                        using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
                        {
                            await response.Content.CopyToAsync(fs);
                        }
                    }
                }
            }
            return fileName;
        }
    }
}
