using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.QnA.Api.Controllers.Deserializer;
using SFA.DAS.QnA.Api.Infrastructure;
using SFA.DAS.QnA.Application.Commands.SetApplicationData;
using SFA.DAS.QnA.Application.Queries.ApplicationData.GetApplicationData;

namespace SFA.DAS.QnA.Api.Controllers
{
    [Route("/applications")]
    [Produces("application/json")]
    public class ApplicationDataController : Controller
    {
        private readonly IMediator _mediator;

        public ApplicationDataController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        ///     Returns the ApplicationData for the Application
        /// </summary>
        /// <returns>The ApplicationData</returns>
        /// <response code="200">Returns the Application's ApplicationData</response>
        /// <response code="404">If there is no Application for the given Application Id</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpGet("{applicationId}/applicationData")]
        public async Task<ActionResult<object>> Get(Guid applicationId)
        {
            var applicationDataResponse = await _mediator.Send(new GetApplicationDataRequest(applicationId));

            if (!applicationDataResponse.Success) return NotFound(new NotFoundError(applicationDataResponse.Message));

            return Ok(HandlerResponseDeserializer.Deserialize(applicationDataResponse));
        }

        /// <summary>
        ///     Returns the QuestionTag Value for an Application by ApplicationId and QuestionTag
        /// </summary>
        /// <param name="applicationId">ApplicationId (Guid)</param>
        /// <param name="questionTag">QuestionTag (string)</param>
        /// <returns>The QuestionTag Value</returns>
        /// <response code="200">Returns the QuestionTag Value</response>
        /// <response code="404">If there is no Application for the given Application Id or QuestionTag does not exist.</response>
        /// <response code="400">QuestionTag value is null.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [HttpGet("{applicationId}/applicationData/{questionTag}")]
        public async Task<ActionResult<string>> GetQuestionTagData(Guid applicationId, string questionTag)
        {
            var applicationDataResponse = await _mediator.Send(new GetQuestionTagDataRequest(applicationId, questionTag));

            if (!applicationDataResponse.Success) return NotFound(new NotFoundError(applicationDataResponse.Message));

            return applicationDataResponse.Value;
        }

        /// <summary>
        ///     Sets the ApplicationData for the Application
        /// </summary>
        /// <response code="200">Returns the Application's ApplicationData</response>
        /// <response code="404">If there is no Application for the given Application Id</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpPost("{applicationId}/applicationData")]
        public async Task<ActionResult<object>> Post(Guid applicationId, [FromBody] dynamic applicationData)
        {
            var applicationDataResponse = await _mediator.Send(new SetApplicationDataRequest(applicationId, applicationData));

            if (!applicationDataResponse.Success) return NotFound(new NotFoundError(applicationDataResponse.Message));

            return HandlerResponseDeserializer.Deserialize(applicationDataResponse);
        }
    }
}