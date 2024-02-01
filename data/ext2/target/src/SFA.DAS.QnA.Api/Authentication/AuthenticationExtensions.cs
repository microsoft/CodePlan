using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.QnA.Configuration.Config;

namespace SFA.DAS.QnA.Api.Authentication
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddApiAuthentication(this IServiceCollection services, ServiceProvider serviceProvider)
        {
            var azureActiveDirectoryConfiguration = serviceProvider.GetService<IOptions<AzureActiveDirectoryConfiguration>>().Value;

            services.AddAuthentication(auth =>
            {
                auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(auth =>
            {
                auth.Authority = $"https://login.microsoftonline.com/{azureActiveDirectoryConfiguration.Tenant}";
                auth.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidAudiences = azureActiveDirectoryConfiguration.Identifier.Split(",")
                };
            });

            services.AddSingleton<IClaimsTransformation, AzureAdScopeClaimTransformation>();
            return services;
        }
    }
}
