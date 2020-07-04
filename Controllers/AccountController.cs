using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Converter.Models;
using Converter.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Converter.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(ITokenService tokenService, ApplicationDbContext context)
        {
            _context = context;
            _tokenService = tokenService;
        }


        [HttpPost("/login")]
        public async Task<IActionResult> Login(User userData)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Login ==  userData.Login &&
                 u.Password == userData.Password);
            if(user == null)
            {
                return BadRequest(new { errorText = "Invalid username or password."});
            }

            var identity = GetIdentity(user);

            var jwtToken = _tokenService.GenerateAccessToken(identity.Claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            await _context.SaveChangesAsync();

            var response = new 
            {
                access_token = jwtToken,
                refresh_token = refreshToken,
                username = identity.Name
            };

            return Json(response);

        }

        [HttpPost("/register")]
        public async Task<IActionResult> Register(User userData)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => 
                u.Login == userData.Login && u.Password == userData.Password);
            if(user != null)
            {
                return BadRequest(new { errorText = "User with this username is already registered"});
            }

            await _context.Users.AddAsync(ValidUserData(userData));
            await _context.SaveChangesAsync();

            return Ok(new { successText = "User register, go to Login"});
        }
        
        [HttpGet("/getuser/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if(user == null)
            {
                return NotFound();
            }

            return new ObjectResult(user);
        }

        [HttpPut("/update")]
        public async Task<IActionResult> Update(User userData)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userData.UserId); 
            if(user == null)
            {
                return NotFound();
            }


            user.UpdateModel(userData);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpDelete("/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if(user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        private ClaimsIdentity GetIdentity(User user)
        {
            
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user?.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user?.Role)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", 
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }

        private User ValidUserData(User user)
        {
            user.Role = user.Role == null ? "user" : user.Role;
            return user;
        }

    }
}