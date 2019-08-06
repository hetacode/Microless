using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Service.Core;

namespace Service
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<FunctionsManager>();
            services.Scan(s => s.FromAssemblyOf<Startup>()
                                .AddClasses(c => c.Where(w => w.FullName.EndsWith("Function", StringComparison.CurrentCultureIgnoreCase)))
                                .AsSelf()
                                .WithTransientLifetime());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });

            var factory = new ConnectionFactory
            {
                HostName = "192.168.8.140",
                VirtualHost = "saga",
                UserName = "guest",
                Password = "guest"
            };
            var channel = factory.CreateConnection().CreateModel();
            var queueName = "Service";
            channel.QueueDeclare(queueName, false, false, true, null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;
            channel.BasicConsume(queueName, true, consumer);

            /// TESTTS
            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetService<FunctionsManager>().ScaffoldFunctions();
            }
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var jsonMessage = Encoding.UTF8.GetString(e.Body);
            var rawMessage = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonMessage);
            var type = rawMessage["_type"];
            var message = JsonConvert.DeserializeObject(jsonMessage, Type.GetType(type));
        }
    }
}
