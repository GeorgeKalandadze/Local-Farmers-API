using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocalFarmersApi.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; }
    }
}