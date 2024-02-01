using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace SFA.DAS.QnA.Application.Commands
{
    public class FileContentValidator : IFileContentValidator
    {
        private const string _errorMessage = "The content for this file does not match its contents";

        // Refer to this: https://www.garykessler.net/library/file_sigs.html
        private static readonly Dictionary<string, byte[]> _knownFileSignatures = new Dictionary<string, byte[]>
        {
            {"JPG",  new byte[] { 0xFF, 0xD8, 0xFF }},
            {"JPEG", new byte[] { 0xFF, 0xD8, 0xFF }},
            {"TIF",  new byte[] { 0x49, 0x49, 0x2A, 0x00 }},
            {"TIFF", new byte[] { 0x49, 0x49, 0x2A, 0x00 }},
            {"GIF",  new byte[] { 0x47, 0x49, 0x46, 0x38 }},
            {"BMP",  new byte[] { 0x42, 0x4D }},
            {"PNG",  new byte[] { 0x89, 0x50, 0x4E, 0x47 }},
            {"ICO",  new byte[] { 0x00, 0x00, 0x01, 0x00 }},

            {"PDF",  new byte[] { 0x25, 0x50, 0x44, 0x46 }},

            {"DOC",  new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }},
            {"XLS",  new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }},
            {"PPT",  new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }},
            {"MSG",  new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }},

            {"DOCX",  new byte[] { 0x50, 0x4B, 0x03, 0x04 }},
            {"XLSX",  new byte[] { 0x50, 0x4B, 0x03, 0x04 }},
            {"PPTX",  new byte[] { 0x50, 0x4B, 0x03, 0x04 }}
        };

        public List<KeyValuePair<string, string>> Validate(IFormFileCollection files)
        {
            var validationErrors = new List<KeyValuePair<string, string>>();

            if (files != null)
            {
                foreach (var file in files)
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var fileExtension = Path.GetExtension(file.FileName).TrimStart('.');

                        if (!FileContentIsValidForFileExtension(fileExtension, stream))
                        {
                            validationErrors.Add(new KeyValuePair<string, string>(file.Name, _errorMessage));
                        }
                    }
                }
            }

            return validationErrors;
        }


        private bool FileContentIsValidForFileExtension(string fileExtension, Stream fileContents)
        {
            var isValid = false;

            if (fileExtension != null && fileContents != null)
            {
                if (_knownFileSignatures.TryGetValue(fileExtension.ToUpperInvariant(), out var headerForFileExtension))
                {
                    var headerOfActualFile = new byte[headerForFileExtension.Length];
                    fileContents.Read(headerOfActualFile, 0, headerOfActualFile.Length);
                    fileContents.Position = 0;

                    isValid = headerOfActualFile.SequenceEqual(headerForFileExtension);
                }
                else
                {
                    isValid = true;
                }
            }

            return isValid;
        }
    }
}