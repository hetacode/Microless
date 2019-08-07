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
            var filters = new FiltersManager(services);
            var config = new BusConfiguration(filters);
            configuration(config);

            services.AddSingleton<IBusConfiguration>(config);
            services.AddSingleton<IBusSubscriptions>(config);

            //services.AddSingleton<IFiltersManager, FiltersManager>();
            //services.AddSingleton<IBusConfiguration, BusConfiguration>();
            services.AddSingleton<MessageBusContainer>();
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
                var bus = scope.ServiceProvider.GetService<IBusSubscriptions>();
                subscribe(bus);
            }
        }
    }
}
