using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocalFarmersApi.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CategoryId { get; set; }
        public bool Organic { get; set; }
        public CategoryDTO Category { get; set; }
    }

}