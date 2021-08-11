using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyLab.HttpMetrics;
using MyLab.Search.Delegate.Services;
using MyLab.Search.EsAdapter;
using MyLab.StatusProvider;
using MyLab.WebErrors;

namespace MyLab.Search.Delegate
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAppStatusProviding(Configuration as IConfigurationRoot)
                .AddUrlBasedHttpMetrics()
                .AddLogging(l => l.AddConsole())
                .AddSingleton<IEsRequestProcessor, EsRequestProcessor>()
                .AddSingleton<IEsRequestBuilder, EsRequestBuilder>()
                .AddSingleton<IEsFilterProvider, EsFilterProvider>()
                .AddSingleton<IEsSortProvider, EsSortProvider>()
                .AddSingleton<IIndexMappingService, IndexMappingService>()
                .AddSingleton<ITokenService, TokenService>()
                .AddEsTools(Configuration)
                .Configure<DelegateOptions>(Configuration.GetSection("Delegate"))
                .AddControllers(o => o.AddExceptionProcessing());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting()
                .UseUrlBasedHttpMetrics()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                })
                .UseStatusApi(); ;
        }
    }
}
