using Microsoft.Extensions.Configuration;

namespace Athena.Api.Services
{
    public class JwtSettings
    {
        private readonly IConfiguration _configuration;

        public string Secret => _configuration[nameof(Secret)];
        public string Issuer => _configuration[nameof(Issuer)];


        public JwtSettings(IConfiguration configuration)
        {
            _configuration = configuration.GetSection(nameof(JwtSettings));
        }
    }
}