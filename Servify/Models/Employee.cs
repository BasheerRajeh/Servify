namespace Servify.Models
{
    public class Employee : BaseEntity<int>
    {
        public string Name { get; set; } = String.Empty;
        public string Position { get; set; } = String.Empty;
        public decimal Salary { get; set; }
        public int RestaurantId { get; set; }
        public Restaurant? Restaurant { get; set; }
    }
}
