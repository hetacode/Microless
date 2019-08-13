using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Hetacode.Microless.Abstractions.Managers;
using Hetacode.Microless.Abstractions.MessageBus;
using Hetacode.Microless.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Hetacode.Microless.Managers
{
    public class FunctionsManager : IFunctionsManager
    {
        private readonly Dictionary<(Type, string), MethodInfo> _functions = new Dictionary<(Type, string), MethodInfo>();
        private readonly IServiceProvider _services;

        public FunctionsManager(IServiceProvider service) => _services = service;

        public void ScaffoldFunctions()
        {
            var methods = Assembly.GetEntryAssembly()
                                  .GetTypes()
                                  .Where(w => w.GetCustomAttributes().Any(a => a.GetType() == typeof(BindMessageAttribute)))
                                  .SelectMany(a => a.GetMethods().Where(w => w.Name == "Run" || w.Name == "Rollback"))
                                  .Select(s => new { Function = s, Attribute = s.DeclaringType.GetCustomAttribute<BindMessageAttribute>() })
                                  .ToList();

            methods.ForEach(f =>
            {
                _functions.Add((f.Attribute.MessageType, f.Function.Name), f.Function);
            });
        }

        public async Task CallFunction<T>(T message, Dictionary<string, string> headers = null)
        {

            using (var scope = _services.CreateScope())
            {
                var busSubscriptions = scope.ServiceProvider.GetService<IBusSubscriptions>();
                var context = new Context(busSubscriptions);
                context.Headers = headers;
                context.CorrelationId = context.GetCorrelationIdFromHeader();

                var subKey = context.IsRollback ? "Rollback" : "Run";
                var method = _functions[(message.GetType(), subKey)];
                var serviceType = method.DeclaringType;
                var functionInstance = scope.ServiceProvider.GetService(serviceType);
                var parameters = new object[] { context, message };
                var result = await (dynamic)method.Invoke(functionInstance, parameters);
            }
        }
    }
}
