#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
// Needed for the [NotMapped] functionality
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models;

public class User
{
    [Key]
    public int UserId {get;set;}

    [Required]
    [MinLength(2)]
    public string FirstName {get;set;}

    [Required]
    [MinLength(2)]
    public string LastName {get;set;}

    [EmailAddress]
    [Required]
    public string Email {get;set;}

    [Required]
    [MinLength(8)]
    [DataType(DataType.Password)]
    public string Password {get;set;}

    // Anything under the NotMapped will not go in to the database
    [NotMapped]
    [Compare("Password")]
    [DataType(DataType.Password)]
    public string PassConfirm {get;set;}

    public DateTime CreatedAt {get;set;} = DateTime.Now;
    
    public DateTime UpdatedAt {get;set;} = DateTime.Now;

    public List<Cart> OrderItems {get;set;} = new List<Cart>();
}