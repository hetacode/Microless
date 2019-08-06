using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Service.Core.Attributes;

namespace Service.Core
{
    public class FunctionsManager
    {
        private readonly Dictionary<Type, MethodInfo> _functions = new Dictionary<Type, MethodInfo>();
        private readonly IServiceProvider _services;

        public FunctionsManager(IServiceProvider service) => _services = service;

        public void ScaffoldFunctions()
        {
            var methods = Assembly.GetAssembly(typeof(Startup))
                                  .GetTypes()
                                  .SelectMany(a => a.GetMethods())
                                  .Where(w => w.GetCustomAttributes().Any(a => a.GetType() == typeof(BindMessageAttribute)))
                                  .Select(s => new { Function = s, Attribute = s.GetCustomAttribute<BindMessageAttribute>() })
                                  .ToList();

            methods.ForEach(f =>
            {
                _functions.Add(f.Attribute.MessageType, f.Function);
            });
        }

        public async Task CallFunction<T>(T message)
        {
            var method = _functions[message.GetType()];
            var serviceType = method.DeclaringType;
            using (var scope = _services.CreateScope())
            {
                var functionInstance = scope.ServiceProvider.GetService(serviceType);
                var parameters = new object[] { new Context(), message };
                var result = await (dynamic)method.Invoke(functionInstance, parameters);
            }
        }

        //public MethodInfo GetFunction(Type messageType)
        //{
        //    return _functions[messageType];
        //}
    }
}
