namespace Servify.Models
{
    public class Review: BaseEntity<int>
    {
        public int CustomerId { get; set; }
        public int ServiceId { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public DateTime ReviewDate { get; set; }
        public Customer Customer { get; set; }
        public Service Service { get; set; }
    }
}
