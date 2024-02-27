using Servify.Models;
using System.ComponentModel.DataAnnotations;

namespace Servify.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name must be between 1 and 100 characters.", MinimumLength = 1)]
        public string Name { get; set; } = String.Empty;


        [Required(ErrorMessage = "Position is required.")]
        [StringLength(50, ErrorMessage = "Position must be between 1 and 50 characters.", MinimumLength = 1)]
        public string Position { get; set; } = String.Empty;

        [Required(ErrorMessage = "Salary is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Salary must be a positive value.")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Restaurant ID is required.")]
        public int restaurantId { get; set; }

        public static EmployeeDto MapToDto(Employee employee)
        {
            return new EmployeeDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Position = employee.Position,
                Salary = employee.Salary
            };
        }
    }
}
