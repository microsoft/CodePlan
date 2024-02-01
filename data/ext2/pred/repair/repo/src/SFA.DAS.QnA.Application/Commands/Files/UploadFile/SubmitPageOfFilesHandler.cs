using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.SetPageAnswers;
using SFA.DAS.QnA.Application.Services;
using SFA.DAS.QnA.Configuration.Config;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Commands.Files.UploadFile

{
    public class SubmitPageOfFilesHandler : SetAnswersBase, IRequestHandler<SubmitPageOfFilesRequest, HandlerResponse<SetPageAnswersResponse>>
    {
        private readonly IOptions<FileStorageConfig> _fileStorageConfig;
        private readonly IEncryptionService _encryptionService;
        private readonly IFileContentValidator _fileContentValidator;

        public SubmitPageOfFilesHandler(QnaDataContext dataContext, IOptions<FileStorageConfig> fileStorageConfig, IEncryptionService encryptionService, IAnswerValidator answerValidator, IFileContentValidator fileContentValidator, INotRequiredProcessor notRequiredProcessor, ITagProcessingService tagProcessingService) : base(dataContext, notRequiredProcessor, tagProcessingService, answerValidator)
        {
            _fileStorageConfig = fileStorageConfig;
            _encryptionService = encryptionService;
            _fileContentValidator = fileContentValidator;
        }

        public async Task<HandlerResponse<SetPageAnswersResponse>> Handle(SubmitPageOfFilesRequest request, CancellationToken cancellationToken)
        {
            var section = await _dataContext.ApplicationSections.SingleOrDefaultAsync(sec => sec.Id == request.SectionId && sec.ApplicationId == request.ApplicationId);
            var validationErrorResponse = ValidateRequest(request, section);

            if (validationErrorResponse != null)
            {
                return validationErrorResponse;
            }

            await SaveAnswersIntoPage(section, request, cancellationToken);
            var application = await _dataContext.Applications.SingleOrDefaultAsync(app => app.Id == request.ApplicationId);
            UpdateApplicationData(request, application, section);

            var nextAction = GetNextActionForPage(section, application, request.PageId);
            var checkboxListAllNexts = GetCheckboxListMatchingNextActionsForPage(section, application, request.PageId);

            SetStatusOfNextPagesBasedOnDeemedNextActions(section, request.PageId, nextAction, checkboxListAllNexts);

            await _dataContext.SaveChangesAsync(cancellationToken);

            return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(nextAction.Action, nextAction.ReturnId));
        }

        private HandlerResponse<SetPageAnswersResponse> ValidateRequest(SubmitPageOfFilesRequest request, ApplicationSection section)
        {
            var page = section?.QnAData?.Pages.SingleOrDefault(p => p.PageId == request.PageId);

            if (page is null)
            {
                return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "Cannot find requested page.");
            }
            else if (request.Files is null)
            {
                return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "No files specified.");
            }
            else if (request.Files.Any(f => f.Name is null))
            {
                return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "All files must specify which question they are related to.");
            }
            else if (page.AllowMultipleAnswers)
            {
                return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "This endpoint cannot be used for Multiple Answers pages. Use AddAnswer / RemoveAnswer instead.");
            }
            else if (page.Questions.Any())
            {
                if (page.Questions.Any(q => !"FileUpload".Equals(q.Input?.Type, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "Pages cannot contain a mixture of FileUploads and other Question Types.");
                }

                var answersToValidate = GetAnswersToValidate(request, page);

                var validationErrors = _answerValidator.Validate(answersToValidate, page);
                if (validationErrors.Any())
                {
                    return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(validationErrors));
                }

                var fileContentValidationErrors = _fileContentValidator.Validate(request.Files);
                if (fileContentValidationErrors.Any())
                {
                    return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(fileContentValidationErrors));
                }
            }

            return null;
        }

        private async Task SaveAnswersIntoPage(ApplicationSection section, SubmitPageOfFilesRequest request, CancellationToken cancellationToken)
        {
            if (section != null)
            {
                // Have to force QnAData a new object and reassign for Entity Framework to pick up changes
                var qnaData = new QnAData(section.QnAData);
                var page = qnaData?.Pages.SingleOrDefault(p => p.PageId == request.PageId);

                if (page != null)
                {
                    var container = await ContainerHelpers.GetContainer(_fileStorageConfig.Value.StorageConnectionString, _fileStorageConfig.Value.ContainerName);

                    foreach (var file in request.Files)
                    {
                        var questionIdFromFileName = file.Name;
                        var questionFolder = ContainerHelpers.GetDirectory(request.ApplicationId, section.SequenceId, request.SectionId, request.PageId, questionIdFromFileName, container);

                        var blob = questionFolder.GetBlockBlobReference(file.FileName);
                        blob.Properties.ContentType = file.ContentType;

                        var encryptedFileStream = _encryptionService.Encrypt(file.OpenReadStream());

                        await blob.UploadFromStreamAsync(encryptedFileStream, cancellationToken);

                        if (page.PageOfAnswers is null)
                        {
                            page.PageOfAnswers = new List<PageOfAnswers>();
                        }

                        var foundExistingOnPage = page.PageOfAnswers.SelectMany(a => a.Answers).Any(answer => answer.QuestionId == file.Name && answer.Value == file.FileName);

                        if (!foundExistingOnPage)
                        {
                            page.PageOfAnswers.Add(new PageOfAnswers
                            {
                                Id = Guid.NewGuid(),
                                Answers = new List<Answer>
                                {
                                    new Answer
                                    {
                                        QuestionId = file.Name,
                                        Value = file.FileName
                                    }
                                }
                            });
                        }
                    }

                    MarkPageAsComplete(page);
                    MarkPageFeedbackAsComplete(page);

                    // Assign QnAData back so Entity Framework will pick up changes
                    section.QnAData = qnaData;
                }
            }
        }

        private void UpdateApplicationData(SubmitPageOfFilesRequest request, Data.Entities.Application application, ApplicationSection section)
        {
            if (application != null)
            {
                var applicationData = JObject.Parse(application.ApplicationData ?? "{}");

                var page = section?.QnAData?.Pages.SingleOrDefault(p => p.PageId == request.PageId);

                if (page != null)
                {
                    var questionTagsWhichHaveBeenUpdated = new List<string>();
                    var answers = GetAnswersFromRequest(request);

                    foreach (var question in page.Questions)
                    {
                        SetApplicationDataField(question, answers, applicationData);
                        if (!string.IsNullOrWhiteSpace(question.QuestionTag))
                            questionTagsWhichHaveBeenUpdated.Add(question.QuestionTag);

                        if (question.Input.Options != null)
                        {
                            foreach (var option in question.Input.Options.Where(o => o.FurtherQuestions != null))
                            {
                                foreach (var furtherQuestion in option.FurtherQuestions)
                                {
                                    SetApplicationDataField(furtherQuestion, answers, applicationData);
                                    if (!string.IsNullOrWhiteSpace(furtherQuestion.QuestionTag))
                                        questionTagsWhichHaveBeenUpdated.Add(furtherQuestion.QuestionTag);
                                }
                            }
                        }
                    }

                    application.ApplicationData = applicationData.ToString(Formatting.None);

                    SetStatusOfAllPagesBasedOnUpdatedQuestionTags(application, questionTagsWhichHaveBeenUpdated);
                    _tagProcessingService.ClearDeactivatedTags(application.Id, request.SectionId);
                }
            }
        }

        private static List<Answer> GetAnswersFromRequest(SubmitPageOfFilesRequest request)
        {
            var answers = new List<Answer>();

            if (request.Files != null)
            {
                foreach (var file in request.Files)
                {
                    var answer = new Answer { QuestionId = file.Name, Value = file.FileName };
                    answers.Add(answer);
                }
            }

            return answers;
        }

        private static List<Answer> GetExistingAnswersFromPage(Page page)
        {
            var answers = new List<Answer>();

            if (page.PageOfAnswers != null)
            {
                foreach (var pageOfAnswers in page.PageOfAnswers)
                {
                    foreach (var pageAnswer in pageOfAnswers.Answers)
                    {
                        if (answers.All(a => a.QuestionId != pageAnswer.QuestionId))
                        {
                            answers.Add(pageAnswer);
                        }
                    }
                }
            }

            return answers;
        }

        private static List<Answer> GetAnswersToValidate(SubmitPageOfFilesRequest request, Page page)
        {
            var answers = GetAnswersFromRequest(request);

            foreach (var existingAnswer in GetExistingAnswersFromPage(page))
            {
                if (answers.All(a => a.QuestionId != existingAnswer.QuestionId))
                {
                    answers.Add(existingAnswer);
                }
            }

            return answers;
        }
    }

}