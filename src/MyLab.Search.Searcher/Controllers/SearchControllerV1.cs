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
    [Obsolete]
    [ApiController]
    [Route("v1/search")]
    public class SearchControllerV1 : ControllerBase
    {
        private readonly IEsRequestProcessor _requestProcessor;
        private readonly ILogger<SearchControllerV1> _logger;

        public SearchControllerV1(
            IEsRequestProcessor requestProcessor,
            ILogger<SearchControllerV1> logger)
        {
            _requestProcessor = requestProcessor;
            _logger = logger;
        }

        [HttpGet("{namespace}")]
        [ErrorToResponse(typeof(ResourceNotFoundException), HttpStatusCode.BadRequest)]
        [ErrorToResponse(typeof(InvalidTokenException), HttpStatusCode.Forbidden)]
        [ErrorToResponse(typeof(TokenizingDisabledException), HttpStatusCode.BadRequest)]
        [ErrorToResponse(typeof(ElasticsearchSearchException), HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Get(
            [FromQuery]ClientSearchRequestV1 request, 
            [FromRoute(Name = "namespace")]string ns, 
            [FromHeader(Name = "X-Search-Token")]string searchToken)
        {
            FoundEntities<FoundEntityContent> result;

            try
            {
                result = await _requestProcessor.ProcessSearchRequestAsync(request.ToV3(), ns, searchToken);
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
