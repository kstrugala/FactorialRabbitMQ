using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Factorial.Api.Handlers;
using Factorial.Api.Repository;
using Factorial.Messages.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RawRabbit;
using RawRabbit.Configuration;
using RawRabbit.Instantiation;

namespace Factorial.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton<IRepository>(_=> new InMemoryRepository());

            ConfigureRabbitMq(services, Configuration.GetSection("rabbitmq"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseMvc();
        
            ConfigureRabbitMqSubscriptions(app);
        }

        private void ConfigureRabbitMqSubscriptions(IApplicationBuilder app)
        {
            IBusClient client = app.ApplicationServices.GetService<IBusClient>();
            
            var handler = app.ApplicationServices.GetService<IEventHandler<FactorialCalculated>>();

            client.SubscribeAsync<FactorialCalculated>(
                        msg => handler.HandleAsync(msg), 
                        ctx=>ctx.UseSubscribeConfiguration(
                            cfg=>cfg.FromDeclaredQueue(
                                q=>q.WithName(
                                    GetExchangeName<FactorialCalculated>()
                                    ))));


        } 

         private void ConfigureRabbitMq(IServiceCollection services, IConfigurationSection section)
        {
            var options = new RawRabbitConfiguration();
            section.Bind(options);

            var client = RawRabbitFactory.CreateSingleton(new RawRabbitOptions
            {
                ClientConfiguration = options
            });

            services.AddSingleton<IBusClient>(_=>client);

            services.AddTransient<IEventHandler<FactorialCalculated>, FactorialCalculatedHandler>();
        }

        private static string GetExchangeName<T>(string name = null)
            => string.IsNullOrWhiteSpace(name)
                ? $"{Assembly.GetEntryAssembly().GetName()}/{typeof(T).Name}"
                : $"{name}/{typeof(T).Name}";
    }
}
