using Microsoft.Extensions.Configuration;

namespace Athena.Api.Services.Settings
{
    public class CorsSettings
    {
        private IConfiguration _configuration;


        public CorsSettings(IConfiguration configuration)
        {
            _configuration = configuration.GetSection(nameof(CorsSettings));
        }


        public string[] AllowedOrigins =>  _configuration.GetSection(nameof(AllowedOrigins)).Get<string[]>();
    }
}