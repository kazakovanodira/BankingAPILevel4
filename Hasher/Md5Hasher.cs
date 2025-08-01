using System.Security.Cryptography;
using System.Text;

namespace banking_api_repo.Hasher;

public static class Md5Hasher
{
    public static string ComputeHash(string input)
    {
        using var md5 = MD5.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        var sb = new StringBuilder();
        foreach (var b in hashBytes)
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }
}
