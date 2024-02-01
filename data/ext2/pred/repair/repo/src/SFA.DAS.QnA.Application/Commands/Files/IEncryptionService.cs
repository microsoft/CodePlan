using System.IO;

namespace SFA.DAS.QnA.Application.Commands.Files
{
    public interface IEncryptionService
    {
        Stream Encrypt(Stream fileStream);
        Stream Decrypt(Stream encryptedFileStream);
    }
}