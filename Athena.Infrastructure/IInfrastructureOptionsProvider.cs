using Microsoft.EntityFrameworkCore;

namespace Athena.Infrastructure
{
    public interface IInfrastructureOptionsProvider
    {
        DbContextOptions<Context> Options { get; }
    }
}