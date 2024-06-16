using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Category
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string Description { get; set; }

    public virtual ICollection<Product> Products { get; set; }
}
