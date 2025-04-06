using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Back_HR.Models;
using Back_HR.DTOs;
using System.ComponentModel.DataAnnotations;
using Back_HR.Models.enums;




[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "RHOnly")]
public class EmployeManagementController : ControllerBase
{
    private readonly HRContext _context;
    private readonly UserManager<User> _userManager;

    public EmployeManagementController(HRContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // CREATE: Add a new employee
    [HttpPost]
    public async Task<ActionResult<EmployeDTO>> PostEmployee([FromBody] CreateEmployeDTO createEmployeDto)
    {
        // Validate model
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if email is already in use
        if (await _userManager.FindByEmailAsync(createEmployeDto.Email) != null)
        {
            return BadRequest("Email is already in use.");
        }

        var employee = new Employe
        {
            Id = Guid.NewGuid(),
            Lastname = createEmployeDto.Lastname,
            Firstname = createEmployeDto.Firstname,
            Telephone = createEmployeDto.Telephone,
            Email = createEmployeDto.Email,
            UserName = createEmployeDto.Email,
            UserType = UserType.EMPLOYE, // Set the user type
            Poste = createEmployeDto.Poste,
            EmailConfirmed = true // You might want to set this based on your workflow
        };

        // Create the user with a temporary password
        var result = await _userManager.CreateAsync(employee, createEmployeDto.TemporaryPassword);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        // Optionally add to role if needed
        // await _userManager.AddToRoleAsync(employee, "Employee");

        var createdEmployeDto = new EmployeDTO
        {
            Id = employee.Id,
            Lastname = employee.Lastname,
            Firstname = employee.Firstname,
            Telephone = employee.Telephone,
            Email = employee.Email,
            Poste = employee.Poste
        };

        return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, createdEmployeDto);
    }

    // READ: Get all employees
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeDTO>>> GetEmployees()
    {
        var employees = await _userManager.Users
            .OfType<Employe>()
            .Select(e => new EmployeDTO
            {
                Id = e.Id,
                Lastname = e.Lastname,
                Firstname = e.Firstname,
                Telephone = e.Telephone,
                Email = e.Email,
                Poste = e.Poste
            })
            .ToListAsync();
        return Ok(employees);
    }

    // READ: Get a single employee by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeDTO>> GetEmployee(Guid id)
    {
        var employee = await _userManager.Users
            .OfType<Employe>()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null) return NotFound();

        var employeDto = new EmployeDTO
        {
            Id = employee.Id,
            Lastname = employee.Lastname,
            Firstname = employee.Firstname,
            Telephone = employee.Telephone,
            Email = employee.Email,
            Poste = employee.Poste
        };
        return Ok(employeDto);
    }

    // UPDATE: Modify an existing employee
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEmployee(Guid id, [FromBody] UpdateEmployeDTO updateEmployeDto)
    {
        var employee = await _userManager.Users
            .OfType<Employe>()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null) return NotFound();

        employee.Lastname = updateEmployeDto.Lastname;
        employee.Firstname = updateEmployeDto.Firstname;
        employee.Telephone = updateEmployeDto.Telephone;
        employee.Poste = updateEmployeDto.Poste;

        // Only update email if it's different
        if (employee.Email != updateEmployeDto.Email)
        {
            employee.Email = updateEmployeDto.Email;
            employee.UserName = updateEmployeDto.Email;
            employee.NormalizedEmail = null;
            employee.NormalizedUserName = null;
        }

        var result = await _userManager.UpdateAsync(employee);
        if (!result.Succeeded) return BadRequest(result.Errors);

        return NoContent();
    }

    // DELETE: Remove an employee
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(Guid id)
    {
        var employee = await _userManager.Users
            .OfType<Employe>()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null) return NotFound();

        var result = await _userManager.DeleteAsync(employee);
        if (!result.Succeeded) return BadRequest(result.Errors);

        return NoContent();
    }
}

// DTOs


public class CreateEmployeDTO
{
    [Required]
    public string Lastname { get; set; }

    [Required]
    public string Firstname { get; set; }

    [Required]
    [Phone]
    public string Telephone { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Poste { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string TemporaryPassword { get; set; }
}

public class UpdateEmployeDTO
{
    [Required]
    public string Lastname { get; set; }

    [Required]
    public string Firstname { get; set; }

    [Required]
    [Phone]
    public string Telephone { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Poste { get; set; }
}