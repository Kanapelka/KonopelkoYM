using Athena.Infrastructure;

namespace Athena.Core.Services
{
    public abstract class BaseService
    {
        private readonly IInfrastructureOptionsProvider _optionsProvider;
        public Context Context => new Context(_optionsProvider);


        protected BaseService(IInfrastructureOptionsProvider optionsProvider)
        {
            _optionsProvider = optionsProvider;
        }
    }
}