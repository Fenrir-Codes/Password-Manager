using System.Security.Cryptography;
using System.Text;

namespace PasswordManager.Services;

public static class EncryptionService
{
    public static string Encrypt(string text)
    {
        byte[] data =
            Encoding.UTF8.GetBytes(text);

        byte[] encrypted =
            ProtectedData.Protect(
                data,
                null,
                DataProtectionScope.CurrentUser);

        return Convert.ToBase64String(
            encrypted);
    }

    public static string Decrypt(string text)
    {
        byte[] data =
            Convert.FromBase64String(text);

        byte[] decrypted =
            ProtectedData.Unprotect(
                data,
                null,
                DataProtectionScope.CurrentUser);

        return Encoding.UTF8.GetString(
            decrypted);
    }
}