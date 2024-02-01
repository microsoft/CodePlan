using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.QnA.Api.Infrastructure;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Commands.AddPageAnswer;
using SFA.DAS.QnA.Application.Commands.RemovePageAnswer;
using SFA.DAS.QnA.Application.Commands.SetPageAnswers;
using SFA.DAS.QnA.Application.Commands.ResetPageAnswers;
using SFA.DAS.QnA.Application.Commands.ResetPagesToIncomplete;
using SFA.DAS.QnA.Application.Commands.SkipPage;
using SFA.DAS.QnA.Application.Queries.Sections.GetPage;
using SFA.DAS.QnA.Application.Queries.Sections.CanUpdatePage;
using System.Text.Json;

namespace SFA.DAS.QnA.Api.Controllers
{
    [Route("applications")]
    [Produces("application/json")]
    public class PagesController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PagesController> _logger;

        public PagesController(ILogger<PagesController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        ///     Returns the requested Page
        /// </summary>
        /// <returns>The requested Page</returns>
        /// <response code="200">Returns a Page</response>
        /// <response code="404">If the ApplicationId, SectionId or PageId are invalid</response>
        [HttpGet("{applicationId}/sections/{sectionId}/pages/{pageId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Page>> GetPage(Guid applicationId, Guid sectionId, string pageId)
        {
            var pageResponse = await _mediator.Send(new GetPageRequest(applicationId, sectionId, pageId), CancellationToken.None);
            if (!pageResponse.Success)
            {
                _logger.LogError($"Unable to find page {pageId} | Reason : {pageResponse.Message}");
                return NotFound();
            }

            return pageResponse.Value;
        }

        /// <summary>
        ///     Returns the requested Page
        /// </summary>
        /// <returns>The requested Page</returns>
        /// <response code="200">Returns a Page</response>
        /// <response code="404">If the ApplicationId, SequenceNo, SectionNo or PageId are invalid</response>
        [HttpGet("{applicationId}/sequences/{sequenceNo}/sections/{sectionNo}/pages/{pageId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Page>> GetPageBySectionNo(Guid applicationId, int sequenceNo, int sectionNo, string pageId)
        {
            var pageResponse = await _mediator.Send(new GetPageBySectionNoRequest(applicationId, sequenceNo, sectionNo, pageId), CancellationToken.None);
            if (!pageResponse.Success)
            {
                _logger.LogError($"Unable to find page {pageId} | Reason : {pageResponse.Message}");
                return NotFound();
            }

            return pageResponse.Value;
        }

        /// <summary>
        ///     Check if the requested Page can be updated with a new answer
        /// </summary>
        /// <returns>True if the page can be updated, otherwise False</returns>
        /// <response code="200">Returns a boolean</response>
        /// <response code="404">If the ApplicationId, SectionId or PageId are invalid</response>
        [HttpGet("{applicationId}/sections/{sectionId}/pages/{pageId}/canupdate")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<bool>> CanUpdatePage(Guid applicationId, Guid sectionId, string pageId)
        {
            var canUpdateResponse = await _mediator.Send(new CanUpdatePageRequest(applicationId, sectionId, pageId), CancellationToken.None);
            if (!canUpdateResponse.Success)
            {
                _logger.LogError($"Unable to find page {pageId} | Reason : {canUpdateResponse.Message}");
                return NotFound();
            }

            return canUpdateResponse.Value;
        }

        /// <summary>
        ///     Check if the requested Page can be updated with a new answer
        /// </summary>
        /// <returns>True if the page can be updated, otherwise False</returns>
        /// <response code="200">Returns a boolean</response>
        /// <response code="404">If the ApplicationId, SequenceNo, SectionNo or PageId are invalid</response>
        [HttpGet("{applicationId}/sequences/{sequenceNo}/sections/{sectionNo}/pages/{pageId}/canupdate")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<bool>> CanUpdatePageBySectionNo(Guid applicationId, int sequenceNo, int sectionNo, string pageId)
        {
            var canUpdateResponse = await _mediator.Send(new CanUpdatePageBySectionNoRequest(applicationId, sequenceNo, sectionNo, pageId), CancellationToken.None);
            if (!canUpdateResponse.Success)
            {
                _logger.LogError($"Unable to find page {pageId} | Reason : {canUpdateResponse.Message}");
                return NotFound();
            }

            return canUpdateResponse.Value;
        }

        /// <summary>
        ///     Sets the answers on the page.
        /// </summary>
        /// <returns>An object describing validity / next steps</returns>
        /// <response code="200">Returns the response</response>
        [HttpPost("{applicationId}/sections/{sectionId}/pages/{pageId}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<SetPageAnswersResponse>> SetPageAnswers(Guid applicationId, Guid sectionId, string pageId, [FromBody] List<Answer> answers)
        {
            _logger.LogInformation($"Answers sent to SetPageAnswers: {JsonSerializer.Serialize(answers)}");

            var savePageAnswersResponse = await _mediator.Send(new SetPageAnswersRequest(applicationId, sectionId, pageId, answers), CancellationToken.None);
            if (!savePageAnswersResponse.Success)
            {
                _logger.LogError($"Unable to save answers for page {pageId} | Reason : {savePageAnswersResponse.Message}");
                return BadRequest(new BadRequestError(savePageAnswersResponse.Message));
            }

            _logger.LogInformation($"Response from SetPageAnswers: {JsonSerializer.Serialize(savePageAnswersResponse.Value)}");

            return savePageAnswersResponse.Value;
        }

        /// <summary>
        ///     Sets the answers on the page.
        /// </summary>
        /// <returns>An object describing validity / next steps</returns>
        /// <response code="200">Returns the response</response>
        [HttpPost("{applicationId}/sequences/{sequenceNo}/sections/{sectionNo}/pages/{pageId}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<SetPageAnswersResponse>> SetPageAnswers(Guid applicationId, int sequenceNo, int sectionNo, string pageId, [FromBody] List<Answer> answers)
        {
            _logger.LogInformation($"Answers sent to SetPageAnswers: {JsonSerializer.Serialize(answers)}");

            var savePageAnswersResponse = await _mediator.Send(new SetPageAnswersBySectionNoRequest(applicationId, sequenceNo, sectionNo, pageId, answers), CancellationToken.None);
            if (!savePageAnswersResponse.Success)
            {
                _logger.LogError($"Unable to save answers for page {pageId} | Reason : {savePageAnswersResponse.Message}");
                return BadRequest(new BadRequestError(savePageAnswersResponse.Message));
            }

            _logger.LogInformation($"Response from SetPageAnswers: {JsonSerializer.Serialize(savePageAnswersResponse.Value)}");

            return savePageAnswersResponse.Value;
        }

        /// <summary>
        ///     Removes all answers on the page.
        /// </summary>
        /// <returns>An object describing if the page has had its answers reset</returns>
        /// <response code="200">Returns the response</response>
        [HttpPost("{applicationId}/sections/{sectionId}/pages/{pageId}/reset")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResetPageAnswersResponse>> ResetPageAnswers(Guid applicationId, Guid sectionId, string pageId)
        {
            _logger.LogInformation($"Resetting all Answers on page {pageId}");

            var resetPageAnswersResponse = await _mediator.Send(new ResetPageAnswersRequest(applicationId, sectionId, pageId), CancellationToken.None);
            if (!resetPageAnswersResponse.Success)
            {
                _logger.LogError($"Unable to reset answers for page {pageId} | Reason : {resetPageAnswersResponse.Message}");
                return BadRequest(new BadRequestError(resetPageAnswersResponse.Message));
            }

            _logger.LogInformation($"Response from ResetPageAnswers: {JsonSerializer.Serialize(resetPageAnswersResponse.Value)}");

            return resetPageAnswersResponse.Value;
        }

        /// <summary>
        ///     Removes all answers on the page.
        /// </summary>
        /// <returns>An object describing if the page has had its answers reset</returns>
        /// <response code="200">Returns the response</response>
        [HttpPost("{applicationId}/sequences/{sequenceNo}/sections/{sectionNo}/pages/{pageId}/reset")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResetPageAnswersResponse>> ResetPageAnswersBySectionNumber(Guid applicationId, int sequenceNo, int sectionNo, string pageId)
        {
            _logger.LogInformation($"Resetting all Answers on page {pageId}");

            var resetPageAnswersResponse = await _mediator.Send(new ResetPageAnswersBySectionNoRequest(applicationId, sequenceNo, sectionNo, pageId), CancellationToken.None);
            if (!resetPageAnswersResponse.Success)
            {
                _logger.LogError($"Unable to reset answers for sequence {sequenceNo}, section {sectionNo}, page {pageId} | Reason : {resetPageAnswersResponse.Message}");
                return BadRequest(new BadRequestError(resetPageAnswersResponse.Message));
            }

            _logger.LogInformation($"Response from ResetPageAnswersBySectionNumber: {JsonSerializer.Serialize(resetPageAnswersResponse.Value)}");

            return resetPageAnswersResponse.Value;
        }

        /// <summary>
        ///     Removes all answers on the section.
        /// </summary>
        /// <returns>An object describing if the section has had its answers reset</returns>
        /// <response code="200">Returns the response</response>
        [HttpPost("{applicationId}/sequences/{sequenceNo}/sections/{sectionNo}/reset")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResetSectionAnswersResponse>> ResetSectionAnswers(Guid applicationId, int sequenceNo, int sectionNo)
        {
            _logger.LogInformation($"Resetting all Answers on sequence {sequenceNo}, section {sectionNo} for application {applicationId}");

            var resetSectionAnswersResponse = await _mediator.Send(new ResetSectionAnswersRequest(applicationId, sequenceNo, sectionNo), CancellationToken.None);
            if (!resetSectionAnswersResponse.Success)
            {
                _logger.LogError($"Unable to reset answers for sequence {sequenceNo}, section {sectionNo}, for application {applicationId} | Reason : {resetSectionAnswersResponse.Message}");
                return BadRequest(new BadRequestError(resetSectionAnswersResponse.Message));
            }

            _logger.LogInformation($"Response from ResetPageAnswersBySectionNumber: {JsonSerializer.Serialize(resetSectionAnswersResponse.Value)}");

            return resetSectionAnswersResponse.Value;
        }

        /// <summary>
        ///     Adds an answer on a page that allows multiple sets of answers
        /// </summary>
        /// <returns>An object describing validity</returns>
        /// <response code="200">Returns the Page</response>
        /// <response code="400">If this page is not a multiple answers page or this ApplicationId, SectionId or PageId does not exist</response>
        [HttpPost("{applicationId}/sections/{sectionId}/pages/{pageId}/multiple")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<AddPageAnswerResponse>> AddPageAnswer(Guid applicationId, Guid sectionId, string pageId, [FromBody] List<Answer> answers)
        {
            var addPageAnswerResponse = await _mediator.Send(new AddPageAnswerRequest(applicationId, sectionId, pageId, answers), CancellationToken.None);
            if (!addPageAnswerResponse.Success)
            {
                _logger.LogError($"Unable to add answer to page {pageId} | Reason : {addPageAnswerResponse.Message}");
                return BadRequest(new BadRequestError(addPageAnswerResponse.Message));
            }

            return addPageAnswerResponse.Value;
        }

        /// <summary>
        ///     Removes an answer from a page that allows multiple sets of answers
        /// </summary>
        /// <returns>An object describing validity</returns>
        /// <response code="200">Returns the Page</response>
        /// <response code="400">If this page is not a multiple answers page or this ApplicationId, SectionId or PageId does not exist</response>
        [HttpDelete("{applicationId}/sections/{sectionId}/pages/{pageId}/multiple/{answerId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Page>> RemovePageAnswer(Guid applicationId, Guid sectionId, string pageId, Guid answerId)
        {
            var removePageAnswerResponse = await _mediator.Send(new RemovePageAnswerRequest(applicationId, sectionId, pageId, answerId), CancellationToken.None);
            if (!removePageAnswerResponse.Success)
            {
                _logger.LogError($"Unable to remove answer for page {pageId} | Reason : {removePageAnswerResponse.Message}");
                return BadRequest(new BadRequestError(removePageAnswerResponse.Message));
            }

            return removePageAnswerResponse.Value.Page;
        }

        /// <summary>
        ///     Skips the page specified and gets the next steps.
        /// </summary>
        /// <returns>An object describing the next steps</returns>
        /// <response code="200">Returns the response</response>
        /// <response code="400">Cannot determine the next steps, or if ApplicationId, SectionId or PageId does not exist</response>
        [HttpPost("{applicationId}/sections/{sectionId}/pages/{pageId}/skip")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<SkipPageResponse>> SkipPage(Guid applicationId, Guid sectionId, string pageId)
        {
            _logger.LogInformation($"Getting the next action...: applicationId = {applicationId} , pageId = {pageId}");

            var getNextActionResponse = await _mediator.Send(new SkipPageRequest(applicationId, sectionId, pageId), CancellationToken.None);
            if (!getNextActionResponse.Success)
            {
                _logger.LogError($"Unable to get the next action for page {pageId} | Reason : {getNextActionResponse.Message}");
                return BadRequest(new BadRequestError(getNextActionResponse.Message));
            }

            _logger.LogInformation($"Response from SkipPage: {JsonSerializer.Serialize(getNextActionResponse.Value)}");

            return getNextActionResponse.Value;
        }

        /// <summary>
        ///     Skips the page specified and gets the next steps.
        /// </summary>
        /// <returns>An object describing the next steps</returns>
        /// <response code="200">>Returns the response</response>
        /// <response code="400">Cannot determine the next steps, or if ApplicationId, SequenceNo, SectionNo or PageId does not exist</response>
        [HttpPost("{applicationId}/sequences/{sequenceNo}/sections/{sectionNo}/pages/{pageId}/skip")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<SkipPageResponse>> SkipPageBySectionNo(Guid applicationId, int sequenceNo, int sectionNo, string pageId)
        {
            _logger.LogInformation($"Getting the next action...: applicationId = {applicationId} , sequenceNo = {sequenceNo} , sectionNo = {sectionNo} , pageId = {pageId}");

            var getNextActionResponse = await _mediator.Send(new SkipPageBySectionNoRequest(applicationId, sequenceNo, sectionNo, pageId), CancellationToken.None);
            if (!getNextActionResponse.Success)
            {
                _logger.LogError($"Unable to get the next action for page {pageId} | Reason : {getNextActionResponse.Message}");
                return BadRequest(new BadRequestError(getNextActionResponse.Message));
            }

            _logger.LogInformation($"Response from SkipPageBySectionNo: {JsonSerializer.Serialize(getNextActionResponse.Value)}");

            return getNextActionResponse.Value;
        }

        /// <summary>
        ///     Sets pages in a section to complete=false, based on list of pageIdsToExclude passed in (which are not reset)
        /// </summary>
        /// <returns>confirmation</returns>
        /// <response code="200">Returns the response</response>
        [HttpPost("{applicationId}/sequences/{sequenceNo}/sections/{sectionNo}/reset-completeflag")]
        [ProducesResponseType(200)]
        public async Task SetPagesToIncomplete(Guid applicationId, int sequenceNo, int sectionNo, [FromBody] List<string> pageIdsToExclude)
        {
            _logger.LogInformation($"Pages sent to reset-completeflag to false: {JsonSerializer.Serialize(pageIdsToExclude)} in SetPagesToIncomplete");
            await _mediator.Send(new ResetPagesToIncompleteRequest(applicationId, sequenceNo, sectionNo, pageIdsToExclude), CancellationToken.None);
        }
    }
}