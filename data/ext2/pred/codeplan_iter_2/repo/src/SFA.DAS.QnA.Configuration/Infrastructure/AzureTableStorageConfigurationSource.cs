using Microsoft.Extensions.Configuration;

namespace SFA.DAS.QnA.Configuration.Infrastructure
{
    public class AzureTableStorageConfigurationSource : IConfigurationSource
    {
        private readonly string _connection;
        private readonly string _environment;
        private readonly string _version;
        private readonly string _appName;

        public AzureTableStorageConfigurationSource(string connection, string appName, string environment, string version)
        {
            _appName = appName;
            _connection = connection;
            _environment = environment;
            _version = version;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AzureTableStorageConfigurationProvider(_connection, _appName, _environment, _version);
        }
    }
}
