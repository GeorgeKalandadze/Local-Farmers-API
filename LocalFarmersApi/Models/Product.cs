using System;
using System.ComponentModel.DataAnnotations;

public class Product
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be 0 or greater")]
    public int StockQuantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CategoryId { get; set; }

    public virtual Category Category { get; set; }

    public bool Organic { get; set; }  
}
