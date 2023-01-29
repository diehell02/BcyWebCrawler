using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BcyWebCrawler.Core
{
    public interface IWebLoader
    {
        public Task<string> GetContentAsync(string url);

        public Task<string> DownloadFileAsync(string url);
    }
}
