using System.Net;
using Microsoft.AspNetCore.Mvc;
using MyLab.Search.Searcher.Models;
using MyLab.Search.Searcher.Services;
using MyLab.WebErrors;

namespace MyLab.Search.Searcher.Controllers
{
    [ApiController]
    [Route("v4/token")]
    public class TokenControllerV4 : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public TokenControllerV4(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost]
        [ErrorToResponse(typeof(TokenizingDisabledException), HttpStatusCode.Locked)]
        public IActionResult Post([FromBody] TokenRequestV4 tokenRequest)
        {
            return Ok(_tokenService.CreateSearchToken(tokenRequest));
        }
    }
}