using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BcyWebCrawler.Core
{
    public class Post
    {
        public long PostId { get; internal set; }

        public long AuthorId { get; internal set; }

        public Author? Author { get; internal set; }

        public string Title { get; internal set; } = string.Empty;

        public List<Tag> Tags { get; internal set; } = new();

        public string Description { get; internal set; } = string.Empty;

        public List<Image> Images { get; internal set; } = new();

        public void Update(string title, string description)
        {
            if (!string.IsNullOrWhiteSpace(title)) Title = title;
            if (!string.IsNullOrWhiteSpace(description)) Description = description;
        }
    }
}
