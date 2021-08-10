using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using MyLab.Log;
using MyLab.Search.Delegate.Models;
using MyLab.Search.Delegate.Services;
using MyLab.WebErrors;
using Nest;
using SearchRequest = MyLab.Search.Delegate.Models.SearchRequest;

namespace MyLab.Search.Delegate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly IEsRequestProcessor _requestProcessor;
        private readonly ILogger<SearchController> _logger;

        public SearchController(
            IEsRequestProcessor requestProcessor,
            ILogger<SearchController> logger)
        {
            _requestProcessor = requestProcessor;
            _logger = logger;
        }

        [HttpGet("{namespace}")]
        [ErrorToResponse(typeof(ResourceNotFoundException), HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Get([FromQuery]SearchRequest request, [FromRoute(Name = "namespace")]string ns)
        {
            IEnumerable<EsIndexedEntity> result;

            try
            {
                result = await _requestProcessor.ProcessSearchRequestAsync(request, ns);
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
