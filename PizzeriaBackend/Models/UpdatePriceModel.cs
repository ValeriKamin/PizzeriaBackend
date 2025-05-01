namespace PizzeriaBackend.Models
{
    public class UpdatePriceModel
    {
        public int FoodId { get; set; }
        public decimal NewPrice { get; set; }
    }
}