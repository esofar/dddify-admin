using System.Buffers.Binary;
using System.Security.Cryptography;

namespace Dddify.Admin.Infrastructure.Services;

[SingletonDependency(RegistrationMode.AsImplementedInterfaces)]
public class ShortCodeGenerator : IShortCodeGenerator
{
    private const string Alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const int Base = 62;

    public string Generate(Guid id, int length = 8)
    {
        if (length <= 0 || length > 11)
            throw new ArgumentOutOfRangeException(nameof(length));

        var hash = SHA256.HashData(id.ToByteArray());
        ulong value = BinaryPrimitives.ReadUInt64BigEndian(hash.AsSpan(0, 8));

        var chars = new char[length];
        for (int i = length - 1; i >= 0; i--)
        {
            chars[i] = Alphabet[(int)(value % Base)];
            value /= Base;
        }

        return new string(chars);
    }
}