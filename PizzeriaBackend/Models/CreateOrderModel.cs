namespace PizzeriaBackend.Models
{
    public class CreateOrderModel
    {
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
    }
}

