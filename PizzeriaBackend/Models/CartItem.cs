namespace PizzeriaBackend.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int FoodId { get; set; }
        public string FoodName { get; set; }
        public int Quantity { get; set; }
        public double? Weight { get; set; }
        public decimal Price { get; set; }
        public string Username { get; set; }

        public string? Category { get; set; }
    }
}
