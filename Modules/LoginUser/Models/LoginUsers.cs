using System.ComponentModel.DataAnnotations;

namespace MyPartFolioAPI.Modules.LoginUser.Models;
public class LoginUsers
{
    [Key]
    public int LoginId { get; set; }
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public string? MobileNumber { get; set; }
    public string? Email { get; set; }
    public string? LoginCount { get; set; }
    public string? Password { get; set; }
    public DateTime? Created_At { get; set; }

}
