namespace Servify.Models
{
    public class Booking : BaseEntity<int>
    {
        public int CustomerId { get; set; }
        public int ServiceId { get; set; } = 0;
        public DateTime BookingDate { get; set; }
        public TimeSpan BookingTime { get; set; }
        public Customer Customer { get; set; }
        public Service Service { get; set; }
    }
}
