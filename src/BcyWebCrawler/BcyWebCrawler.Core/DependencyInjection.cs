using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BcyWebCrawler.Core
{
    public class DependencyInjection
    {
        private static readonly DependencyInjection s_instance = new();
        private static readonly IServiceCollection s_serviceCollection = new ServiceCollection();
        private static ServiceProvider? s_serviceProvider;

        public static ServiceProvider? ServiceProvider => s_serviceProvider;

        private DependencyInjection() { }

        public static DependencyInjection ConfigureServices(Action<IServiceCollection> action)
        {
            action(s_serviceCollection);
            return s_instance;
        }

        public static void Build() => s_serviceProvider = s_serviceCollection.BuildServiceProvider();
    }
}
