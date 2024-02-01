using System.IO;

namespace SFA.DAS.QnA.Application.Commands.Files.DownloadFile
{
    public class DownloadFile
    {
        public string ContentType { get; set; }
        public Stream Stream { get; set; }
        public string FileName { get; set; }
    }
}