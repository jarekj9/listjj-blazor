﻿using Microsoft.AspNetCore.Identity;
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
using StackExchange.Redis;
using System.Linq;
using System.Collections.Generic;

namespace Listjj.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public LoginController(IConfiguration configuration,
                               SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, false, false);

            if (!result.Succeeded) return BadRequest(new LoginResult { Successful = false, Error = "Username and password are invalid." });

            var user = await _signInManager.UserManager.FindByEmailAsync(login.Email);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, login.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var usersRole = (await _signInManager.UserManager.GetRolesAsync(user)).FirstOrDefault();
            if(!string.IsNullOrWhiteSpace(usersRole))
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

            return Ok(new LoginResult { Successful = true, Token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}