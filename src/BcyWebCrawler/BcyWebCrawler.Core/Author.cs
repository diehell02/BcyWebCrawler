using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BcyWebCrawler.Core
{
    public class Author
    {
        public long AuthorId { get; internal set; }

        public string Name { get; internal set; } = string.Empty;

        public int Sex { get; internal set; }

        public string Location { get; internal set; } = string.Empty;

        public string SelfIntroduce { get; internal set; } = string.Empty;

        public string Avatar { get; internal set; } = string.Empty;

        public DateTime LastTimeAvatarUpdated { get; internal set; }

        public string Url { get; internal set; } = string.Empty;

        public List<Post> Posts { get; } = new();

        public void Update(string name, int sex, string location, string selfIntroduce)
        {
            if (!string.IsNullOrWhiteSpace(name)) Name = name;
            Sex = sex;
            if (!string.IsNullOrWhiteSpace(location)) Location = location;
            if (!string.IsNullOrWhiteSpace(selfIntroduce)) SelfIntroduce = selfIntroduce;
        }
    }
}
