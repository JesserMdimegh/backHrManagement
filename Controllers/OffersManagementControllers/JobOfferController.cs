using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Back_HR.DTOs;
using Back_HR.Models;
using Back_HR.Models.enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Back_HR.Controllers.OffersManagementControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobOfferController : ControllerBase
    {

        private readonly HRContext _context;
        private readonly UserManager<User> _userManager;


        public JobOfferController(HRContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("create")]
        [Authorize(Policy = "RHOnly")]
        public async Task<IActionResult> CreateJobOffer([FromBody] JobOfferDtoCreate dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var email = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            Console.WriteLine($"Extracted email (sub): {email ?? "null"}");
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("Unable to identify user.");
            }

            
            var rhUser = await _userManager.FindByEmailAsync(email) as RH;
            if (rhUser == null)
            {
                return Unauthorized("RH user not found.");
            }

            
            var jobOffer = new JobOffer
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Experience = dto.Experience,
                Salary = dto.Salary,
                Location = dto.Location,
                PublishDate = DateTime.Now, 
                Status = OffreStatus.OPEN, 
                RHId = rhUser.Id,           
                RHResponsable = rhUser,    
                Competences = dto.Competences ?? new List<Competence>() 
            };

            // Add to database
            _context.JobOffers.Add(jobOffer);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Job offer created successfully", JobOfferId = jobOffer.Id });
        }
    }
}
