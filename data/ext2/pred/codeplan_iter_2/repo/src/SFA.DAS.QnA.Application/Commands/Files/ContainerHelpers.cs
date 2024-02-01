using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace SFA.DAS.QnA.Application.Commands.Files
{
    public static class ContainerHelpers
    {
        public static async Task<CloudBlobContainer> GetContainer(string connectionString, string containerName)
        {
            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudBlobClient();
            var container = client.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();
            return container;
        }

        public static CloudBlobDirectory GetDirectory(Guid applicationId, Guid sequenceId, Guid sectionId, string pageId, string questionId, CloudBlobContainer container)
        {
            var applicationFolder = container.GetDirectoryReference(applicationId.ToString());
            var sequenceFolder = applicationFolder.GetDirectoryReference(sequenceId.ToString());
            var sectionFolder = sequenceFolder.GetDirectoryReference(sectionId.ToString());
            var pageFolder = sectionFolder.GetDirectoryReference(pageId.ToLower());
            if (questionId is null)
            {
                return pageFolder;
            }
            var questionFolder = pageFolder.GetDirectoryReference(questionId.ToLower());
            return questionFolder;
        }
    }
}