using Pizzeria.Models;
using PizzeriaBackend.Models.Cart;

namespace PizzeriaBackend.Models.Orders
{

    public class Order
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string DeliveryType { get; set; } 
        public string Address { get; set; }
        public string Apartment { get; set; }
        public string Entrance { get; set; }
        public string Floor { get; set; }
        public string DoorCode { get; set; }
        public string CourierComment { get; set; }
        public int UserId { get; set; }

        public string DeliveryTime { get; set; }
        public string CardNumber { get; set; }
        public string CVM { get; set; }
        public string Expiry { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } 
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
        public List<CartItem> Items { get; set; }
    }

    public class OrderWithItems
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string DeliveryType { get; set; }
        public string Address { get; set; }
        public string Apartment { get; set; }
        public string Entrance { get; set; }
        public string Floor { get; set; }
        public string DoorCode { get; set; }
        public string CourierComment { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItem> Items { get; set; }
        public string DeliveryTime { get; set; }

        public int UserId { get; set; }



    }



    public class OrderItem
    {
        public string FoodName { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; set; }
        public int FoodId { get; set; }

        public User User { get; set; }
        public List<CartItem> Items { get; set; }
    }
}
