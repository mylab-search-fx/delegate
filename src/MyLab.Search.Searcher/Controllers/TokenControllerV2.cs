using System.Net;
using Microsoft.AspNetCore.Mvc;
using MyLab.Search.Searcher.Models;
using MyLab.Search.Searcher.Services;
using MyLab.WebErrors;

namespace MyLab.Search.Searcher.Controllers
{
    [ApiController]
    [Route("v2/token")]
    public class TokenControllerV2 : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public TokenControllerV2(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost]
        [ErrorToResponse(typeof(TokenizingDisabledException), HttpStatusCode.Locked)]
        public IActionResult Post([FromBody] TokenRequestV2 tokenRequest)
        {
            return Ok(_tokenService.CreateSearchToken(tokenRequest.ToV4()));
        }
    }
}