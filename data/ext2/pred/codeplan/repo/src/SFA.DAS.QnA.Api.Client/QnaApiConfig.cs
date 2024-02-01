using System;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace SFA.DAS.QnA.Api.Client
{
    public class QnaApiConfig
    {
        public string GetBearerToken()
        {
            if (DisableBearerHeader)
            {
                return "";
            }

            var authority = $"https://login.microsoftonline.com/{TenantId}";
            var clientCredential = new ClientCredential(ClientId, ClientSecret);
            var context = new AuthenticationContext(authority, true);
            var result = context.AcquireTokenAsync(ResourceId, clientCredential).Result;

            return result.AccessToken;
        }

        public string ResourceId { get; set; }

        public string ClientSecret { get; set; }

        public string ClientId { get; set; }

        public string TenantId { get; set; }

        public Uri BaseUri { get; set; }

        public bool DisableBearerHeader { get; set; }
    }
}