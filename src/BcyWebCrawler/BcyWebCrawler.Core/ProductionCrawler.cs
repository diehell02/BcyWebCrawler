using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using BcyWebCrawler.Core.Utils;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;

namespace BcyWebCrawler.Core
{
    internal class ProductionCrawler : ICrawler<Post?>
    {
        private class ProductJson
        {
            public class Detail
            {
                public class PostData
                {
                    public long uid { get; set; }

                    public string plain { get; set; } = string.Empty;

                    public class Image
                    {
                        public string original_path { get; set; } = string.Empty;

                        public int w { get; set; }

                        public int h { get; set; }

                        public string format { get; set; } = string.Empty;
                    }

                    public Image[]? multi { get; set; }

                    public string work { get; set; } = string.Empty;

                    public class PostTag
                    {
                        public string tag_name { get; set; } = string.Empty;

                        public string type { get; set; } = string.Empty;
                    }

                    public PostTag[]? post_tags { get; set; }
                }

                public PostData? post_data { get; set; }

                public class PostUserInfo
                {
                    public string uname { get; set; } = string.Empty;

                    public int sex { get; set; }

                    public string location { get; set; } = string.Empty;

                    public string self_intro { get; set; } = string.Empty;

                    public string avatar { get; set; } = string.Empty;
                }

                public PostUserInfo? post_user_info { get; set; }
            }

            public Detail? detail { get; set; }
        }

        private readonly IWebLoader _webLoader;

        public ProductionCrawler(IWebLoader webLoader)
        {
            _webLoader = webLoader;
        }

        public async Task<Post?> AnanyzeAsync(string url)
        {
            string content = await GetContentAsync(url);
            return await AnalyzeContent(content);
        }

        private async Task<Post?> AnalyzeContent(string content)
        {
            Regex regex = new(@"JSON\.parse\((.+)\);");
            Match match = regex.Match(content);
            if (!match.Success)
            {
                return null;
            }
            string json = match.Groups[1].Value;
            json = json.Replace("\\\"", "\"");
            json = json.Substring(1, json.Length - 2);
            ProductJson? productJson;
            try
            {
                productJson = JsonSerializer.Deserialize<ProductJson>(json);
            }
            catch (ArgumentNullException)
            {
                return null;
            }
            catch (JsonException)
            {
                return null;
            }
            catch (NotSupportedException)
            {
                return null;
            }
            if (productJson is null)
            {
                return null;
            }
            Post post = new();
            ProductJson.Detail.PostUserInfo? postUserInfo = productJson.detail?.post_user_info;
            if (postUserInfo is not null)
            {
                Author author = new()
                {
                    Name = postUserInfo.uname,
                    Sex = postUserInfo.sex,
                    Location = postUserInfo.location,
                    SelfIntroduce = postUserInfo.self_intro,
                    Avatar = await ImageUtil.DownloadFileAsync(Regex.Unescape(postUserInfo.avatar)),
                };
                post.Author = author;
            }
            ProductJson.Detail.PostData? postData = productJson.detail?.post_data;
            if (postData is not null)
            {
                post.Uid = postData.uid;
                post.Title = HttpUtility.UrlDecode(postData.plain)?.Replace("<br>", "") ?? string.Empty;
                post.Description = HttpUtility.UrlDecode(postData.plain)?.Replace("<br>", $"{Environment.NewLine}") ?? string.Empty;
                ProductJson.Detail.PostData.PostTag[]? postTags = postData.post_tags;
                if (postTags is not null)
                {
                    List<Tag> tags = new();
                    foreach (ProductJson.Detail.PostData.PostTag tag in postTags)
                    {
                        tags.Add(new Tag() { Name = tag.tag_name, Type = tag.type });
                    }
                    post.Tags = tags.ToArray();
                }
                ProductJson.Detail.PostData.Image[]? multi = postData.multi;
                if (multi is not null)
                {
                    List<string> albums = new();
                    foreach (ProductJson.Detail.PostData.Image item in multi)
                    {
                        albums.Add(await ImageUtil.DownloadFileAsync(Regex.Unescape(item.original_path)));
                    }
                }
            }
            return post;
        }

        private async Task<string> GetContentAsync(string url)
        {
            HtmlWeb web = new();
            HtmlDocument doc = web.Load(url);
            return doc.Text;
            //return await _webLoader.GetContentAsync(url);
        }
    }
}
