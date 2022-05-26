using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyLab.Log;
using MyLab.Search.Searcher.Models;
using MyLab.Search.Searcher.Services;
using MyLab.WebErrors;

namespace MyLab.Search.Searcher.Controllers
{
    [ApiController]
    [Route("v3/search")]
    public class SearchControllerV3 : ControllerBase
    {
        private readonly IEsRequestProcessor _requestProcessor;
        private readonly ILogger<SearchControllerV3> _logger;

        public SearchControllerV3(
            IEsRequestProcessor requestProcessor,
            ILogger<SearchControllerV3> logger)
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
            [FromBody] ClientSearchRequestV3 request,
            [FromRoute(Name = "namespace")] string ns,
            [FromHeader(Name = "X-Search-Token")] string searchToken)
        {
            FoundEntities<FoundEntityContent> result;

            try
            {
                result = await _requestProcessor.ProcessSearchRequestAsync(request.ToV4(), ns, searchToken);
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
