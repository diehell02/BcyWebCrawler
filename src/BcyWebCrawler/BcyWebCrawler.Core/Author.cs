using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BcyWebCrawler.Core
{
    public class Author
    {
        public string Name { get; internal set; } = string.Empty;

        public int Sex { get; internal set; }

        public string Location { get; internal set; } = string.Empty;

        public string SelfIntroduce { get; internal set; } = string.Empty;

        public string Avatar { get; internal set; } = string.Empty;

        public string Url { get; internal set; } = string.Empty;
    }
}
