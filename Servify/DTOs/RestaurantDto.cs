using Servify.Models;

namespace Servify.DTOs
{
    public class RestaurantDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Location { get; set; } = String.Empty;
        public List<EmployeeDto>? Employees { get; set; } = null;

        public static RestaurantDto MapToDto(Restaurant restaurant)
        {
            return new RestaurantDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Location = restaurant.Location,
                Employees = restaurant.Employees.Select(EmployeeDto.MapToDto).ToList()
            };
        }
    }
}
