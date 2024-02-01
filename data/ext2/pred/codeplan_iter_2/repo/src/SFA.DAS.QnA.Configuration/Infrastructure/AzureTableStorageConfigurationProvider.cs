using System;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace SFA.DAS.QnA.Configuration.Infrastructure
{
    public class AzureTableStorageConfigurationProvider : ConfigurationProvider
    {
        private readonly string _connection;
        private readonly string _environment;
        private readonly string _version;
        private readonly string _appName;


        public AzureTableStorageConfigurationProvider(string connection, string appName, string environment, string version)
        {
            _connection = connection;
            _environment = environment;
            _version = version;
            _appName = appName;
        }

        public override void Load()
        {
            if (_environment.Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            var table = GetTable();
            var operation = GetOperation(_appName, _environment, _version);

            var result = table.ExecuteAsync(operation).Result;

            var configItem = (ConfigurationItem)result.Result;

            var jsonObject = JObject.Parse(configItem.Data);

            foreach (var child in jsonObject.Children())
            {
                foreach (var jToken in child.Children().Children())
                {
                    var child1 = (JProperty)jToken;
                    Data.Add($"{child.Path}:{child1.Name}", child1.Value.ToString());
                }
            }
        }

        private CloudTable GetTable()
        {
            var storageAccount = CloudStorageAccount.Parse(_connection);
            var tableClient = storageAccount.CreateCloudTableClient();
            return tableClient.GetTableReference("Configuration");
        }

        private TableOperation GetOperation(string serviceName, string environmentName, string version)
        {
            return TableOperation.Retrieve<ConfigurationItem>(environmentName, $"{serviceName}_{version}");
        }
    }
}
