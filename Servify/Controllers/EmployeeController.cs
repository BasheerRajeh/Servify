﻿using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> Get()
        {
            var employees = await _context.Employees.ToListAsync();

            if (employees == null || !employees.Any())
            {
                return NotFound();
            }

            var employeesDto = employees.Select(EmployeeDto.MapToDto).ToList();

            return employeesDto;
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