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
        public async Task<IActionResult> GetCursorPagination(long? lastId, int pageSize = 10)
        {

            //if (lastId == null) return NotFound(new { Error = "Id Not Found" });

            // IQueryable itself uses Await internally
            //IQueryable<EmployeeModel> query = _context.EmployeeModels.Where(x => x.Id > lastId).Take(pageSize);

            //return Ok(new
            //{
            //    Data = query,
            //    Message = "Success"
            //})
         
            // Typical EF Core Pattern --> query stays flexible --> SQL stays optimized --> execution happens only once

            IQueryable<EmployeeModel> query = _context.EmployeeModels.AsNoTracking();

            if (lastId < 0) return NotFound(new { Message = "No Data Ahead or lastId is Invalid" });

            if (lastId.HasValue) query = query.Where(x => x.Id > lastId.Value);

            var employees = await query
                .OrderBy(x => x.Id)
                .Take(pageSize)
                .ToListAsync();

            var countTask = await query.CountAsync();
            var salaryTask = await query.SumAsync(x => x.Salary);

            return Ok(new
            {
                Data = employees,
                Count = countTask,
                //Salary = salaryTask.Result
            });

            
            /*
            // EF a single DbContext instance and a single database connection, so Parallel Queries can't be executed.
            // Either Run the individual await query or

            // DbContextFactory (true parallel)

            //Use : IDbContextFactory -->services.AddDbContextFactory<AppDbContext>();
            //Use : var context1 = factory.CreateDbContext();
            //var context2 = factory.CreateDbContext();

            //var t1 = context1.EmployeeModels.ToListAsync();
            //var t2 = context2.EmployeeModels.CountAsync();

            //await Task.WhenAll(t1, t2);

            // To Do this 
            await Task.WhenAll(employees, countTask, salaryTask);

            return Ok(new
            {
                Data = employees,
                Count = countTask.Result,
                Salary = salaryTask.Result
            });

            */


            // Too Late 300ms +

            //IQueryable<EmployeeModel> query = _context.EmployeeModels.AsNoTracking();

            //if (query.Count() < lastId || lastId < 0 || lastId is null) return NotFound(new { Message = "No Data Ahead or lastId is Invalid" });
                
            //if (lastId.HasValue) query = query.Where(x => x.Id > lastId.Value);

            //var employees = await query.OrderBy(x => x.Id).Take(pageSize).ToListAsync();

            //var count = employees.Count();

            //var salary = employees.Sum(x => x.Salary);
            //return Ok(new
            //{
            //    Data = employees,
            //    Count = count,
            //    Salary = salary
            //});

            // single query

            //var result = await _context.EmployeeModels
            //.Where(x => lastId == null || x.Id > lastId)
            //.GroupBy(x => 1)
            //.Select(g => new
            //{
            //    Data = g
            //        .OrderBy(x => x.Id)
            //        .Take(pageSize)
            //        .ToList(),

            //    Count = g.Count(),
            //    Salary = g.Sum(x => x.Salary)
            //})
            //.FirstOrDefaultAsync();

        }
    }
}