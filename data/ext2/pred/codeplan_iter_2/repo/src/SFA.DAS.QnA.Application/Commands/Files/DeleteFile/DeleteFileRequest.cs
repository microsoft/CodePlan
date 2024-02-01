using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Configuration.Config;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.Commands.Files.DeleteFile

{
    public class DeleteFileRequest : IRequest<HandlerResponse<bool>>
    {
        public Guid ApplicationId { get; }
        public Guid SectionId { get; }
        public string PageId { get; }
        public string QuestionId { get; }
        public string FileName { get; }

        public DeleteFileRequest(Guid applicationId, Guid sectionId, string pageId, string questionId, string fileName)
        {
            ApplicationId = applicationId;
            SectionId = sectionId;
            PageId = pageId;
            QuestionId = questionId;
            FileName = fileName;
        }
    }

    public class DeleteFileHandler : IRequestHandler<DeleteFileRequest, HandlerResponse<bool>>

    {
        private readonly QnaDataContext _dataContext;
        private readonly IOptions<FileStorageConfig> _fileStorageConfig;
        private readonly IAnswerValidator _answerValidator;

        public DeleteFileHandler(QnaDataContext dataContext, IAnswerValidator answerValidator, IOptions<FileStorageConfig> fileStorageConfig)
        {
            _dataContext = dataContext;
            _fileStorageConfig = fileStorageConfig;
            _answerValidator = answerValidator;
        }

        public async Task<HandlerResponse<bool>> Handle(DeleteFileRequest request, CancellationToken cancellationToken)
        {

            var section = await _dataContext.ApplicationSections.FirstOrDefaultAsync(sec => sec.Id == request.SectionId && sec.ApplicationId == request.ApplicationId, cancellationToken);

            if (section == null)
            {
                return new HandlerResponse<bool>(success: false, message: $"Section {request.SectionId} in Application {request.ApplicationId} does not exist.");
            }

            var qnaData = new QnAData(section.QnAData);
            var page = qnaData.Pages.FirstOrDefault(p => p.PageId == request.PageId);

            if (page == null)
            {
                return new HandlerResponse<bool>(success: false, message: $"Page {request.PageId} in Application {request.ApplicationId} does not exist.");
            }

            if (page.Questions.All(q => q.Input.Type != "FileUpload"))
            {
                return new HandlerResponse<bool>(success: false, message: $"Page {request.PageId} in Application {request.ApplicationId} does not contain any File Upload questions.");
            }

            if (page.PageOfAnswers == null || !page.PageOfAnswers.Any())
            {
                return new HandlerResponse<bool>(success: false, message: $"Page {request.PageId} in Application {request.ApplicationId} does not contain any uploads.");
            }

            var container = await ContainerHelpers.GetContainer(_fileStorageConfig.Value.StorageConnectionString, _fileStorageConfig.Value.ContainerName);
            var directory = ContainerHelpers.GetDirectory(request.ApplicationId, section.SequenceId, request.SectionId, request.PageId, request.QuestionId, container);

            var answer = page.PageOfAnswers.SingleOrDefault(poa => poa.Answers.Any(a => a.QuestionId == request.QuestionId && a.Value == request.FileName));
            if (answer is null)
            {
                return new HandlerResponse<bool>(success: false, message: $"Question {request.QuestionId} on Page {request.PageId} in Application {request.ApplicationId} does not contain an upload named {request.FileName}.");
            }

            page.PageOfAnswers.Remove(answer);

            if (page.PageOfAnswers.Count == 0)
            {
                page.Complete = false;
                if (page.HasFeedback)
                {
                    foreach (var feedback in page.Feedback.Where(feedback => feedback.IsNew).Select(feedback => feedback))
                    {
                        feedback.IsCompleted = false;
                    }
                }
            }
            else
            {
                var answers = page.PageOfAnswers?.SelectMany(p => p.Answers);
                if (answers != null)
                {
                    var validationErrors = _answerValidator.Validate(answers.ToList(), page);
                    if (validationErrors != null && validationErrors.Any())
                        page.Complete = false;
                }
            }

            section.QnAData = qnaData;
            await _dataContext.SaveChangesAsync(cancellationToken);

            var blobRef = directory.GetBlobReference(request.FileName);
            await blobRef.DeleteAsync(cancellationToken);

            await RemoveApplicationDataForThisQuestion(request.ApplicationId, request.QuestionId, page);

            return new HandlerResponse<bool>(true);
        }

        private async Task RemoveApplicationDataForThisQuestion(Guid applicationId, string questionId, Page page)
        {
            var application = await _dataContext.Applications.SingleOrDefaultAsync(app => app.Id == applicationId);
            var applicationData = JsonNode.Parse(application.ApplicationData);

            var question = page.Questions.Single(q => q.QuestionId == questionId);

            applicationData.AsObject().Remove(question.QuestionTag);

            application.ApplicationData = applicationData.ToString();

            await _dataContext.SaveChangesAsync();
        }

    }

}