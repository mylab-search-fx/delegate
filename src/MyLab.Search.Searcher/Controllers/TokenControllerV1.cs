using System.Net;
using Microsoft.AspNetCore.Mvc;
using MyLab.Search.Searcher.Models;
using MyLab.Search.Searcher.Services;
using MyLab.WebErrors;

namespace MyLab.Search.Searcher.Controllers
{
    [ApiController]
    [Route("v1/token")]
    public class TokenControllerV1 : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public TokenControllerV1(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost]
        [ErrorToResponse(typeof(TokenizingDisabledException), HttpStatusCode.Locked)]
        public IActionResult Post([FromBody] TokenRequestV1 tokenRequest)
        {
            return Ok(_tokenService.CreateSearchToken(tokenRequest.ToV3()));
        }
    }
}
