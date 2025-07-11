﻿using PizzeriaBackend.Models.Cart;
using PizzeriaBackend.Models.Reviews;

namespace PizzeriaBackend.Models.Menu
{
    public class Food
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Quantity { get; set; }
        public double? Weight { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }

    }
}
