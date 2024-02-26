using Servify.Models;

namespace Servify.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Position { get; set; } = String.Empty;
        public decimal Salary { get; set; }
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
