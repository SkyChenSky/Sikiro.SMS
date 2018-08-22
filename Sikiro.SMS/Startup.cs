using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;
using NSwag.AspNetCore;
using Sikiro.Nosql.Mongo;
using Sikiro.SMS.Api.Helper;

namespace Sikiro.SMS.Api
{
    public class Startup
    {
        private readonly InfrastructureConfig _infrastructureConfig;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _infrastructureConfig = configuration.Get<InfrastructureConfig>();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(option =>
                {
                    option.Filters.Add<GolbalExceptionAttribute>();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.RegisterEasyNetQ(_infrastructureConfig.Infrastructure.RabbitMQ);
            services.AddSingleton(new MongoRepository(_infrastructureConfig.Infrastructure.Mongodb));
            services.AddService();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwaggerUiWithApiExplorer(settings =>
           {
               settings.GeneratorSettings.DefaultPropertyNameHandling =
                   PropertyNameHandling.CamelCase;
               settings.PostProcess = document =>
               {
                   document.Info.Version = "v1";
                   document.Info.Title = "QD.SMS.API";
                   document.Info.Description = "短信服务API";
                   document.Info.TermsOfService = "None";
               };
           });
            app.UseMvc();
        }
    }


    internal class InfrastructureConfig
    {
        public Infrastructure Infrastructure { get; set; }
    }

    public class Infrastructure
    {
        public string Mongodb { get; set; }
        public string RabbitMQ { get; set; }
    }
}