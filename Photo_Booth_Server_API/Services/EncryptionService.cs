using System.Security.Cryptography;

namespace Photo_Booth_Server_API.Services
{
    public class EncryptionService
    {
        public byte[] Encrypt(byte[] data, string password, out byte[] salt, out byte[] nonce, out byte[] tag)
        {
            salt = RandomNumberGenerator.GetBytes(16);
            nonce = RandomNumberGenerator.GetBytes(12);
            tag = new byte[16];

            using var deriveBytes = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);

            var key = deriveBytes.GetBytes(32);
            var encrypted = new byte[data.Length];
            using var aes = new AesGcm(key, 16);

            aes.Encrypt(nonce, data, encrypted, tag);
            return encrypted;
             
        }    

        public byte[] Decrypt(byte[] encryptedData, string password, byte[] salt, byte[] nonce, byte[] tag)
        {
            using var deriveBytes = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            var key = deriveBytes.GetBytes(32);
            var decrypted = new byte[encryptedData.Length];
            using var aes = new AesGcm(key, 16);
            aes.Decrypt(nonce, encryptedData, tag, decrypted);
            return decrypted;
        }
    
    }
}
