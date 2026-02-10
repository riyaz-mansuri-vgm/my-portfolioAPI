using System.ComponentModel.DataAnnotations;

namespace MyPartFolioAPI.Modules.Users.Models;
public class Users
{
    [Key]
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public string? MobileNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Password { get; set; }
    public DateTime? Created_At { get; set; }
    public DateTime? Update_Date { get; set; }
    public bool? Status { get; set; }
}
