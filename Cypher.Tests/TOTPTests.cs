using System.Text;
using FluentAssertions;
using OtpNet;

namespace Cypher.Tests;

public class TOTPTests
{
    // https://github.com/google/google-authenticator/wiki/Key-Uri-Format
    // https://datatracker.ietf.org/doc/id/draft-mraihi-totp-timebased-06.html

    [Fact]
    public void TestVector()
    {
        const string rfc6238SecretSha1 = "12345678901234567890";
        byte[] keyData = Encoding.UTF8.GetBytes(rfc6238SecretSha1);
        var time = DateTimeOffset.FromUnixTimeSeconds(59).DateTime;
        var totp = new Totp(keyData, totpSize: 8);

        totp.ComputeTotp(time).Should()
            .Be("94287082");

        totp.ComputeTotp().Should()
            .Be("94287082");
    }

    [Fact]
    public void GetCode()
    {
        byte[] keyData = Base32Encoding.ToBytes("PEPTHLLCKWFSFJVCMX7QNHITRM2PDS3G");
        var totp = new Totp(keyData);

        string result = totp.ComputeTotp();
    }
}