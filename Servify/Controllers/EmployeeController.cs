using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Servify.Data;
using Servify.DTOs;
using Servify.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Servify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ServifyDbContext _context;

        public EmployeeController(ServifyDbContext context)
        {
            _context = context;
        }


        // GET: api/<EmployeeController>
        [HttpGet]
        public ActionResult<IEnumerable<EmployeeDto>> Get(
            [FromQuery] decimal? minSalary,
            [FromQuery] decimal? maxSalary,
            [FromQuery] string? position,
            [FromQuery] int? restaurantId,
            [FromQuery] string? name, 
            [FromQuery] string? sortBy = "name",
            [FromQuery] string sortOrder = "asc")
        {

            IQueryable<Employee> query = _context.Employees;

            // Filter by minimum salary if provided
            if (minSalary.HasValue)
            {
                query = query.Where(e => e.Salary >= minSalary);
            }

            // Filter by maximum salary if provided
            if (maxSalary.HasValue)
            {
                query = query.Where(e => e.Salary <= maxSalary);
            }

            // Filter by position if provided
            if (!string.IsNullOrEmpty(position))
            {
                query = query.Where(e => e.Position == position);
            }

            // Filter by restaurantId if provided
            if (restaurantId.HasValue)
            {
                query = query.Where(e => e.RestaurantId == restaurantId);
            }

            // Filter by name if provided
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => e.Name.Contains(name));
            }

            // Sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "salary":
                        query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(e => e.Salary) : query.OrderBy(e => e.Salary);
                        break;
                    case "name":
                        query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(e => e.Name) : query.OrderBy(e => e.Name);
                        break;
                    default:
                        break;
                }
            }

            var employees = query.Select(EmployeeDto.MapToDto).ToList();

            if (employees == null || !employees.Any())
            {
                return NotFound();
            }

            return Ok(employees);

        }

        // GET api/<EmployeeController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> Get(int id)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            var employeeDto = EmployeeDto.MapToDto(employee);
            return employeeDto;
        }

        // POST api/<EmployeeController>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] EmployeeDto employeeDto)
        {

            if (employeeDto == null)
            {
                return BadRequest("Employee data is null");
            }

            var restaurant = await _context.Restaurants.FindAsync(employeeDto.restaurantId);
            if (restaurant == null)
            {
                return BadRequest("Invalid RestaurantId");
            }

            var employee = new Employee
            {
                Name = employeeDto.Name,
                Position = employeeDto.Position,
                Salary = employeeDto.Salary,
                RestaurantId = employeeDto.restaurantId,
                Restaurant = restaurant,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            };

            restaurant.Employees.Add(employee);

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = employee.Id }, employee);
        }

        // PUT api/<EmployeeController>/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, [FromBody] EmployeeDto employeeDto)
        {
            if (id != employeeDto.Id)
            {
                return BadRequest("Id in URL does not match Id in body");
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants.FindAsync(employeeDto.restaurantId);
            if (restaurant == null)
            {
                return BadRequest("Invalid RestaurantId");
            }

            employee.Name = employeeDto.Name;
            employee.Position = employeeDto.Position;
            employee.Salary = employeeDto.Salary;
            employee.RestaurantId = employeeDto.restaurantId;
            employee.Restaurant = restaurant;
            employee.UpdateDate = DateTime.Now;

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE api/<EmployeeController>/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
