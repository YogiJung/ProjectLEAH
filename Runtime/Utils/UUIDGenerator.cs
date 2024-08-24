using System;
using System.Security.Cryptography;

namespace ProjectLeah.Runtime.Utils
{
    public static class UUIDGenerator
    {
        public static string GenerateRandomId()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] data = new byte[16];
                rng.GetBytes(data);
                return BitConverter.ToString(data).Replace("-", "").ToLower();
            }
        }
    }
}