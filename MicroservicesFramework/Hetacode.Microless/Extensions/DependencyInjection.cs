using System;
using System.Reflection;
using Hetacode.Microless.Managers;
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

        public static void UseMicroless(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetService<FunctionsManager>().ScaffoldFunctions();
            }
        }
    }
}
