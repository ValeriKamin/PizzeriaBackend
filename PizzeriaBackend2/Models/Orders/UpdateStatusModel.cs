namespace PizzeriaBackend.Models.Orders
{
    public class UpdateStatusModel
    {
        public int OrderId { get; set; }
        public string NewStatus { get; set; }
    }
}