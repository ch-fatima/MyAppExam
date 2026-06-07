using Core.Extensions;
using Core.Models;
using System;
using System.Security.Cryptography;
using System.Text;
using static Dapper.SqlMapper;

namespace Infrastructure.Cryptography
{
    public class AesGcmService : ICryptographyService
    {
        private readonly byte[] _key;
        private const int NonceSize = 12; 
        private const int TagSize = 16;   
        private readonly InitSetting _setting;

        public AesGcmService(InitSetting setting)
        {
            _setting= setting;  
            var keyString = _setting.Cryptography.EncryptionKey;
            if (string.IsNullOrEmpty(keyString) || keyString.Length != 64)
                throw new Exception("Encryption Key must be a 64-character Hex string (256-bit).");

            _key = SecurityHelper.HexStringToByteArray(keyString);
            _setting = setting;
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] nonce = new byte[NonceSize];
            RandomNumberGenerator.Fill(nonce); 

            byte[] tag = new byte[TagSize];
            byte[] cipherText = new byte[plainBytes.Length];

            using (var aesGcm = new AesGcm(_key, TagSize))
            {
                aesGcm.Encrypt(nonce, plainBytes, cipherText, tag);
            }

            byte[] result = new byte[NonceSize + TagSize + cipherText.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
            Buffer.BlockCopy(tag, 0, result, NonceSize, TagSize);
            Buffer.BlockCopy(cipherText, 0, result, NonceSize + TagSize, cipherText.Length);

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string base64CipherText)
        {
            if (string.IsNullOrEmpty(base64CipherText)) return base64CipherText;

            byte[] fullData = Convert.FromBase64String(base64CipherText);

            byte[] nonce = fullData[..NonceSize];
            byte[] tag = fullData[NonceSize..(NonceSize + TagSize)];
            byte[] cipherText = fullData[(NonceSize + TagSize)..];

            byte[] decryptedBytes = new byte[cipherText.Length];

            using (var aesGcm = new AesGcm(_key, TagSize))
            {
                aesGcm.Decrypt(nonce, cipherText, tag, decryptedBytes);
            }

            return Encoding.UTF8.GetString(decryptedBytes);
        }

    }
}
