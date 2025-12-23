using System;
using System.Security.Cryptography;

public class OTPGenerator
{
    public static string Generate6DigitOTP()
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] bytes = new byte[4]; // 32 bits
            rng.GetBytes(bytes);
            int value = BitConverter.ToInt32(bytes, 0) & 0x7FFFFFFF; // ensure positive
            int otp = value % 1000000; // limit to 6 digits
            return otp.ToString("D6"); // pad with leading zeros if needed
        }
    }
}
