namespace PizzeriaBackend.Models.Reviews
{
    public class Review
    {
        public int ReviewId { get; set; }
        public string Name { get; set; }
        public string Topic { get; set; }
        public string Comment { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
