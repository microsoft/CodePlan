using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.QnA.Api.Infrastructure;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.PageFeedback.CompleteFeedbackWithinSequence;
using SFA.DAS.QnA.Application.Commands.PageFeedback.DeleteFeedback;
using SFA.DAS.QnA.Application.Commands.PageFeedback.UpsertFeedback;

namespace SFA.DAS.QnA.Api.Controllers
{
    [Route("applications")]
    [Produces("application/json")]
    public class FeedbackController : Controller
    {
        private readonly ILogger<FeedbackController> _logger;
        private readonly IMediator _mediator;

        public FeedbackController(ILogger<FeedbackController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        ///     Inserts / Updates Page Feedback
        /// </summary>
        /// <returns>The page containing the feedback</returns>
        /// <response code="200">Returns a Page</response>
        /// <response code="404">If the ApplicationId, SectionId or PageId are invalid</response>
        [HttpPut("{applicationId}/sections/{sectionId}/pages/{pageId}/feedback")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Page>> UpsertFeedback(Guid applicationId, Guid sectionId, string pageId, [FromBody] Feedback feedback)
        {
            var upsertFeedbackResponse = await _mediator.Send(new UpsertFeedbackRequest(applicationId, sectionId, pageId, feedback), CancellationToken.None);
            if (!upsertFeedbackResponse.Success)
            {
                _logger.LogError($"Unable to upsert feedback for page {pageId} | Reason : {upsertFeedbackResponse.Message}");
                return BadRequest(new BadRequestError(upsertFeedbackResponse.Message));
            }

            return upsertFeedbackResponse.Value;
        }

        [HttpDelete("{applicationId}/sections/{sectionId}/pages/{pageId}/feedback/{feedbackId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Page>> DeleteFeedback(Guid applicationId, Guid sectionId, string pageId, Guid feedbackId)
        {
            var deleteFeedbackResponse = await _mediator.Send(new DeleteFeedbackRequest(applicationId, sectionId, pageId, feedbackId), CancellationToken.None);
            if (!deleteFeedbackResponse.Success)
            {
                _logger.LogError($"Unable to delete feedback for page {pageId} | Reason : {deleteFeedbackResponse.Message}");
                return NotFound(new NotFoundError(deleteFeedbackResponse.Message));
            }

            return deleteFeedbackResponse.Value;
        }


        /// <summary>
        ///     Completes all Feedback within the Sequence and its Sections
        /// </summary>
        /// <returns>Boolean stating that all Feedback as been set to complete</returns>
        /// <response code="200">Returns success</response>
        /// <response code="404">If the ApplicationId or SequenceId are invalid</response>
        [HttpPost("{applicationId}/sequence/{sequenceId}/feedback/completed")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<bool>> CompleteFeedbackWithinSequence(Guid applicationId, Guid sequenceId)
        {
            var completeFeedbackWithinSequenceResponse = await _mediator.Send(new CompleteFeedbackWithinSequenceRequest(applicationId, sequenceId), CancellationToken.None);
            if (!completeFeedbackWithinSequenceResponse.Success) return BadRequest(new BadRequestError(completeFeedbackWithinSequenceResponse.Message));

            return completeFeedbackWithinSequenceResponse.Value;
        }
    }
}