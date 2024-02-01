using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Commands;
using System.IO;

namespace SFA.DAS.QnA.Application.UnitTests.Validators.FileContentValidatorTests
{
    [TestFixture]
    public class When_Validate_Called
    {
        [TestCase("test.jpg", new byte[] { 0xFF, 0xD8, 0xFF }, true)]
        [TestCase("test.JPG", new byte[] { 0xFF, 0xD8, 0xFF }, true)]
        [TestCase("test.JPg", new byte[] { 0xFF, 0xD8, 0xFF }, true)]
        [TestCase("test.jPG", new byte[] { 0xFF, 0xD8, 0xFF }, true)]
        [TestCase("test.JpG", new byte[] { 0xFF, 0xD8, 0xFF }, true)]
        [TestCase("test.jPg", new byte[] { 0xFF, 0xD8, 0xFF }, true)]
        [TestCase("test.jpg.jpg", new byte[] { 0xFF, 0xD8, 0xFF }, true)]
        [TestCase("test.pdf.jpg", new byte[] { 0x25, 0x50, 0x44, 0x46 }, false)]
        [TestCase("pdf-file.jpg", new byte[] { 0x25, 0x50, 0x44, 0x46 }, false)]
        [TestCase("corrupted-file.jpg", new byte[] { 0x0A, 0xD9, 0xEE }, false)]
        public void Then_correct_errors_are_returned(string filename, byte[] fileContent, bool isValid)
        {
            var file = new FormFile(new MemoryStream(fileContent), 0, fileContent.Length, "Q1", filename);
            var formFileCollection = new FormFileCollection { file };

            var validator = new FileContentValidator();
            var errors = validator.Validate(formFileCollection);

            (errors.Count is 0).Should().Be(isValid);
        }
    }
}
