using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MyLab.Search.Delegate.Models;
using MyLab.WebErrors;

namespace MyLab.Search.Delegate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        [HttpPost]
        [ErrorToResponse(typeof(TokenizingDisabledException), HttpStatusCode.Locked)]
        public async Task<IActionResult> Post([FromBody] TokenRequest tokenRequest)
        {
            return Ok();
        }
    }
}
