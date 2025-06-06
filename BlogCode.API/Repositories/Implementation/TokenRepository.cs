﻿using BlogCode.API.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlogCode.API.Repositories.Implementation
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration _configuration;
        public TokenRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string CreateToken(IdentityUser user, List<string> roles)
        {
            // Create Claim
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
            };
            claims.AddRange(roles.Select(role=> new Claim(ClaimTypes.Role, role)));

            // JWT security token parameters
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires:DateTime.Now.AddMinutes(15),
                signingCredentials: credentials
                );
            // Return the token
            return new JwtSecurityTokenHandler().WriteToken(token);           
        }
    }
}
