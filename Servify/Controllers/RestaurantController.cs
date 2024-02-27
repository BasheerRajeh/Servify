using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Servify.Data;
using Servify.DTOs;
using Servify.Models;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Servify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly ServifyDbContext _context;
        private readonly ILogger<RestaurantController> _logger;

        public RestaurantController(ServifyDbContext context, ILogger<RestaurantController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/<Restaurant>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RestaurantDto>>> Get()
        {
            var restaurants = await _context.Restaurants.Include(_ => _.Employees).ToListAsync();

            if (restaurants == null || !restaurants.Any())
            {
                return NotFound();
            }

            var restaurantDtos = restaurants.Select(RestaurantDto.MapToDto).ToList();
            return restaurantDtos;

        }

        // GET api/<Restaurant>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RestaurantDto>> Get(int id)
        {
            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == id);

            if (restaurant == null)
            {
                return NotFound();
            }

            var restaurantDto = RestaurantDto.MapToDto(restaurant);
            return restaurantDto;
        }

        // POST api/<Restaurant>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] RestaurantDto restaurantDto)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState.Values
                       .SelectMany(v => v.Errors)
                       .Select(e => e.ErrorMessage)
                       .FirstOrDefault();

                _logger.LogError($"Invalid data");
                return BadRequest(new Response
                {
                    Status = "Error",
                    Message = errorMessage ?? "Invalid employee data",
                });
            }

            var restaurant = new Restaurant
            {
                Name = restaurantDto.Name,
                Location = restaurantDto.Location,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            };

            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"${restaurant.Name} Restaurant created successfully");
            return CreatedAtAction(nameof(Get), new { id = restaurant.Id }, restaurant);

        }

        // PUT api/<Restaurant>/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, [FromBody] RestaurantDto restaurantDto)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState.Values
                       .SelectMany(v => v.Errors)
                       .Select(e => e.ErrorMessage)
                       .FirstOrDefault();

                return BadRequest(new Response
                {
                    Status = "Error",
                    Message = errorMessage ?? "Invalid employee data",
                });
            }

            if (id != restaurantDto.Id)
            {
                return BadRequest("Mismatched id in request body and URI");
            }

            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            restaurant.Name = restaurantDto.Name;
            restaurant.Location = restaurantDto.Location;
            restaurant.UpdateDate = DateTime.Now;

            _context.Entry(restaurant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"${restaurant} restaurant has been updated");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantExists(id))
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

        // DELETE api/<Restaurant>/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            _context.Restaurants.Remove(restaurant);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"${restaurant} restaurant has been removed");
            return NoContent();
        }
        
        private bool RestaurantExists(int id)
        {
            return _context.Restaurants.Any(r => r.Id == id);
        }

    }
}
