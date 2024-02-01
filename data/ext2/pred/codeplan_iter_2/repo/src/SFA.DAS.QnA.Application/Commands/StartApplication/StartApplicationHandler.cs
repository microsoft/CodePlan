using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Commands.StartApplication
{
    public class StartApplicationHandler : IRequestHandler<StartApplicationRequest, HandlerResponse<StartApplicationResponse>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IApplicationDataValidator _applicationDataValidator;
        private readonly ILogger<StartApplicationHandler> _logger;
        private bool _applicationDataIsInvalid;

        public StartApplicationHandler(QnaDataContext dataContext, IApplicationDataValidator applicationDataValidator, ILogger<StartApplicationHandler> logger)
        {
            _dataContext = dataContext;
            _applicationDataValidator = applicationDataValidator;
            _logger = logger;
        }

        public async Task<HandlerResponse<StartApplicationResponse>> Handle(StartApplicationRequest request, CancellationToken cancellationToken)
        {
            var latestWorkflow = await _dataContext.Workflows.AsNoTracking()
                .SingleOrDefaultAsync(w => w.Type == request.WorkflowType && w.Status == "Live", cancellationToken);

            if (latestWorkflow is null)
            {
                _logger.LogError($"Workflow type {request.WorkflowType} does not exist");
                return new HandlerResponse<StartApplicationResponse>(false, $"Workflow Type does not exist.");
            }

            try
            {
                _applicationDataIsInvalid = !_applicationDataValidator.IsValid(latestWorkflow.ApplicationDataSchema, request.ApplicationData);
            }
            catch (JsonException)
            {
                _logger.LogError("Supplied ApplicationData is not valid JSON");
                return new HandlerResponse<StartApplicationResponse>(false, $"Supplied ApplicationData is not valid JSON.");
            }

            if (_applicationDataIsInvalid)
            {
                _logger.LogError("Supplied ApplicationData is not valid using Project's Schema");
                return new HandlerResponse<StartApplicationResponse>(false, $"Supplied ApplicationData is not valid using Project's Schema.");
            }

            var newApplication = await CreateNewApplication(request, latestWorkflow, cancellationToken, request.ApplicationData);

            if (newApplication is null)
            {
                _logger.LogError($"Workflow type {request.WorkflowType} does not exist");
                return new HandlerResponse<StartApplicationResponse>(false, $"WorkflowType '{request.WorkflowType}' does not exist.");
            }

            await CopyWorkflows(cancellationToken, newApplication);
            await _dataContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Successfully created new Application. Application Id = {newApplication.Id} | Workflow = {request.WorkflowType}");
            return new HandlerResponse<StartApplicationResponse>(new StartApplicationResponse { ApplicationId = newApplication.Id });
        }

        private async Task<Data.Entities.Application> CreateNewApplication(StartApplicationRequest request, Workflow latestWorkflow, CancellationToken cancellationToken, string applicationData)
        {
            var newApplication = new Data.Entities.Application
            {
                ApplicationStatus = ApplicationStatus.InProgress,
                WorkflowId = latestWorkflow.Id,
                Reference = request.UserReference,
                CreatedAt = SystemTime.UtcNow(),
                ApplicationData = applicationData
            };

            _dataContext.Applications.Add(newApplication);
            _logger.LogInformation($"Created Application entity: {newApplication.Id}");

            return newApplication;
        }

        private async Task CopyWorkflows(CancellationToken cancellationToken, Data.Entities.Application newApplication)
        {

            var workflowSequences = await _dataContext.WorkflowSequences.AsNoTracking()
                .Where(seq => seq.WorkflowId == newApplication.WorkflowId).ToListAsync(cancellationToken);

            var groupedSequences = workflowSequences.GroupBy(seq => new { seq.SequenceNo, seq.IsActive }).ToList();

            var newApplicationSequences = groupedSequences.Select(seq => new ApplicationSequence
            {
                ApplicationId = newApplication.Id,
                SequenceNo = seq.Key.SequenceNo,
                IsActive = seq.Key.IsActive
            }).ToList();

            await _dataContext.ApplicationSequences.AddRangeAsync(newApplicationSequences, cancellationToken);
            _logger.LogInformation($"Created ApplicationSequence entities for Application: {newApplication.Id}");

            var sectionIds = groupedSequences.SelectMany(seq => seq).Select(seq => seq.SectionId).ToList();

            var workflowSections = await _dataContext.WorkflowSections.AsNoTracking()
                .Where(sec => sectionIds.Contains(sec.Id)).ToListAsync(cancellationToken: cancellationToken);

            var newApplicationSections = new List<ApplicationSection>();
            foreach (var sequence in groupedSequences)
            {
                var applicationSequence = newApplicationSequences.Single(appSeq => appSeq.SequenceNo == sequence.Key.SequenceNo);

                foreach (var sectionDetails in sequence)
                {
                    var workflowSection = workflowSections.Single(wSec => wSec.Id == sectionDetails.SectionId);

                    var newSection = new ApplicationSection
                    {
                        Id = Guid.NewGuid(),
                        SequenceId = applicationSequence.Id,
                        Title = workflowSection.Title,
                        LinkTitle = workflowSection.LinkTitle,
                        ApplicationId = newApplication.Id,
                        DisplayType = workflowSection.DisplayType,
                        QnAData = workflowSection.QnAData,
                        SectionNo = sectionDetails.SectionNo,
                        SequenceNo = sectionDetails.SequenceNo
                    };

                    foreach (var page in newSection.QnAData.Pages)
                    {
                        page.SectionId = newSection.Id;
                        page.SequenceId = newSection.SequenceId;
                    }

                    newApplicationSections.Add(newSection);
                }
            }

            await _dataContext.ApplicationSections.AddRangeAsync(newApplicationSections, cancellationToken);
            _logger.LogInformation($"Created ApplicationSection entities for Application: {newApplication.Id}");
        }
    }
}