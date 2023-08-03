using OtpNet;

namespace Cypher;

public static class CypherModule
{
    public const string Key = "M+1mYAyDeEsfXuXIaRzODCkBdG53q2Tec3ufd/uHf14=";
    public const string Vector = "6k0Ryx6jxV+GMGDXloT2YA==";
    public const string StoreName = "cypher.store";

    public static string GetTotpCode(string secret)
    {
        byte[] secretBytes = Base32Encoding.ToBytes(secret);
        var totp = new Totp(secretBytes);

        return totp.ComputeTotp();
    }

    public static string GetPath()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return Path.Combine(home, StoreName);
    }
}