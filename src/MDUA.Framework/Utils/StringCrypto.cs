using System;
using System.IO;
using System.Security.Cryptography;

namespace MDUA.Framework.Utils
{

    /// <summary>
    /// Summary description for StringCrypto.
    /// </summary>
    public class StringCrypto
    {
        private static TripleDESCryptoServiceProvider clientDESCryptoServiceProvider;

        static StringCrypto()
        {
            clientDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
            clientDESCryptoServiceProvider.Key = new byte[] { 171, 21, 182, 52, 131, 222, 121, 82, 28, 12, 2, 12, 32, 18, 11, 1 };
            clientDESCryptoServiceProvider.IV = new byte[] { 102, 181, 11, 22, 212, 213, 42, 32 };
        }

        /// <summary>
        /// Base64 format for HTML view. It replaces + and / with other character
        /// </summary>
        /// <param name="normalString"></param>
        /// <returns></returns>
        public static string ToBase64(string normalString)
        {
            byte[] encodedBytes = System.Text.ASCIIEncoding.UTF8.GetBytes(normalString);

            return ToBase64(encodedBytes);
        }
        public static string ToBase64(byte[] normalBytes)
        {
            string encodedString = Convert.ToBase64String(normalBytes);

            encodedString = encodedString.Replace('+', '.');
            encodedString = encodedString.Replace('/', '_');
            encodedString = encodedString.Replace('=', '-');

            return encodedString;
        }
        public static byte[] FromBase64Bytes(string base64String)
        {
            base64String = base64String.Replace('.', '+');
            base64String = base64String.Replace('_', '/');
            base64String = base64String.Replace('-', '=');

            byte[] decodedBytes = Convert.FromBase64String(base64String);
            return decodedBytes;
        }
        public static string FromBase64(string base64String)
        {
            byte[] decodedBytes = FromBase64Bytes(base64String);
            string decodedString = System.Text.ASCIIEncoding.UTF8.GetString(decodedBytes);
            return decodedString;
        }

        public static string Decrypt(string AString)
        {
            byte[] encryptedData;
            MemoryStream dataStream = null;
            CryptoStream encryptedStream = null;

            if (0 == AString.Length)
                return string.Empty;
            // Get the byte data
            encryptedData = FromBase64Bytes(AString); //Convert.FromBase64String(AString);

            try
            {
                dataStream = new MemoryStream();
                try
                {
                    // Create decryptor and stream
                    ICryptoTransform decryptor;
                    decryptor = clientDESCryptoServiceProvider.CreateDecryptor();
                    encryptedStream = new CryptoStream(dataStream, decryptor, CryptoStreamMode.Write);

                    // Write the decrypted data to the memory stream
                    encryptedStream.Write(encryptedData, 0, encryptedData.Length - 1);
                    encryptedStream.FlushFinalBlock();

                    // Position back at start
                    dataStream.Position = 0;
                    int size = (int)dataStream.Length;

                    // Create area for data
                    encryptedData = new byte[size];

                    // Read decrypted data to byte()
                    dataStream.Read(encryptedData, 0, size);

                    return System.Text.ASCIIEncoding.UTF8.GetString(encryptedData, 0, size);
                }
                finally
                {
                    encryptedStream.Close();
                }
            }
            finally
            {
                dataStream.Close();
            }


        }
        public static string Encrypt(string AString)
        {
            if (AString == string.Empty)
            {
                return AString;
            }
            else
            {

                MemoryStream dataStream = new MemoryStream();
                ICryptoTransform encryptor = clientDESCryptoServiceProvider.CreateEncryptor();

                try
                {
                    //Create the encrypted stream
                    CryptoStream encryptedStream = new CryptoStream(dataStream, encryptor, CryptoStreamMode.Write);

                    try
                    {
                        //Write the string to memory via the encryption algorithm							
                        StreamWriter theWriter = new StreamWriter(encryptedStream);
                        try
                        {
                            //Write the string to the memory stream
                            theWriter.Write(AString);

                            //End the writing
                            theWriter.Flush();
                            encryptedStream.FlushFinalBlock();

                            //Position back at start
                            dataStream.Position = 0;

                            //Create area for data
                            byte[] encryptedData = new byte[dataStream.Length + 1];

                            //Read data from memory
                            dataStream.Read(encryptedData, 0, (int)dataStream.Length);

                            //Convert to String
                            return ToBase64(encryptedData); // Convert.ToBase64String(encryptedData);
                        }
                        finally
                        {
                            theWriter.Close();
                        }
                    }
                    finally
                    {
                        encryptedStream.Close();
                    }
                }
                finally
                {
                    dataStream.Close();
                }
            }
        }

        //private byte[] key = { };
        //private byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef };
        //public string Decrypt(string stringToDecrypt, string sEncryptionKey)
        //{
        //    byte[] inputByteArray = new byte[stringToDecrypt.Length + 1];
        //    try
        //    {
        //        key = System.Text.Encoding.UTF8.GetBytes(sEncryptionKey);
        //        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        //        inputByteArray = Convert.FromBase64String(stringToDecrypt);
        //        MemoryStream ms = new MemoryStream();
        //        CryptoStream cs = new CryptoStream(ms,
        //          des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
        //        cs.Write(inputByteArray, 0, inputByteArray.Length);
        //        cs.FlushFinalBlock();
        //        System.Text.Encoding encoding = System.Text.Encoding.UTF8;
        //        return encoding.GetString(ms.ToArray());
        //    }
        //    catch (Exception e)
        //    {
        //        return e.Message;
        //    }
        //}

        //public string Encrypt(string stringToEncrypt, string SEncryptionKey)
        //{
        //    try
        //    {
        //        key = System.Text.Encoding.UTF8.GetBytes(SEncryptionKey);
        //        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        //        byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
        //        MemoryStream ms = new MemoryStream();
        //        CryptoStream cs = new CryptoStream(ms,
        //          des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
        //        cs.Write(inputByteArray, 0, inputByteArray.Length);
        //        cs.FlushFinalBlock();
        //        return Convert.ToBase64String(ms.ToArray());
        //    }
        //    catch (Exception e)
        //    {
        //        return e.Message;
        //    }
        //} 

    }
}
