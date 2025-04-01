using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Back_HR.DTOs;
using Back_HR.Models;
using Back_HR.Models.enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Back_HR.Controllers.AuthController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly HRContext _context;

        public AuthController(UserManager<User> userManager, IConfiguration configuration, HRContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context; 
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterModel model)
        {
            Console.WriteLine("Registering user*************************");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Handle CV file upload
            string? cvPath = null;
            if (model.CvFile != null && model.CvFile.Length > 0)
            {
                // Ensure the file is a PDF
                if (model.CvFile.ContentType != "application/pdf")
                {
                    return BadRequest("Only PDF files are allowed for CV upload.");
                }

                // Define the storage path (e.g., wwwroot/uploads/cvs/)
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/cvs");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate a unique file name to avoid overwriting
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.CvFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file to the server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.CvFile.CopyToAsync(fileStream);
                }

                // Store the relative path or URL in the database
                cvPath = $"/uploads/cvs/{uniqueFileName}";
            }

            var user = new Candidat
            {
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                Telephone = model.Telephone,
                Email = model.Email,
                UserName = model.Email,
                Cv = cvPath,
                UserType = UserType.CANDIDAT
            };

            if (model.Competences != null && model.Competences.Any())
            {
                var validCompetences = model.Competences
                    .Where(c => !string.IsNullOrEmpty(c.Titre) || c.Id != Guid.Empty)
                    .ToList();

                if (validCompetences.Any())
                {
                    var competenceIds = validCompetences.Where(c => c.Id != Guid.Empty).Select(c => c.Id).ToList();
                    var competenceTitres = validCompetences.Where(c => c.Id == Guid.Empty).Select(c => c.Titre).ToList();

                    var existingCompetences = await _context.Competences
                        .Where(c => competenceIds.Contains(c.Id) || competenceTitres.Contains(c.Titre))
                        .ToListAsync();

                    var newCompetences = validCompetences
                        .Where(c => c.Id == Guid.Empty && !existingCompetences.Any(ec => ec.Titre == c.Titre))
                        .Select(c => new Competence
                        {
                            Id = Guid.NewGuid(),
                            Titre = c.Titre
                        })
                        .ToList();

                    if (newCompetences.Any())
                    {
                        _context.Competences.AddRange(newCompetences);
                        await _context.SaveChangesAsync();
                    }

                    
                    user.Competences = existingCompetences.Concat(newCompetences).ToList();
                }
            }

            // Create the user
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            // Add the role
            await _userManager.AddToRoleAsync(user, user.UserType.ToString());

            // Add UserType claim 
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("UserType", user.UserType.ToString()));

            // Generate JWT token
            var token = GenerateJwtToken(user);
            
            // Save changes to persist the many-to-many relationship
            await _context.SaveChangesAsync();

            return Ok(new { Message = "User created successfully", Token = token , user = user });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var token = GenerateJwtToken(user);
                return Ok(new { Token = token , user });
            }
            return Unauthorized("Invalid credentials");
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserType", user.UserType.ToString()),
                new Claim("Identifier",user.Id.ToString())
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
        [Authorize] 
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

    
