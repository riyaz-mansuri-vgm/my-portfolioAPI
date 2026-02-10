using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace MyPartFolioAPI.Modules.DataProtection.Services;

public class DataProtectionService : IDataProtectionService
{
    public IDataProtectionProvider DataProtectionProvider { get; }

    private static readonly string Key = "b14ca5898a4e4133bbce2ea2315a2001";
    private static readonly string IV = "YWJjZGVmMTIzNDU2Nzg5YQ==";

    public string GetProtectedValue<T>(T data)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Convert.FromBase64String(Key);
            aesAlg.IV = Convert.FromBase64String(IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        string jsonString = JsonConvert.SerializeObject(data);
                        swEncrypt.Write(jsonString);
                    }
                }

                return Convert.ToHexString(msEncrypt.ToArray());
            }
        }
    }

    public T GetPlainValue<T>(string encryptedHex)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Convert.FromBase64String(Key);
            aesAlg.IV = Convert.FromBase64String(IV);

            byte[] encryptedBytes = Convert.FromHexString(encryptedHex);

            using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        string jsonString = srDecrypt.ReadToEnd();
                        return JsonConvert.DeserializeObject<T>(jsonString);
                    }
                }
            }
        }
    }
}