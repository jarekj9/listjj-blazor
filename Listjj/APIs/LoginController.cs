using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System;
using Listjj.Infrastructure.Models;
using Listjj.Infrastructure.Data;
using Listjj.Models;
using System.Linq;
using System.Collections.Generic;
using Google.Apis.Auth;


namespace Listjj.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private ApplicationUser user;

        public LoginController(IConfiguration configuration, UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userManager = userManager;
            user = null;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            if (!string.IsNullOrWhiteSpace(login.GoogleJwt))
            {
                if(!(await TryAuthWithGoogle(login)))
                {
                    return Unauthorized();
                }
            }
            else
            {
                var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, false, false);
                if (!result.Succeeded) return BadRequest(new LoginResult { Successful = false, Error = "Username and password are invalid." });
                user = await _signInManager.UserManager.FindByEmailAsync(login.Email);
            }

            var token = await GenerateToken(login);

            return Ok(new LoginResult { Successful = true, Token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        private async Task<bool> TryAuthWithGoogle(LoginModel login)
        {
            user = await _signInManager.UserManager.FindByEmailAsync(login.Email);
            if (user == null)
            {
                user = new ApplicationUser { UserName = login.Email, Email = login.Email };
                var createUserResult = await _userManager.CreateAsync(user);
                if (!createUserResult.Succeeded)
                {
                    return false;
                }
            }
            try
            {
                var verifiedPayload = await GoogleJsonWebSignature.ValidateAsync(login.GoogleJwt);
                if (verifiedPayload != null)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return true;
                }
                return false;
            }
            catch (InvalidJwtException ex)
            {
                return false;
            }
        }

        private async Task<JwtSecurityToken> GenerateToken(LoginModel login)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, login.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var usersRole = (await _signInManager.UserManager.GetRolesAsync(user)).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(usersRole))
            {
                claims.Add(new Claim(ClaimTypes.Role, usersRole));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecurityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddDays(Convert.ToInt32(_configuration["JwtExpiryInDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtAudience"],
                claims,
                expires: expiry,
                signingCredentials: creds
            );

            return token;
        }

    }
}
