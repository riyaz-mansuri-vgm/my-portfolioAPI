namespace MyPartFolioAPI.Modules.Users.Models.Request;

public class UpdateUseDetailRequestModel
{
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? MobileNumber { get; set; }
    public string? Address { get; set; }
}
