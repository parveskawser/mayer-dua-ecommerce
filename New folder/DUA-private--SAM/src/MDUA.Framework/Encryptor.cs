using System;
using System.Text;

public class ReversibleEncryptor
{
    private const string Base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private readonly int rounds;
    private readonly int[] roundKeys;

    public ReversibleEncryptor(string privateKey, int rounds = 4)
    {
        this.rounds = rounds;
        this.roundKeys = GenerateRoundKeys(privateKey, rounds);
    }

    public string Encrypt(long number)
    {
        // Limit input to max 8-digit range (adjustable)
        if (number < 0 || number > 9999999999)
            throw new ArgumentOutOfRangeException("Input must be a non-negative number within 10-digit range.");

        ulong value = (ulong)number;
        uint left = (uint)(value >> 24);
        uint right = (uint)(value & 0xFFFFFF);

        for (int i = 0; i < rounds; i++)
        {
            uint temp = left;
            left = right ^ F(left, roundKeys[i]);
            right = temp;
        }

        ulong encrypted = ((ulong)left << 24) | right;
        return ToBase62(encrypted).PadLeft(10, '0');
    }

    public long Decrypt(string encryptedText)
    {
        ulong encoded = FromBase62(encryptedText);

        uint left = (uint)(encoded >> 24);
        uint right = (uint)(encoded & 0xFFFFFF);

        for (int i = rounds - 1; i >= 0; i--)
        {
            uint temp = right;
            right = left ^ F(right, roundKeys[i]);
            left = temp;
        }

        ulong original = ((ulong)left << 24) | right;
        return (long)original;
    }

    private int[] GenerateRoundKeys(string key, int count)
    {
        int[] keys = new int[count];
        int seed = key.GetHashCode();
        Random rand = new Random(seed);

        for (int i = 0; i < count; i++)
        {
            keys[i] = rand.Next();
        }

        return keys;
    }

    private uint F(uint x, int key)
    {
        return (uint)(((x ^ key) + ((x << 5) | (x >> 3))) & 0xFFFFFF);
    }

    private string ToBase62(ulong value)
    {
        var sb = new StringBuilder();
        do
        {
            sb.Insert(0, Base62Chars[(int)(value % 62)]);
            value /= 62;
        } while (value > 0);
        return sb.ToString();
    }

    private ulong FromBase62(string input)
    {
        ulong result = 0;
        foreach (char c in input)
        {
            result = result * 62 + (ulong)Base62Chars.IndexOf(c);
        }
        return result;
    }
}
