namespace PizzeriaBackend.Models.Cart
{
    public class AddToCartModel
    {
        public int FoodId { get; set; }
        public int Quantity { get; set; }
        public string Username { get; set; }
    }
}
