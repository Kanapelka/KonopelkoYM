using System.Text;
using Athena.Api.Services.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Athena.Api.Services
{
    public class AthenaServicesConfiguration
    {
        private readonly IConfiguration _configuration;
        private readonly CorsSettings _corsSettings;


        public AthenaServicesConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
            _corsSettings = new CorsSettings(configuration);
        }


        public void ConfigureAuthentication(AuthenticationOptions options)
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }

        public void ConfigureJwtBearer(JwtBearerOptions options)
        {
            JwtSettings jwtSettings = new JwtSettings(_configuration);

            byte[] secretKey = Encoding.UTF8.GetBytes(jwtSettings.Secret);

            options.RequireHttpsMetadata = false;
            options.SaveToken = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ValidateIssuer = false,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = false,
            };
        }

        public void ConfigureCorsPolicy(CorsOptions options)
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                builder.WithOrigins(_corsSettings.AllowedOrigins).AllowAnyHeader().AllowAnyMethod();
            });
        }
    }
}