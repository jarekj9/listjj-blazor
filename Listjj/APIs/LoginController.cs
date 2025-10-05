using Blazorise.Extensions;
using Google.Apis.Auth;
using Listjj.Abstract;
using Listjj.Data.Options;
using Listjj.Infrastructure.Data;
using Listjj.Infrastructure.Models;
using Listjj.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace Listjj.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private ApplicationUser _user;
        private readonly MicrosoftAuthOptions _microsoftAuthOptions;

        public LoginController(IConfiguration configuration, UserManager<ApplicationUser> userManager, IUserService userService,  SignInManager<ApplicationUser> signInManager, 
            IOptions<GoogleAuthOptions> googleAuthOptions, IOptions<MicrosoftAuthOptions> microsoftAuthOptions)
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            _userService = userService;
            _user = null;
            _microsoftAuthOptions = microsoftAuthOptions.Value;
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
            else if (!string.IsNullOrWhiteSpace(login.MicrosoftJwt))
            {
                if (!(await TryAuthWithMicrosoft(login)))
                {
                    return Unauthorized();
                }
            }
            else
            {
                var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, false, false);
                if (!result.Succeeded) return BadRequest(new LoginResult { Successful = false, Error = "Username and password are invalid." });
                _user = await _signInManager.UserManager.FindByEmailAsync(login.Email);
            }

            var token = await GenerateToken(login);

            return Ok(new LoginResult { Successful = true, Token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        [Route("serviceclientlogin")]

        [HttpPost]
        public async Task<IActionResult> ServiceClientLogin([FromBody] LoginModel login)
        {
            _user = await _userService.GetByApiKey(login.Password);
            if (_user == null)
            {
                return Unauthorized();
            }
            var token = await GenerateToken(login);
            return Ok(new LoginResult { Successful = true, Token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        private async Task<bool> TryAuthWithGoogle(LoginModel login)
        {
            var verifiedPayload = await GoogleJsonWebSignature.ValidateAsync(login.GoogleJwt);
            login.Email = verifiedPayload.Email; // before that it is none, necessary for main Login method
            var verifiedEmail = verifiedPayload.Email;
            if (verifiedEmail.IsNullOrEmpty())
            {
                return false;
            }
            var signInResult = await HandleVerifiedEmailSignIn(verifiedEmail);
            return signInResult;
        }

        private async Task<bool> TryAuthWithMicrosoft(LoginModel login)
        {
            var claimsPrincipal = await ValidateMicrosoftTokenAsync(login.MicrosoftJwt);
            var verifiedEmail = claimsPrincipal != null && claimsPrincipal.Identity.IsAuthenticated ? 
                claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value : null;
            login.Email = verifiedEmail; // before that it is none, necessary for main Login method
            if (verifiedEmail.IsNullOrEmpty())
            {
                return false;
            }
            var signInResult = await HandleVerifiedEmailSignIn(verifiedEmail);
            return signInResult;
        }

        private async Task<bool> HandleVerifiedEmailSignIn(string verifiedEmail)
        {
            _user = await _signInManager.UserManager.FindByEmailAsync(verifiedEmail);
            if (_user == null)
            {
                _user = new ApplicationUser { UserName = verifiedEmail, Email = verifiedEmail };
                var createUserResult = await _userManager.CreateAsync(_user);
                if (!createUserResult.Succeeded)
                {
                    return false;
                }
            }
            await _signInManager.SignInAsync(_user, isPersistent: false);
            return true;
        }

        private async Task<JwtSecurityToken> GenerateToken(LoginModel login)
        {
            var claims = new List<Claim>
            {
                new Claim("username", login.Email),
                new Claim(ClaimTypes.Name, login.Email),
                new Claim(ClaimTypes.NameIdentifier, _user.Id.ToString())
            };

            var usersRole = (await _signInManager.UserManager.GetRolesAsync(_user)).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(usersRole))
            {
                claims.Add(new Claim("role", usersRole));
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

        public async Task<ClaimsPrincipal> ValidateMicrosoftTokenAsync(string token)
        {
            var discoveryUrl = $"https://login.microsoftonline.com/{_microsoftAuthOptions.TenantId}/v2.0/.well-known/openid-configuration";
            Console.WriteLine($"DISCOVERY URL {discoveryUrl}");

            var mgr = new ConfigurationManager<OpenIdConnectConfiguration>(
                discoveryUrl,
                new OpenIdConnectConfigurationRetriever()
            );
            var cfg = await mgr.GetConfigurationAsync();
            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = cfg.Issuer,
                ValidAudience = _microsoftAuthOptions.ClientId,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKeys = cfg.SigningKeys
            };

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);
                return principal;
            }
             catch (Exception ex)
            {
                Console.WriteLine("Token validation failed: " + ex.Message);
                return null;
            }
        }
    }
}
