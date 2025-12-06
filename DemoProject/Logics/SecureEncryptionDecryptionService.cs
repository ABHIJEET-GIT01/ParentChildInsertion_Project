using System.Security.Cryptography;

namespace DemoProject.Logics
{
    public class SecureEncryptionDecryptionService
    {
        private byte[] key;
        private byte[] iv;

        public SecureEncryptionDecryptionService()
        {
            // Generate a random key and IV for each instance of the service
            GenerateKeyAndIV();
        }

        private void GenerateKeyAndIV()
        {
            using (Aes aesAlg = Aes.Create())
            {
                // Generate a secure random key
                aesAlg.GenerateKey();
                key = aesAlg.Key;

                // Generate a secure random IV
                aesAlg.GenerateIV();
                iv = aesAlg.IV;
            }
            // Generate a random key
           // using (var rng = new RNGCryptoServiceProvider())
           // {
           //     key = new byte[32]; // 256 bits
           //     rng.GetBytes(key);
           // }
           //
           // // Generate a random IV
           // using (var rng = new RNGCryptoServiceProvider())
           // {
           //     iv = new byte[16]; // 128 bits
           //     rng.GetBytes(iv);
           // }
        }

        public string Encrypt(string plaintext)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plaintext);
                        }
                    }

                    return Convert.ToBase64String(iv.Concat(msEncrypt.ToArray()).ToArray());
                }
            }
        }

        public string Decrypt(string encryptedText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;

                // Extract IV from the first 16 bytes of the base64 string
                byte[] extractedIV = Convert.FromBase64String(encryptedText).Take(16).ToArray();
                aesAlg.IV = extractedIV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText).Skip(16).ToArray()))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}

