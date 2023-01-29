using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BcyWebCrawler.Core
{
    public interface ICrawler<T>
    {
        public Task<T> AnanyzeAsync(string url);
    }
}
