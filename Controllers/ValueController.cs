using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Converter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValueController : Controller
    {
        
        [Authorize]
        [Route("getlogin")]
        public IActionResult GetLogin()
        {
            return Ok($"Your login: {User.Identity.Name}");
        }

        [Authorize]
        [Route("getrole")]
        public IActionResult GetRole()
        {
            return Ok($"Your role: admin");
        }
    }
}