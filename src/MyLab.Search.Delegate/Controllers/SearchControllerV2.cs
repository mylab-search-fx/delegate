using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyLab.Log;
using MyLab.Search.Delegate.Models;
using MyLab.Search.Delegate.Services;
using MyLab.WebErrors;

namespace MyLab.Search.Delegate.Controllers
{
    [ApiController]
    [Route("v2/search")]
    public class SearchControllerV2 : ControllerBase
    {
        private readonly IEsRequestProcessor _requestProcessor;
        private readonly ILogger<SearchControllerV2> _logger;

        public SearchControllerV2(
            IEsRequestProcessor requestProcessor,
            ILogger<SearchControllerV2> logger)
        {
            _requestProcessor = requestProcessor;
            _logger = logger;
        }

        [HttpPost("{namespace}")]
        [ErrorToResponse(typeof(ResourceNotFoundException), HttpStatusCode.BadRequest)]
        [ErrorToResponse(typeof(InvalidTokenException), HttpStatusCode.Forbidden)]
        [ErrorToResponse(typeof(TokenizingDisabledException), HttpStatusCode.BadRequest)]
        [ErrorToResponse(typeof(ElasticsearchSearchException), HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Get(
            [FromBody] ClientSearchRequestV2 request,
            [FromRoute(Name = "namespace")] string ns,
            [FromHeader(Name = "X-Search-Token")] string searchToken)
        {
            FoundEntities<FoundEntityContent> result;

            try
            {
                result = await _requestProcessor.ProcessSearchRequestAsync(request, ns, searchToken);
            }
            catch (Exception e)
            {
                e.AndFactIs("Initial request", request)
                    .AndFactIs("Request namespace", ns);
                throw;
            }

            return Ok(result);
        }
    }
}
