using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pagination.Data;
using Pagination.Models;

namespace Pagination.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PaginationController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PaginationController(AppDbContext context) => _context = context;

        [HttpGet("offset")]
        public async Task<IActionResult> GetOffsetPagination(int pageNumber = 1, int pageSize = 10)
        {
            var employees = await _context.EmployeeModels
                .OrderBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(employees);
        }

        [HttpGet("cursor")]
        public async Task<IActionResult> GetCursorPagination(long? lastId, int pageSize)
        {
            IQueryable<EmployeeModel> query = _context.EmployeeModels;

            if (lastId.HasValue) query = query.Where(x => x.Id > lastId.Value);

            var employees = await query
                .OrderBy(x => x.Id)
                .Take(pageSize)
                .ToListAsync();

            return Ok(employees);
        }
    }
}