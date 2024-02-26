namespace Servify.DTOs
{
    public class RestaurantDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Location { get; set; } = String.Empty;
        public List<EmployeeDto> Employees { get; set; } = new List<EmployeeDto>();
    }
}
