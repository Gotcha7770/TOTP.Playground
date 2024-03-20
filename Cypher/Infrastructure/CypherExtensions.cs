using System.IO.Abstractions;
using System.Security.Cryptography;

namespace Cypher.Infrastructure;

public static class CypherExtensions
{
    public static async Task WriteToFile(this IFileSystem fileSystem, string path, string data)
    {
        if (fileSystem.File.Exists(path))
            fileSystem.File.Delete(path);
        await using Stream stream = EncryptStream(fileSystem.File.OpenWrite(path));
        await using StreamWriter writer = new StreamWriter(stream);

        await writer.WriteAsync(data);
    }

    public static Stream EncryptStream(Stream stream)
    {
        using Aes aes = CreateAes();
        return new CryptoStream(stream, aes.CreateEncryptor(), CryptoStreamMode.Write);
    }

    public static async Task<string> ReadFromFile(this IFileSystem fileSystem, string path)
    {
        await using Stream stream = DecryptStream(fileSystem.File.OpenRead(path));
        using StreamReader reader = new StreamReader(stream);

        return await reader.ReadToEndAsync();
    }

    public static Stream DecryptStream(Stream stream)
    {
        using Aes aes = CreateAes();
        return new CryptoStream(stream, aes.CreateDecryptor(), CryptoStreamMode.Read);
    }

    private static Aes CreateAes()
    {
        Aes aes = Aes.Create();
        aes.Key = Convert.FromBase64String(CypherModule.Key);
        aes.IV = Convert.FromBase64String(CypherModule.Vector);
        return aes;
    }
}