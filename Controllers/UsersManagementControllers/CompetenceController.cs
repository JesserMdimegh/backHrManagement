using Back_HR.DTOs;
using Back_HR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Back_HR.Controllers.UsersManagementControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompetenceController : ControllerBase
    {


        private readonly HRContext _context;

        public CompetenceController(HRContext context)
        {
            _context = context;
        }


        [HttpGet("competences")]
        public async Task<IActionResult> GetCompetences()
        {
            var competences = await _context.Competences.ToListAsync();

            return Ok(competences);


        }

        [HttpPost("competences")]
        public async Task<IActionResult> AddCompetence([FromBody] CompetenceDTO createCompetenceDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var competence = new Competence
            {
                Titre = createCompetenceDto.Titre
            };

            _context.Competences.Add(competence);
            await _context.SaveChangesAsync();

            var competenceDto = new CompetenceDTO
            {
                Titre = competence.Titre
            };

            return CreatedAtAction(nameof(GetCompetences), new { id = competence.Id }, competenceDto);
        }

       
    }
}
