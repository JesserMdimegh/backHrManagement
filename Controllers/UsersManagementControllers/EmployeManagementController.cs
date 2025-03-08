using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Back_HR.Models;
using Back_HR.DTOs;

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

    // READ: Get all employees
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeDTO>>> GetEmployees()
    {
        var employees = await _context.Employees
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
        var employee = await _context.Employees
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
    public async Task<IActionResult> PutEmployee(Guid id, [FromBody] EmployeDTO updateEmployeDto)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null) return NotFound();

        employee.Lastname = updateEmployeDto.Lastname;
        employee.Firstname = updateEmployeDto.Firstname;
        employee.Telephone = updateEmployeDto.Telephone;
        employee.Email = updateEmployeDto.Email;
        employee.UserName = updateEmployeDto.Email;
        employee.Poste = updateEmployeDto.Poste;

        var result = await _userManager.UpdateAsync(employee);
        if (!result.Succeeded) return BadRequest(result.Errors);

        return NoContent();
    }

    // DELETE: Remove an employee
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(Guid id)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null) return NotFound();

        var result = await _userManager.DeleteAsync(employee);
        if (!result.Succeeded) return BadRequest(result.Errors);

        return NoContent();
    }
}