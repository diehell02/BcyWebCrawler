using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace BcyWebCrawler.Core
{
    public static class CrawlerFactory
    {
        public static ICrawler<Post?> GetPostCrawler()
        {
            return CallConstructor<PostCrawler>();
        }

        private static T CallConstructor<T>() where T : class
        {
            Type instanceType = typeof(T);
            ConstructorInfo[] constructorInfos = instanceType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (constructorInfos.Length > 1)
            {
                throw new Exception($"Constructor of {instanceType.Name} too many.");
            }
            ConstructorInfo constructorInfo = constructorInfos[0];
            List<object?> parameters = new();
            foreach (ParameterInfo parameterInfo in constructorInfo.GetParameters())
            {
                parameters.Add(DependencyInjection.ServiceProvider?.GetService(parameterInfo.ParameterType));
            }
            object? instance = Activator.CreateInstance(instanceType, parameters.ToArray());
            if (instance is T t)
            {
                return t;
            }
            else
            {
                throw new Exception($"{instanceType.Name} CreateInstance error.");
            }
        }
    }
}
