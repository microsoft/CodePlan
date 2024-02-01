using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.QnA.Api.Client
{
    public static class QnaApiClientExtensions
    {
        public static void AddQnaApiClient(this IServiceCollection services, QnaApiConfig config, bool disableBearerHeader = false)
        {
            config.DisableBearerHeader = disableBearerHeader;
            services.AddTransient<QnaApiConfig>(provider => config);
            services.AddHttpClient<QnaApiClient>(client => client.BaseAddress = config.BaseUri);
        }
    }
}