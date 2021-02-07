using Athena.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Athena.Api.Services.Settings
{
    public class InfrastructureSettings : IInfrastructureSettings
    {
        private readonly IConfiguration _configuration;


        public InfrastructureSettings(IConfiguration configuration)
        {
            _configuration = configuration.GetSection(nameof(InfrastructureSettings));
        }

        public string ConnectionString => _configuration["SqlServerConnectionString"];
    }
}