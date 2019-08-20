using System;
using System.Linq;
using System.Reflection;
using Hetacode.Microless.Abstractions.Filters;
using Hetacode.Microless.Abstractions.Managers;
using Hetacode.Microless.Abstractions.MessageBus;
using Hetacode.Microless.Abstractions.StateMachine;
using Hetacode.Microless.Attributes;
using Hetacode.Microless.Managers;
using Hetacode.Microless.MessageBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Saga.StateMachine;

namespace Hetacode.Microless.Extensions
{
    public static class DependencyInjection
    {
        public static void AddMicroless(this IServiceCollection services)
        {
            services.AddSingleton<IFunctionsManager, FunctionsManager>();
            services.AddSingleton<IStepsManager, StepsManager>();
            services.AddSingleton<IAggregatorBuilder, AggregatorBuilder>();

            // Register all Functions
            services.Scan(s => s.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                                .AddClasses(c => c.Where(w => w.FullName.EndsWith("Function", StringComparison.CurrentCultureIgnoreCase)))
                                .AsSelf()
                                .WithTransientLifetime());
            // Register all Aggregators
            services.Scan(s => s.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                                .AddClasses(c => c.Where(w => w.GetCustomAttributes().Any(i => i.GetType() == typeof(AggregatorAttribute))))
                                .AsSelfWithInterfaces()
                                .WithSingletonLifetime());
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
                scope.ServiceProvider.GetService<IFunctionsManager>().ScaffoldFunctions();

                // Initialize all aggregators
                AppDomain.CurrentDomain.GetAssemblies().ToList().ForEach(a =>
                {
                    var types = a.GetTypes().Where(w => w.GetCustomAttributes().Any(i => i.GetType() == typeof(AggregatorAttribute))).ToList();
                    types.ForEach(f =>
                    {
                        scope.ServiceProvider.GetService(f);
                    });
                });
            }
        }

        public static void UseMessageBusFunctions(this IApplicationBuilder app, Action<IFunctionsManager, IBusSubscriptions> subscribe)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetService<MessageBusContainer>();
                var functionsManager = scope.ServiceProvider.GetService<IFunctionsManager>();
                var busConfig = scope.ServiceProvider.GetService<IBusSubscriptions>();
                subscribe(functionsManager, busConfig);
            }
        }

        public static void UseMessageBusAggregators(this IApplicationBuilder app, Action<IStepsManager, IBusSubscriptions> subscribe)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetService<MessageBusContainer>();
                var stepsManager = scope.ServiceProvider.GetService<IStepsManager>();
                var busConfig = scope.ServiceProvider.GetService<IBusSubscriptions>();
                subscribe(stepsManager, busConfig);
            }
        }
    }
}
