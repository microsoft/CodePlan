using Microsoft.Extensions.Options;
using SFA.DAS.QnA.Configuration.Config;

namespace SFA.DAS.QnA.Application.Commands.Files
{
    public class ConfigKeyProvider : IKeyProvider
    {
        private readonly IOptions<FileStorageConfig> _config;

        public ConfigKeyProvider(IOptions<FileStorageConfig> config)
        {
            _config = config;
        }

        public string GetKey()
        {
            var key = _config.Value.FileEncryptionKey;

            return key;
        }
    }
}