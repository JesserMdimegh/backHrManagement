using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Back_HR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Back_HR.Controllers.AuthController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            User user;
            switch (model.UserType)
            {
                case "RH":
                    user = new RH { Email = model.Email, UserName = model.Email };
                    break;
                case "Employe":
                    user = new Employe { Email = model.Email, UserName = model.Email, Poste = model.Poste };
                    break;
                case "Candidat":
                    user = new Candidat { Email = model.Email, UserName = model.Email, Cv = model.Cv };
                    break;
                default:
                    return BadRequest("Invalid UserType");
            }

            user.Firstname = model.Firstname;
            user.Lastname = model.Lastname;
            user.Telephone = model.Telephone;

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.UserType);
                return Ok("User created successfully");
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var token = GenerateJwtToken(user);
                return Ok(new { Token = token });
            }
            return Unauthorized("Invalid credentials");
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserType", user.GetType().Name) // e.g., "RH", "Employe", "Candidat"
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMonths(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("logout")]
        [Authorize] // Requires a valid token
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var dbContext = HttpContext.RequestServices.GetRequiredService<HRContext>();
            var jwtToken = new JwtSecurityToken(token);
            dbContext.RevokedTokens.Add(new RevokedToken
            {
                Id = Guid.NewGuid(),
                Token = token,
                Expiration = jwtToken.ValidTo
            });
            await dbContext.SaveChangesAsync();
            return Ok("Logged out successfully");
        }

    }
}

    
