namespace Servify.Models
{
    public class Restaurant: BaseEntity<int>
    {
        public string Name { get; set; } = String.Empty;
        public string Location { get; set; } = String.Empty;
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();

    }
}
