using Athena.Core.Models;
using Athena.Core.Services;
using Athena.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Athena.Core
{
    public class Core
    {
        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<IInfrastructureOptionsProvider, SqlServerInfrastructureConfigurationProvider>();
            services.AddSingleton<AuthorizationService>();
            services.AddScoped<UserService>();
            services.AddScoped<ProjectService>();
            services.AddScoped<UserInfo>();
            services.AddScoped<MemberService>();
            services.AddScoped<TicketStatusService>();
            services.AddScoped<TicketService>();
            services.AddScoped<TaskSerivce>();
            services.AddScoped<DashboardService>();
            services.AddScoped<NotificationService>();
        }
    }
}