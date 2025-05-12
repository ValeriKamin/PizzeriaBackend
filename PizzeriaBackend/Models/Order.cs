namespace PizzeriaBackend.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public string DeliveryType { get; set; } 
        public string Address { get; set; }
        public string Apartment { get; set; }
        public string Entrance { get; set; }
        public string Floor { get; set; }
        public string DoorCode { get; set; }
        public string CourierComment { get; set; }


        public string DeliveryTime { get; set; }


        public string CardNumber { get; set; }
        public string CVM { get; set; }
        public string Expiry { get; set; }

        public decimal Total { get; set; }
        public string Status { get; set; } 
        public DateTime CreatedAt { get; set; }
    }

    public class OrderWithItems
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public decimal Total { get; set; }
        public List<OrderItem> Items { get; set; }
    }

    public class OrderItem
    {
        public string FoodName { get; set; }
        public int Quantity { get; set; }
    }
}
