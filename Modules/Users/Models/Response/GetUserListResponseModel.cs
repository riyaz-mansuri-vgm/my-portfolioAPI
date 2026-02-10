namespace MyPartFolioAPI.Modules.Users.Models.Response;
public class GetUserListResponseModel
{
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public  string? MobileNumber { get; set; }
    public string? UserEmail { get; set; }
    public string? Address { get; set; }
}
