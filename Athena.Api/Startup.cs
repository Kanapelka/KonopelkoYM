using Athena.Api.Services;
using Athena.Api.Services.Settings;
using Athena.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Athena.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            AthenaServicesConfiguration athenaConfiguration = new AthenaServicesConfiguration(Configuration);

            services.AddSingleton<IInfrastructureSettings, InfrastructureSettings>();
            Core.Core.Register(services);

            services.AddSingleton<JwtSettings>();
            services.AddSingleton<JwtService>();
            services.AddAuthentication(athenaConfiguration.ConfigureAuthentication)
                    .AddJwtBearer(athenaConfiguration.ConfigureJwtBearer);

            services.AddHttpContextAccessor();

            services.AddControllers();

            services.AddCors(athenaConfiguration.ConfigureCorsPolicy);

            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "react-app"; });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "react-app";
            });
        }
    }
}