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
    [Route("v4/indexes")]
    public class SearchControllerV4 : ControllerBase
    {
        private readonly IEsRequestProcessor _requestProcessor;
        private readonly ILogger<SearchControllerV4> _logger;

        public SearchControllerV4(
            IEsRequestProcessor requestProcessor,
            ILogger<SearchControllerV4> logger)
        {
            _requestProcessor = requestProcessor;
            _logger = logger;
        }

        [HttpPost("{index}/searcher")]
        [ErrorToResponse(typeof(ResourceNotFoundException), HttpStatusCode.BadRequest)]
        [ErrorToResponse(typeof(InvalidTokenException), HttpStatusCode.Forbidden)]
        [ErrorToResponse(typeof(TokenizingDisabledException), HttpStatusCode.BadRequest)]
        [ErrorToResponse(typeof(ElasticsearchSearchException), HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Search(
            [FromBody] ClientSearchRequestV4 request,
            [FromRoute(Name = "index")] string index,
            [FromHeader(Name = "X-Search-Token")] string searchToken)
        {
            FoundEntities<FoundEntityContent> result;

            try
            {
                result = await _requestProcessor.ProcessSearchRequestAsync(request, index, searchToken);
            }
            catch (Exception e)
            {
                e.AndFactIs("Initial request", request)
                    .AndFactIs("index-id", index);
                throw;
            }

            return Ok(result);
        }
    }
}
