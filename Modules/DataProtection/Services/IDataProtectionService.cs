namespace MyPartFolioAPI.Modules.DataProtection.Services;

public interface IDataProtectionService
{
    string GetProtectedValue<T>(T data);
    T GetPlainValue<T>(string value);
}