using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Back_HR.DTOs;
using Back_HR.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "RHOnly")]
public class UserManagementController : ControllerBase
{
    private readonly HRContext _context;

    public UserManagementController(HRContext context)
    {
        _context = context;
    }

    // READ: Get all users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
    {
        var users = await _context.Users
            .Select(u => new UserDTO
            {
                Id = u.Id,
                Firstname = u.Firstname,
                Lastname = u.Lastname,
                Telephone = u.Telephone,
                Email = u.Email
            })
            .ToListAsync();
        return Ok(users);
    }

    // READ: Get a single user by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDTO>> GetUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        var userDto = new UserDTO
        {
            Id = user.Id,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Telephone = user.Telephone,
            Email = user.Email
        };
        return Ok(userDto);
    }

    // UPDATE: Modify a user
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(Guid id, [FromBody] UserDTO updateUserDto) 
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.Firstname = updateUserDto.Firstname;
        user.Lastname = updateUserDto.Lastname;
        user.Telephone = updateUserDto.Telephone;
        user.Email = updateUserDto.Email;
        user.UserName = updateUserDto.Email; // Sync with IdentityUser

        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: Remove a user
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}