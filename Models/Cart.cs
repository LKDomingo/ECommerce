#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
// Needed for the [NotMapped] functionality
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models;

public class Cart
{
    [Key]
    public int CartId {get;set;}

    public int UserId {get;set;}
    
    public User? User {get;set;}

    public int ProductId {get;set;}

    public Product? Product {get;set;}
}