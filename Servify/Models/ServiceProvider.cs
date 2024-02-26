namespace Servify.Models
{
    public class ServiceProvider: BaseEntity<int>
    {
        public string Name { get; set; }
        public string ContactInfo { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public double Rating { get; set; }
        public ICollection<Service> Services { get; set; }
    }
}
