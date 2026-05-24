using Dddify.Admin.Application.Services.Security;
using System.Security.Cryptography;

namespace Dddify.Admin.Infrastructure.Security;

[SingletonDependency(RegistrationMode.AsImplementedInterfaces)]
public class PasswordGenerator : IPasswordGenerator
{
    private const int MinPasswordLength = 8;
    private const int MaxPasswordLength = 16;
    private const string UppercaseLetters = "ABCDEFGHJKLMNPQRSTUVWXYZ";
    private const string LowercaseLetters = "abcdefghijkmnopqrstuvwxyz";
    private const string Digits = "23456789";
    private const string SpecialCharacters = "!@#$%^&*";

    public string Generate(int length = 12, bool includeSpecialCharacters = true)
    {
        if (length is < MinPasswordLength or > MaxPasswordLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(length),
                $"Password length must be between {MinPasswordLength} and {MaxPasswordLength}.");
        }

        var passwordChars = new List<char>
        {
            GetRandomChar(UppercaseLetters),
            GetRandomChar(LowercaseLetters),
            GetRandomChar(Digits),
        };

        if (includeSpecialCharacters)
        {
            passwordChars.Add(GetRandomChar(SpecialCharacters));
        }

        var allCharacters = UppercaseLetters + LowercaseLetters + Digits
            + (includeSpecialCharacters ? SpecialCharacters : string.Empty);

        while (passwordChars.Count < length)
        {
            passwordChars.Add(GetRandomChar(allCharacters));
        }

        Shuffle(passwordChars);

        return new string(passwordChars.ToArray());
    }

    private static char GetRandomChar(string source)
    {
        return source[RandomNumberGenerator.GetInt32(source.Length)];
    }

    private static void Shuffle(IList<char> chars)
    {
        for (var i = chars.Count - 1; i > 0; i--)
        {
            var j = RandomNumberGenerator.GetInt32(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }
    }
}
