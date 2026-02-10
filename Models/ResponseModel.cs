namespace MyPartFolioAPI.Models;

public class ResponseModel
{
    public int Status { get; set; } = (int)ResponseCode.NoContent;
    public string Message { get; set; } = string.Empty;
    public dynamic Data { get; set; } = new { };
}
