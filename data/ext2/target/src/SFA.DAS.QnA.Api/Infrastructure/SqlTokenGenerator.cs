using Microsoft.Azure.Services.AppAuthentication;
using System.Threading.Tasks;

namespace SFA.DAS.QnA.Api.Infrastructure
{
    public static class SqlTokenGenerator
    {
        private const string AzureResource = "https://database.windows.net/";

        public static async Task<string> GenerateTokenAsync()
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var accessToken = await azureServiceTokenProvider.GetAccessTokenAsync(AzureResource);

            return accessToken;
        }
    }
}
