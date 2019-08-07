using System;
using System.Reflection;
using Hetacode.Microless.Abstractions.MessageBus;
using Hetacode.Microless.Managers;
using Hetacode.Microless.MessageBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Hetacode.Microless.Extensions
{
    public static class DependencyInjection
    {
        public static void AddMicroless(this IServiceCollection services)
        {
            services.AddSingleton<FunctionsManager>();

            // Register all Functions
            services.Scan(s => s.FromAssemblies(Assembly.GetAssembly(services.GetType()))
                                .AddClasses(c => c.Where(w => w.FullName.EndsWith("Function", StringComparison.CurrentCultureIgnoreCase)))
                                .AsSelf()
                                .WithTransientLifetime());
        }

        public static void AddMessageBus(this IServiceCollection services, Action<IBusConfiguration> configuration)
        {
            var configurationInstance = new BusConfiguration();
            configuration(configurationInstance);

            var bus = new MessageBusContainer(configurationInstance);
            services.AddSingleton(bus);
        }

        public static void UseMicroless(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetService<FunctionsManager>().ScaffoldFunctions();
            }
        }
    }
}
