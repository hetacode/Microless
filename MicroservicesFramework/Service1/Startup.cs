using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hetacode.Microless.Extensions;
using Hetacode.Microless.RabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Service1
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMicroless();
            services.AddMessageBus(config =>
            {
                config.Provider = new RabbitMQProvider("192.168.8.140", "guest", "guest", "saga");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseMicroless();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
            app.UseMessageBus((functions, subscribe) =>
            {
                subscribe.AddReceiver("Service1", async (queueName, message, headers) =>
                {
                    await functions.CallFunction(queueName, message, headers);
                });
            });
        }
    }
}
