using System;
using System.Reflection;
using Hetacode.Microless.Abstractions.Filters;
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
            services.AddSingleton<IFiltersManager>(o =>
            {
                var serviceProvider = o.GetService<IServiceProvider>();
                return new FiltersManager(serviceProvider, services);
            });

            services.AddSingleton<BusConfiguration>();
            services.AddSingleton<IBusConfiguration>(c => c.GetService<BusConfiguration>());
            services.AddSingleton<IBusSubscriptions>(c => c.GetService<BusConfiguration>());

            services.AddSingleton(o =>
            {
                using (var scope = o.CreateScope())
                {
                    var config = scope.ServiceProvider.GetService<IBusConfiguration>();
                    configuration(config);
                    var sub = scope.ServiceProvider.GetService<IBusSubscriptions>();
                    return new MessageBusContainer(sub);
                }
            });
        }

        public static void UseMicroless(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetService<FunctionsManager>().ScaffoldFunctions();
            }
        }

        public static void UseMessageBus(this IApplicationBuilder app, Action<IBusSubscriptions> subscribe)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetService<MessageBusContainer>();
                var busConfig = scope.ServiceProvider.GetService<IBusSubscriptions>();
                subscribe(busConfig);
            }
        }
    }
}
