using System.Net;
using Microsoft.AspNetCore.Mvc;
using MyLab.Search.Delegate.Services;
using MyLab.WebErrors;
using TokenRequest = MyLab.Search.Delegate.Models.TokenRequest;

namespace MyLab.Search.Delegate.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost]
        [ErrorToResponse(typeof(TokenizingDisabledException), HttpStatusCode.Locked)]
        public IActionResult Post([FromBody] TokenRequest tokenRequest)
        {
            return Ok(_tokenService.CreateSearchToken(tokenRequest));
        }
    }
}
