using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BcyWebCrawler.Core
{
    public class Post
    {
        public long Uid { get; internal set; }

        public Author? Author { get; internal set; }

        public string Title { get; internal set; } = string.Empty;

        public Tag[]? Tags { get; internal set; }

        public string Description { get; internal set; } = string.Empty;

        public string[]? Albums { get; internal set; }
    }
}
