using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RawRabbit;
using RawRabbit.vNext;
using RawRabbit.Configuration;
using RawRabbit.Instantiation;
using RawRabbit.Pipe;
using System.Reflection;
using Factorial.Messages.Commands;
using Factorial.Service.Handlers;
using Factorial.Service.Framework;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace Factorial.Service
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

            var serilogOptions = new SerilogOptions();
            Configuration.GetSection("serilog").Bind(serilogOptions);
            services.AddSingleton<SerilogOptions>(serilogOptions);

            ConfigureRabbitMq(services, Configuration.GetSection("rabbitmq"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
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


            loggerFactory.AddSerilog();
            var serilogOptions = app.ApplicationServices.GetService<SerilogOptions>();
            var level = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), serilogOptions.Level, true);

            Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .MinimumLevel.Is(level)
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(serilogOptions.ApiUrl))
                        {
                            MinimumLogEventLevel = level,
                            AutoRegisterTemplate = true,
                            IndexFormat = string.IsNullOrWhiteSpace(serilogOptions.IndexFormat) ? 
                                "logstash-{0:yyyy.MM.dd}" : serilogOptions.IndexFormat,
                            ModifyConnectionSettings = x => serilogOptions.UseBasicAuth ? 
                            x.BasicAuthentication(serilogOptions.Username, serilogOptions.Password) : x
                        }
                    )
                    .CreateLogger();    

            app.UseMvc();

            ConfigureRabbitMqSubscriptions(app);
        }

        private void ConfigureRabbitMqSubscriptions(IApplicationBuilder app)
        {
            IBusClient client = app.ApplicationServices.GetService<IBusClient>();

            var handler = app.ApplicationServices.GetService<ICommandHandler<CalculateFactorial>>();

            client.SubscribeAsync<CalculateFactorial>(
                msg => handler.HandleAsync(msg),
                ctx => ctx.UseSubscribeConfiguration(
                    cfg => cfg.FromDeclaredQueue(
                        q => q.WithName(GetExchangeName<CalculateFactorial>())
                    )
                )
            );

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
            services.AddSingleton<IFactorialCalculator>(_=> new Factorial());
            services.AddTransient<ICommandHandler<CalculateFactorial>, CalculateFactorialHandler>();
        }

        private static string GetExchangeName<T>(string name = null)
            => string.IsNullOrWhiteSpace(name)
                ? $"{Assembly.GetEntryAssembly().GetName()}/{typeof(T).Name}"
                : $"{name}/{typeof(T).Name}";

    }
}
