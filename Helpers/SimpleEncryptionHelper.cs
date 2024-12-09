using System.Security.Cryptography;
using System.Text;

namespace PasswordManagerApplication.Helpers
{
    public class SimpleEncryptionHelper
    {
        // Static Key and IV (For demo purposes only; don't hard-code these in production)
        private static readonly string key = "1234567890123456";  // 16-byte key (128-bit)
        private static readonly string iv = "1234567890123456";   // 16-byte IV (128-bit)

        // Encrypt method
        public static string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);  // Set the key
                aesAlg.IV = Encoding.UTF8.GetBytes(iv);    // Set the IV

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText); // Write the plain text to encrypt
                        }
                    }

                    // Return the encrypted data as Base64 string
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        // Decrypt method
        public static string Decrypt(string cipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);  // Set the key
                aesAlg.IV = Encoding.UTF8.GetBytes(iv);    // Set the IV

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();  // Return the decrypted text
                        }
                    }
                }
            }
        }
    }
}