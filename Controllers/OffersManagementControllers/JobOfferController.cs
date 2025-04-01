using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Back_HR.DTOs;
using Back_HR.Models;
using Back_HR.Models.enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                Competences = new List<Competence>() 
            };

            if (dto.Competences != null && dto.Competences.Any())
            {
                foreach (var competence in dto.Competences)
                {
                    // Check if the competence already exists in the database
                    var existingCompetence = await _context.Competences
                        .FirstOrDefaultAsync(c => c.Id == competence.Id);

                    if (existingCompetence != null)
                    {
                        // If it exists, attach it to the JobOffer
                        jobOffer.Competences.Add(existingCompetence);
                    }
                    else
                    {
                        // If it doesn’t exist, treat it as a new competence
                        jobOffer.Competences.Add(competence);
                    }
                }
            }

            // Add to database
            _context.JobOffers.Add(jobOffer);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Job offer created successfully", JobOffer = jobOffer });
        }



        [HttpGet("by-rh/{rhId}")]
        [Authorize(Policy = "RHOnly")]
        public async Task<IActionResult> GetOffersByRHId(Guid rhId)
        {
            // Optional: Verify the RH user exists
            var rhUser = await _userManager.Users.OfType<RH>().FirstOrDefaultAsync(u => u.Id == rhId);
            if (rhUser == null)
            {
                return NotFound($"No RH user found with ID {rhId}.");
            }

            var offers = await _context.JobOffers
                .Where(jo => jo.RHId == rhId)
                .Select(jo => new JobOfferDtoGet
                {
                    Id = jo.Id,
                    Title = jo.Title,
                    Description = jo.Description,
                    Experience = jo.Experience,
                    Salary = jo.Salary,
                    Location = jo.Location,
                    PublishDate = jo.PublishDate,
                    Status = jo.Status,
                    Competences = jo.Competences
                })
                .ToListAsync();

            if (!offers.Any())
            {
                return Ok(new List<JobOfferDtoGet>()); 
            }

            return Ok(offers);
        }

        private async Task<Competence?> GetCompetenceByTitre(string titre)
        {
            if (string.IsNullOrEmpty(titre))
            {
                return null;
            }

            return await _context.Competences
                .FirstOrDefaultAsync(c => c.Titre.ToLower() == titre.ToLower());
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "RHOnly")]
        public async Task<IActionResult> GetOfferById(Guid id)
        {
            var offer = await _context.JobOffers
                .Where(jo => jo.Id == id)
                .Select(jo => new JobOfferDtoGet
                {
                    Id = jo.Id,
                    Title = jo.Title,
                    Description = jo.Description,
                    Experience = jo.Experience,
                    Salary = jo.Salary,
                    Location = jo.Location,
                    PublishDate = jo.PublishDate,
                    Status = jo.Status,
                    Competences = jo.Competences
                })
                .FirstOrDefaultAsync();

            if (offer == null)
            {
                return NotFound($"No job offer found with ID {id}.");
            }

            return Ok(offer);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllOffers()
        {
            var offers = await _context.JobOffers
                .Select(jo => new JobOfferDtoGet
                {
                    Id = jo.Id,
                    Title = jo.Title,
                    Description = jo.Description,
                    Experience = jo.Experience,
                    Salary = jo.Salary,
                    Location = jo.Location,
                    PublishDate = jo.PublishDate,
                    Status = jo.Status,
                    Competences = jo.Competences
                })
                .ToListAsync();

            return Ok(offers);
        }


        [HttpDelete("{id}")]
        [Authorize(Policy = "RHOnly")]
        public async Task<IActionResult> DeleteOffer(Guid id)
        {
            var email = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("Unable to identify user.");
            }

            var rhUser = await _userManager.FindByEmailAsync(email) as RH;
            if (rhUser == null)
            {
                return Unauthorized("RH user not found.");
            }

            var offer = await _context.JobOffers
                .FirstOrDefaultAsync(jo => jo.Id == id);

            if (offer == null)
            {
                return NotFound($"No job offer found with ID {id}.");
            }


            if (offer.RHId != rhUser.Id)
            {
                return Forbid("You can only delete job offers you created.");
            }


            _context.JobOffers.Remove(offer);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Job offer deleted successfully", JobOfferId = id });
        }
    }
}

