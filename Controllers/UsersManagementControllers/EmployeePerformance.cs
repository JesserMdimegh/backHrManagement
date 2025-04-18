using Back_HR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Back_HR.DTOs;
using Microsoft.EntityFrameworkCore;



namespace Back_HR.Controllers.UsersManagementControllers
{
    
        [ApiController]
        [Route("api/[controller]")]
        [Authorize(Policy = "RHOnly")]
        public class EmployeePerformance : ControllerBase
        {
            private readonly HRContext _context;

            public EmployeePerformance(HRContext context)
            {
                _context = context;
            }

            [HttpPost]
            public async Task<IActionResult> SubmitReview([FromBody] PerformanceReviewDTO reviewDto)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var review = new PerformanceReview
                {
                    EmployeeId = reviewDto.EmployeeId,
                    ReviewDate = DateTime.UtcNow,
                    TasksCompleted = reviewDto.TasksCompleted,
                    OnTimeCompletionRate = reviewDto.OnTimeCompletionRate,
                    ProcessImprovementIdeas = reviewDto.ProcessImprovementIdeas,
                    Absences = reviewDto.Absences,
                    LateArrivals = reviewDto.LateArrivals,
                    OutputQualityScore = reviewDto.OutputQualityScore,
                    InitiativeScore = reviewDto.InitiativeScore,
                    CommunicationScore = reviewDto.CommunicationScore,
                    ManagerComment = reviewDto.ManagerComment,
                    ClientSatisfactionScore = reviewDto.ClientSatisfactionScore
                };

                review.CalculateOverallScore(); // Auto-compute the final score
                _context.PerformanceReviews.Add(review);
                await _context.SaveChangesAsync();

                return Ok(review); // Return the saved entity (or map to a DTO)
            }

            [HttpGet("{employeeId}")]
            public async Task<ActionResult<List<PerformanceReview>>> GetReviews(Guid employeeId)
            {
                return await _context.PerformanceReviews
                    .Where(r => r.EmployeeId == employeeId)
                    .OrderByDescending(r => r.ReviewDate)
                    .ToListAsync();
            }
        }
    
}
