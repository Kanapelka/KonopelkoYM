using Athena.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Athena.Tests.Mocks
{
    public class InMemoryContextOptionsProvider : IInfrastructureOptionsProvider
    {
        public DbContextOptions<Context> Options => new DbContextOptionsBuilder<Context>()
            .UseInMemoryDatabase(databaseName: "athena")
            .Options;
    }
}