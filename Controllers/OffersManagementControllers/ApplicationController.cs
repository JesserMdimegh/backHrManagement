using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Back_HR.Models;
using Back_HR.DTOs;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Back_HR.Models.enums;

namespace Back_HR.Controllers.OffersManagementControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly HRContext _context;
        private readonly UserManager<User> _userManager;

        public ApplicationController(HRContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("apply")]
        [Authorize(Policy = "CandidatOnly")]
        public async Task<IActionResult> ApplyForJobOffer([FromBody] ApplicationDtoPost dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get the authenticated user's email from the JWT
            var email = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("Unable to identify user.");
            }

            // Find the candidate user
            var candidate = await _userManager.FindByEmailAsync(email) as Candidat;
            if (candidate == null)
            {
                return Unauthorized("Candidate user not found.");
            }

            // Ensure the CandidatId matches the authenticated user
            if (dto.CandidatId != candidate.Id)
            {
                return Forbid("You can only apply for yourself.");
            }

            // Verify the job offer exists and is open
            var jobOffer = await _context.JobOffers
                .FirstOrDefaultAsync(jo => jo.Id == dto.JobOfferId);
            if (jobOffer == null)
            {
                return NotFound($"No job offer found with ID {dto.JobOfferId}.");
            }
            if (jobOffer.Status != OffreStatus.OPEN)
            {
                return BadRequest("This job offer is not open for applications.");
            }

            // Check if the candidate has already applied
            var existingApplication = await _context.Applications
                .AnyAsync(a => a.CandidateId == dto.CandidatId && a.JobOfferId == dto.JobOfferId);
            if (existingApplication)
            {
                return Conflict("You have already applied for this job offer.");
            }

            // Create the application
            var application = new Application
            {
                Id = Guid.NewGuid(),
                CandidateId = dto.CandidatId,
                JobOfferId = dto.JobOfferId,
                Cv = dto.Cv,
                Status = ApplicationStatus.PENDING,
                ApplicationDate = DateTime.Now
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Application submitted successfully", ApplicationId = application.Id });
        }

        [HttpGet("offer/{jobOfferId}/candidates")]
        [Authorize(Policy = "RHOnly")]
        public async Task<IActionResult> GetCandidatesAppliedToOffer(Guid jobOfferId)
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

            var jobOffer = await _context.JobOffers
                .FirstOrDefaultAsync(jo => jo.Id == jobOfferId);
            if (jobOffer == null)
            {
                return NotFound($"No job offer found with ID {jobOfferId}.");
            }
            if (jobOffer.RHId != rhUser.Id)
            {
                return Forbid("You can only view candidates for your own job offers.");
            }

            var applications = await _context.Applications
                .Where(a => a.JobOfferId == jobOfferId)
                .Select(a => new
                {
                    ApplicationId = a.Id,
                    CandidatId = a.CandidateId,
                    a.Cv,
                    a.ApplicationDate
                })
                .ToListAsync();

            return Ok(applications);
        }

        [HttpDelete("{applicationId}/cancel")]
        [Authorize(Policy = "CandidatOnly")]
        public async Task<IActionResult> CancelApplication(Guid applicationId)
        {
            var email = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("Unable to identify user.");
            }

            var candidate = await _userManager.FindByEmailAsync(email) as Candidat;
            if (candidate == null)
            {
                return Unauthorized("Candidate user not found.");
            }

            var application = await _context.Applications
                .FirstOrDefaultAsync(a => a.Id == applicationId);
            if (application == null)
            {
                return NotFound($"No application found with ID {applicationId}.");
            }

            if (application.CandidateId != candidate.Id)
            {
                return Forbid("You can only cancel your own applications.");
            }

            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Application canceled successfully", ApplicationId = applicationId });
        }

        [HttpPut("{applicationId}/accept")]
        [Authorize(Policy = "RHOnly")]
        public async Task<IActionResult> AcceptApplication(Guid applicationId)
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

            var application = await _context.Applications
                .Include(a => a.JobOffer)
                .FirstOrDefaultAsync(a => a.Id == applicationId);
            if (application == null)
            {
                return NotFound($"No application found with ID {applicationId}.");
            }

            if (application.JobOffer.RHId != rhUser.Id)
            {
                return Forbid("You can only accept applications for your own job offers.");
            }

            application.Status = ApplicationStatus.ACCEPTED; 
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Application accepted successfully", ApplicationId = applicationId });
        }

        [HttpPut("{applicationId}/reject")]
        [Authorize(Policy = "RHOnly")]
        public async Task<IActionResult> RejectApplication(Guid applicationId)
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

            var application = await _context.Applications
                .Include(a => a.JobOffer)
                .FirstOrDefaultAsync(a => a.Id == applicationId);
            if (application == null)
            {
                return NotFound($"No application found with ID {applicationId}.");
            }

            if (application.JobOffer.RHId != rhUser.Id)
            {
                return Forbid("You can only reject applications for your own job offers.");
            }

            application.Status = ApplicationStatus.REJECTED;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Application rejected successfully", ApplicationId = applicationId });
        }
    }
}