using System.Threading.Tasks;
using Converter.Models;
using Converter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Converter.Controllers
{
    public class TokenController : Controller
    {
        private readonly ITokenService _tokenService;
        private readonly ApplicationDbContext _context;
        public TokenController(ITokenService tokenService, ApplicationDbContext context)
        {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("/refresh")]
        public async Task<IActionResult> Refresh(string token, string refreshToken)
        {
            System.Console.WriteLine(token);
            System.Console.WriteLine(refreshToken);
            var principal = _tokenService.GetPrincipalFromExpiredToken(token);
            var username = principal.Identity.Name;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == username);
            if (user == null || user.RefreshToken != refreshToken) return BadRequest();

            var newJwtToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _context.SaveChangesAsync();

            return new ObjectResult(new
            {
                token = newJwtToken,
                refreshToken = newRefreshToken
            });
        }

        [Authorize]
        [Route("revoke")]
        public async Task<IActionResult> Revoke()
        {
            var username = User.Identity.Name;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == username);
            if (user == null) return BadRequest();

            user.RefreshToken = null;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}