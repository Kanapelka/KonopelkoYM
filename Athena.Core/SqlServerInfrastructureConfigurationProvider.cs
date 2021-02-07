using Athena.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Athena.Core
{
    public class SqlServerInfrastructureConfigurationProvider : IInfrastructureOptionsProvider
    {
        private readonly IInfrastructureSettings _infrastructureSettings;

        public DbContextOptions<Context> Options => new DbContextOptionsBuilder<Context>()
            .UseSqlServer(_infrastructureSettings.ConnectionString)
            .UseSnakeCaseNamingConvention()
            .Options;


        public SqlServerInfrastructureConfigurationProvider(IInfrastructureSettings infrastructureSettings)
        {
            _infrastructureSettings = infrastructureSettings;
        }
    }
}