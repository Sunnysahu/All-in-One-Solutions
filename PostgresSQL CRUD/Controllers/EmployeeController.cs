using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostgresSQL_CRUD.Data;
using PostgresSQL_CRUD.Models;

namespace PostgresSQL_CRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context) => _context = context;

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] Employee employee)
        {
            _context.Employees.Add(employee);

            await _context.SaveChangesAsync();

            return Ok(employee);
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _context.Employees.ToListAsync();

            return Ok(employees);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] Employee model)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound();

            employee.Name = model.Name;
            employee.Email = model.Email;
            employee.Salary = model.Salary;
            employee.Department = model.Department;

            await _context.SaveChangesAsync();

            return Ok(employee);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound();

            _context.Employees.Remove(employee);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
