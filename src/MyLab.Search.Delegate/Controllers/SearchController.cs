using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using MyLab.Search.Delegate.Services;
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

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]SearchRequest request)
        {
            var result = await _requestProcessor.ProcessSearchRequestAsync(request);
            return Ok(result);
        }
    }
}
