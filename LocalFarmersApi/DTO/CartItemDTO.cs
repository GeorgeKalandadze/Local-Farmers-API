using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocalFarmersApi.DTO
{
    public class CartItemDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
    }
}