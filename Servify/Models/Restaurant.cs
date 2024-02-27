using System.ComponentModel.DataAnnotations;

namespace Servify.Models
{
    public class Restaurant : BaseEntity<int>
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = String.Empty;

        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; } = String.Empty;

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
