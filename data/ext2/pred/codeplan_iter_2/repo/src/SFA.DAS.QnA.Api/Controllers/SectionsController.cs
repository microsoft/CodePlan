using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.QnA.Api.Infrastructure;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Application.Queries.Sections.GetSection;
using SFA.DAS.QnA.Application.Queries.Sections.GetSections;
using SFA.DAS.QnA.Application.Queries.Sections.GetSequenceSections;

namespace SFA.DAS.QnA.Api.Controllers
{
    [Route("applications")]
    [Produces("application/json")]
    public class SectionsController : Controller
    {
        private readonly IMediator _mediator;

        public SectionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        ///     Returns all of the Sections for the Application
        /// </summary>
        /// <returns>An array of Sections</returns>
        /// <response code="200">Returns the Application's Sections</response>
        /// <response code="204">If there are no Sections for the given Application Id</response>
        /// <response code="404">If there is no Application for the given Application Id</response>
        [HttpGet("{applicationId}/sections")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<List<Section>>> GetSections(Guid applicationId)
        {
            var sections = await _mediator.Send(new GetSectionsRequest(applicationId), CancellationToken.None);
            if (!sections.Success) return NotFound(new NotFoundError(sections.Message));
            if (sections.Value.Count == 0) return NoContent();

            return sections.Value;
        }

        /// <summary>
        ///     Returns the Sequence's Sections
        /// </summary>
        /// <returns>The Sequence's Sections</returns>
        /// <response code="200">Returns the Sequence's Sections</response>
        /// <response code="204">If there are no Sections for the given SequenceId</response>
        /// <response code="404">If the ApplicationId or SequenceId are invalid</response>
        [HttpGet("{applicationId}/sequences/{sequenceId}/sections")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<List<Section>>> GetSequenceSections(Guid applicationId, Guid sequenceId)
        {
            var sectionsResponse = await _mediator.Send(new GetSequenceSectionsRequest(applicationId, sequenceId), CancellationToken.None);
            if (!sectionsResponse.Success) return NotFound();
            if (sectionsResponse.Value == null) return NoContent();

            return sectionsResponse.Value;
        }

        /// <summary>
        ///     Returns the requested Section
        /// </summary>
        /// <returns>The requested Section</returns>
        /// <response code="200">Returns a Section</response>
        /// <response code="404">If the ApplicationId or SectionId are invalid</response>
        [HttpGet("{applicationId}/sections/{sectionId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Section>> GetSection(Guid applicationId, Guid sectionId)
        {
            var sectionsResponse = await _mediator.Send(new GetSectionRequest(applicationId, sectionId), CancellationToken.None);
            if (!sectionsResponse.Success) return NotFound();

            return sectionsResponse.Value;
        }

        /// <summary>
        ///     Returns the requested Section
        /// </summary>
        /// <returns>The requested Section</returns>
        /// <response code="200">Returns a Section</response>
        /// <response code="404">If the ApplicationId, SequenceNo or SectionNo are invalid</response>
        [HttpGet("{applicationId}/sequences/{sequenceNo}/sections/{sectionNo}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Section>> GetSectionBySectionNo(Guid applicationId, int sequenceNo, int sectionNo)
        {
            var sectionsResponse = await _mediator.Send(new GetSectionBySectionNoRequest(applicationId, sequenceNo, sectionNo), CancellationToken.None);
            if (!sectionsResponse.Success) return NotFound();

            return sectionsResponse.Value;
        }
    }
}