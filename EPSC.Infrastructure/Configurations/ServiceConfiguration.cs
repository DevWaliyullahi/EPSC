using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using EPSC.Infrastructure.Configurations.Data;
using EPSC.Infrastructure.Identity.Auth;
using EPSC.Utility.Configurations;


namespace EPSC.Infrastructure.Configurations
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection InitService(this IServiceCollection services, IConfiguration configuration)
        {
            // Identity Core setup
            services.AddIdentity<EPSAuthUser, EPSAuthRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<EPSCDbContext>()
            .AddDefaultTokenProviders();

            // JWT Authentication
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Key))
            {
                throw new ArgumentNullException(nameof(jwtSettings), "JwtSettings or SecretKey cannot be null.");
            }

            var key = Encoding.ASCII.GetBytes(jwtSettings.Key);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            services.AddAuthorization();

            return services;
        }
    }
}
