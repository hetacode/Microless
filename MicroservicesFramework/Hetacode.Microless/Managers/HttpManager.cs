using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hetacode.Microless.Abstractions.Managers;
using Hetacode.Microless.Attributes;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Hetacode.Microless.Abstractions.Functions;
using Hetacode.Microless.Abstractions.MessageBus;

namespace Hetacode.Microless.Managers
{
    public class HttpManager : IHttpManager
    {
        private readonly IServiceProvider _service;
        private readonly IBusSubscriptions _subscriptions;
        private Dictionary<string, (Type caller, HttpRequestAttribute attribute)> _callers = new Dictionary<string, (Type, HttpRequestAttribute)>();

        public HttpManager(IServiceProvider service, IBusSubscriptions subscriptions)
            => (_service, _subscriptions) = (service, subscriptions);

        public void Init()
        {
            AppDomain.CurrentDomain
                .GetAssemblies()
                .ToList()
                .ForEach(f =>
                {
                    var classes = f.GetTypes()
                                   .Where(w => w.GetCustomAttributes().Any(a => a.GetType() == typeof(HttpRequestAttribute)))
                                   .Select(s => new { Caller = s, Attribute = s.GetCustomAttribute<HttpRequestAttribute>() })
                                   .ToList();
                    classes.ForEach(fe =>
                    {
                        _callers.Add(fe.Attribute.Endpoint, (fe.Caller, fe.Attribute));
                    });
                });

        }

        public async void ResolveAndCall(string endpoint, string body, Dictionary<string, string> headers, Action<object, Dictionary<string, string>> responseAction)
        {
            if (!_callers.ContainsKey(endpoint))
            {
                Console.WriteLine($"Endpoint {endpoint} doesn't exists");
                return;
            }

            var caller = _callers[endpoint];
            var message = JsonConvert.DeserializeObject(body, caller.attribute.RequestType);
            var instance = _service.GetService(caller.caller);
            if (caller.caller.GetInterfaces().Any(a => a.GetGenericTypeDefinition() == typeof(IFunction<>)))
            {
                var method = instance.GetType().GetMethods().First(m => m.Name == "Run");
                var serviceType = method.DeclaringType;
                var context = new Context(_subscriptions);
                context.ResponseActionDelegate = responseAction;
                context.Headers = headers;
                context.CorrelationId = context.GetCorrelationIdFromHeader();
                context.SetSenderToHeader(caller.attribute.QueueName);
                var parameters = new object[] { context, message };
                await (dynamic)method.Invoke(instance, parameters);
            }
            else
            {
                // The same call method such as above ( function
                // but in future could be changed
                var method = instance.GetType().GetMethods().First(m => m.Name == "Run");
                var serviceType = method.DeclaringType;
                var context = new Context(_subscriptions);
                context.Headers = headers;
                context.ResponseActionDelegate = responseAction;
                context.CorrelationId = context.GetCorrelationIdFromHeader();
                context.SetSenderToHeader(caller.attribute.QueueName);
                var parameters = new object[] { context, message };
                method.Invoke(instance, parameters);
            }
        }
    }
}
