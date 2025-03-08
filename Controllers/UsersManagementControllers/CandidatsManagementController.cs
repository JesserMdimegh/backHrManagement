using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Back_HR.DTOs;
using Back_HR.Models;
using Microsoft.AspNetCore.Identity;

[ApiController]
[Route("api/[controller]")] 
[Authorize(Policy = "RHOnly")]
public class CandidatManagementController : ControllerBase
{
    private readonly HRContext _context;
    private readonly UserManager<User> _userManager;

    public CandidatManagementController(HRContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // READ: Get all candidats
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CandidatDTO>>> GetCandidats()
    {
        var candidats = await _context.Candidates
            .Select(c => new CandidatDTO
            {
                Id = c.Id,
                Lastname = c.Lastname,
                Firstname = c.Firstname,
                Telephone = c.Telephone,
                Email = c.Email,
                Cv = c.Cv
            })
            .ToListAsync();
        return Ok(candidats);
    }

    // READ: Get a single candidat by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<CandidatDTO>> GetCandidat(Guid id)
    {
        var candidat = await _context.Candidates
            .FirstOrDefaultAsync(c => c.Id == id);

        if (candidat == null) return NotFound();

        var candidatDto = new CandidatDTO
        {
            Id = candidat.Id,
            Lastname = candidat.Lastname,
            Firstname = candidat.Firstname,
            Telephone = candidat.Telephone,
            Email = candidat.Email,
            Cv = candidat.Cv
        };
        return Ok(candidatDto);
    }

    // DELETE: Remove a candidat
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCandidat(Guid id)
    {
        var candidat = await _context.Candidates
            .FirstOrDefaultAsync(c => c.Id == id);

        if (candidat == null) return NotFound();

        var result = await _userManager.DeleteAsync(candidat);
        if (!result.Succeeded) return BadRequest(result.Errors);

        return NoContent();
    }
}