using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Flexinets.Radius
{
    public static class RadiusPassword
    {
        /// <summary>
        /// Encrypt/decrypt using XOR
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static Byte[] EncryptDecrypt(Byte[] input, Byte[] key)
        {
            var output = new Byte[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                output[i] = (Byte)(input[i] ^ key[i]);
            }
            return output;
        }


        /// <summary>
        /// Create a radius shared secret key
        /// </summary>
        /// <param name="sharedsecret"></param>
        /// <param name="Stuff"></param>
        /// <returns></returns>
        private static Byte[] CreateKey(Byte[] sharedsecret, Byte[] authenticator)
        {
            var key = new Byte[16 + sharedsecret.Length];
            Buffer.BlockCopy(sharedsecret, 0, key, 0, sharedsecret.Length);
            Buffer.BlockCopy(authenticator, 0, key, sharedsecret.Length, authenticator.Length);

            using (var md5 = MD5CryptoServiceProvider.Create())
            {
                return md5.ComputeHash(key);
            }
        }


        /// <summary>
        /// Decrypt user password
        /// </summary>
        /// <param name="_radiusSharedSecret"></param>
        /// <param name="authenticator"></param>
        /// <param name="passwordBytes"></param>
        /// <returns></returns>
        public static String Decrypt(Byte[] radiusSharedSecret, Byte[] authenticator, Byte[] passwordBytes)
        {
            var sb = new StringBuilder();
            var key = CreateKey(radiusSharedSecret, authenticator);

            for (Byte n = 1; n <= passwordBytes.Length / 16; n++)
            {
                var temp = new Byte[16];
                Buffer.BlockCopy(passwordBytes, (n - 1) * 16, temp, 0, 16);
                sb.Append(Encoding.UTF8.GetString(EncryptDecrypt(temp, key)));
                key = CreateKey(radiusSharedSecret, temp);
            }

            return sb.ToString().Replace("\0", "");
        }
    }
}
