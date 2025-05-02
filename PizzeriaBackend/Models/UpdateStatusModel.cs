namespace PizzeriaBackend.Models
{
    public class UpdateStatusModel
    {
        public int OrderId { get; set; }
        public string NewStatus { get; set; }
    }
}