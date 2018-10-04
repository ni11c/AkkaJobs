using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Agridea.Security
{
    public static class CryptographyHelper
    {
        public static void AddSaltToPlainText(ref byte[] salt, ref byte[] plainText)
        {
            if (salt == null)
            {
                var bytes = new byte[16];
                RandomNumberGenerator.Create().GetBytes(bytes);
                salt = bytes;
            }
            plainText = CombineBytes(salt, plainText);
        }

        public static byte[] CombineBytes(byte[] buffer1, byte[] buffer2)
        {
            var numArray = new byte[buffer1.Length + buffer2.Length];
            Buffer.BlockCopy(buffer1, 0, numArray, 0, buffer1.Length);
            Buffer.BlockCopy(buffer2, 0, numArray, buffer1.Length, buffer2.Length);
            return numArray;

        }

        public static bool CompareBytes(byte[] byte1, byte[] byte2)
        {
            if (byte1 == null || byte2 == null || byte1.Length != byte2.Length)
                return false;

            return !byte1.Where((t, index) => t != byte2[index]).Any();
        }

        public static byte[] CreateHashWithSalt(byte[] plainText, byte[] salt)
        {
            AddSaltToPlainText(ref salt, ref plainText);
            byte[] hash;
            using (var crypto = new SHA256Managed())
            {
                hash = crypto.ComputeHash(plainText);
            }
            hash = CombineBytes(salt, hash);
            return hash;
        }

        public static bool CompareHash(string password, string hashed)
        {

            byte[] bytes = Encoding.Unicode.GetBytes(password);
            byte[] hashedtext = Convert.FromBase64String(hashed);
            byte[] numArray = null;

            if (hashedtext.Length > 16)
            {
                numArray = new byte[16];
                Buffer.BlockCopy(hashedtext, 0, numArray, 0, 16);
            }
            var hashWithSalt = CreateHashWithSalt(bytes, numArray);

            return CompareBytes(hashWithSalt, hashedtext);
        }
    }
}
