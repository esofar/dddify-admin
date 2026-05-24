using System.Security.Cryptography;
using System.Text;

namespace Dddify.Admin.Application.Services.Sms;

public static class PhoneNumberProtector
{
    public static string Mask(string phoneNumber)
    {
        if (phoneNumber.Length < 7)
        {
            return "****";
        }

        return $"{phoneNumber[..3]}****{phoneNumber[^4..]}";
    }

    public static string Hash(string phoneNumber)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(phoneNumber));

        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
