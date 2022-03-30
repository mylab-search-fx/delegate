using System.Net;
using Microsoft.AspNetCore.Mvc;
using MyLab.Search.Delegate.Models;
using MyLab.Search.Delegate.Services;
using MyLab.WebErrors;

namespace MyLab.Search.Delegate.Controllers
{
    [ApiController]
    [Route("v3/token")]
    public class TokenControllerV3 : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public TokenControllerV3(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost]
        [ErrorToResponse(typeof(TokenizingDisabledException), HttpStatusCode.Locked)]
        public IActionResult Post([FromBody] TokenRequestV3 tokenRequest)
        {
            return Ok(_tokenService.CreateSearchToken(tokenRequest));
        }
    }
}