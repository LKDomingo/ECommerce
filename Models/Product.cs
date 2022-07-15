#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
// Needed for the [NotMapped] functionality
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models;

public class Product
{
    [Key]
    public int ProductId {get;set;}

    [Required]
    [MinLength(5)]
    public string Name {get;set;}

    [Required]
    public string ImageURL {get;set;}

    [Required]
    [MinLength(5)]
    public string Description {get;set;}

    [Required]
    [Range(0, int.MaxValue)]
    public int Quantity {get;set;}

    [Required]
    [Range(0, int.MaxValue)]
    public int Price {get;set;}

    public DateTime CreatedAt {get;set;} = DateTime.Now;
    
    public DateTime UpdatedAt {get;set;} = DateTime.Now;
    
    public List<Cart> ProductOrders {get;set;} = new List<Cart>();
}