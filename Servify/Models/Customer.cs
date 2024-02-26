namespace Servify.Models
{
    public class Customer: BaseEntity<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public string Email { get; set; } 
        public string Address { get; set; } 
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}
