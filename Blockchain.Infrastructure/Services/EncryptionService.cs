using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain.Infrastructure.Services
{
    public class EncryptionService
    {
        private readonly string _key;

        public EncryptionService(IConfiguration config)
            => _key = config["BlockchainSettings:WalletEncryptionKey"]!;

        public string Encrypt(string plainText)
        {
            using var aes = System.Security.Cryptography.Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_key.PadRight(32)[..32]);
            aes.GenerateIV();
            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            var result = new byte[aes.IV.Length + cipherBytes.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(cipherBytes, 0, result, aes.IV.Length, cipherBytes.Length);
            return Convert.ToBase64String(result);
        }

        public string Decrypt(string cipherText)
        {
            var allBytes = Convert.FromBase64String(cipherText);
            using var aes = System.Security.Cryptography.Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_key.PadRight(32)[..32]);
            var iv = new byte[aes.BlockSize / 8];
            var cipher = new byte[allBytes.Length - iv.Length];
            Buffer.BlockCopy(allBytes, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(allBytes, iv.Length, cipher, 0, cipher.Length);
            aes.IV = iv;
            using var decryptor = aes.CreateDecryptor();
            var plainBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}
