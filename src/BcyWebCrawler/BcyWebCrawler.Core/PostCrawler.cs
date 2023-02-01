using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using BcyWebCrawler.Core.Context;
using BcyWebCrawler.Core.Utils;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BcyWebCrawler.Core
{
    internal class PostCrawler : ICrawler<Post?>
    {
        #region JSON

        private class ProductJson
        {
            public class Detail
            {
                public class PostData
                {
                    public long uid { get; set; }

                    public string plain { get; set; } = string.Empty;

                    public class PostImage
                    {
                        public long mid { get; set; }

                        public string original_path { get; set; } = string.Empty;

                        public int w { get; set; }

                        public int h { get; set; }

                        public string format { get; set; } = string.Empty;
                    }

                    public PostImage[]? multi { get; set; }

                    public string work { get; set; } = string.Empty;

                    public class PostTag
                    {
                        public long tag_id { get; set; }

                        public string tag_name { get; set; } = string.Empty;

                        public string type { get; set; } = string.Empty;
                    }

                    public PostTag[]? post_tags { get; set; }
                }

                public PostData? post_data { get; set; }

                public class PostUserInfo
                {
                    public long uid { get; set; }

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

        #endregion

        #region Static

        private const string BR = "<br>";
        private static readonly Regex s_jsonParseRegex = new(@"JSON\.parse\((.+)\);");

        #endregion

        private readonly IWebLoader _webLoader;

        public PostCrawler(IWebLoader webLoader)
        {
            _webLoader = webLoader;
        }

        public async Task<Post?> AnanyzeAsync(string url)
        {
            string content = GetContentAsync(url);
            return await AnalyzeContent(content);
        }

        private static async Task<Post?> AnalyzeContent(string content)
        {
            Match match = s_jsonParseRegex.Match(content);
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
            ProductJson.Detail.PostUserInfo? postUserInfo = productJson.detail?.post_user_info;
            if (postUserInfo is null)
            {
                return null;
            }
            string relativePath = Path.Combine($"{postUserInfo.uid}");
            using AuthorContext authorContext = new();
            authorContext.Database.EnsureCreated();
            await authorContext.Authors.LoadAsync();
            IQueryable<Author> queryAuthorResult = from a in authorContext.Authors
                             where a.AuthorId == postUserInfo.uid
                             select a;
            Author author;
            if (queryAuthorResult.Any())
            {
                author = queryAuthorResult.First();
                author.Update(postUserInfo.uname, postUserInfo.sex, postUserInfo.location, postUserInfo.self_intro);
                if (DateTime.Now - author.LastTimeAvatarUpdated > TimeSpan.FromDays(7))
                {
                    await ImageUtil.DownloadFileAsync(Regex.Unescape(postUserInfo.avatar), relativePath, author.Avatar);
                }
            }
            else
            {
                author = new()
                {
                    AuthorId = postUserInfo.uid,
                    Name = postUserInfo.uname,
                    Sex = postUserInfo.sex,
                    Location = postUserInfo.location,
                    SelfIntroduce = postUserInfo.self_intro,
                    Avatar = await ImageUtil.DownloadFileAsync(Regex.Unescape(postUserInfo.avatar), relativePath),
                    LastTimeAvatarUpdated = DateTime.Now,
                };
                authorContext.Authors.Add(author);
            }
            Post? post = null;
            ProductJson.Detail.PostData? postData = productJson.detail?.post_data;
            if (postData is not null)
            {
                await authorContext.Posts.LoadAsync();
                IQueryable<Post> queryPostResult = from p in authorContext.Posts
                                      where p.PostId == postData.uid
                                      select p;
                if (queryPostResult.Any())
                {
                    post = queryPostResult.First();
                    post.Update(title: HttpUtility.UrlDecode(postData.plain)?.Replace(BR, "") ?? string.Empty,
                        description: HttpUtility.UrlDecode(postData.plain)?.Replace(BR, $"{Environment.NewLine}") ?? string.Empty);
                }
                else
                {
                    post = new()
                    {
                        AuthorId = author.AuthorId,
                        Author = author,
                        PostId = postData.uid,
                        Title = HttpUtility.UrlDecode(postData.plain)?.Replace(BR, "") ?? string.Empty,
                        Description = HttpUtility.UrlDecode(postData.plain)?.Replace(BR, $"{Environment.NewLine}") ?? string.Empty
                    };
                    authorContext.Posts.Add(post);
                }
                ProductJson.Detail.PostData.PostTag[]? postTags = postData.post_tags;
                if (postTags is not null)
                {
                    foreach (ProductJson.Detail.PostData.PostTag tag in postTags)
                    {
                        IEnumerable<Tag> queryTagResult = from t in post.Tags
                                                         where t.TagId == tag.tag_id
                                                         select t;
                        if (!queryTagResult.Any())
                        {
                            post.Tags.Add(new Tag() { TagId = tag.tag_id, Name = tag.tag_name, Type = tag.type });
                        }
                    }
                }
                ProductJson.Detail.PostData.PostImage[]? multi = postData.multi;
                if (multi is not null)
                {
                    foreach (ProductJson.Detail.PostData.PostImage item in multi)
                    {
                        IEnumerable<Image> queryImageResult = from i in post.Images
                                                              where i.ImageId == item.mid
                                                              select i;
                        if (!queryImageResult.Any())
                        {
                            Image image = new()
                            {
                                ImageId = item.mid,
                                FileName = await ImageUtil.DownloadFileAsync(Regex.Unescape(item.original_path), relativePath)
                            };
                            post.Images.Add(image);
                        }
                    }
                }
            }
            await authorContext.SaveChangesAsync();
            return post;
        }

        private static string GetContentAsync(string url)
        {
            HtmlWeb web = new();
            HtmlDocument doc = web.Load(url);
            return doc.Text;
            //return await _webLoader.GetContentAsync(url);
        }
    }
}
