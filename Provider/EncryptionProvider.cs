using System.Security.Cryptography;
using System.Text;

namespace Hrms.Provider
{
    public static class EncryptionProvider
    {
        public static string EncryptionKey => "b14ca5898a4e4133bbce2ea2315a1916";
        public static string Encrypt(string clearText)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                encryptor.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                encryptor.IV = new byte[16];
                using MemoryStream memoryStream = new();
                using CryptoStream cryptoStream = new(memoryStream, encryptor.CreateEncryptor(), CryptoStreamMode.Write);
                cryptoStream.Write(clearBytes, 0, clearBytes.Length); cryptoStream.Close();
                clearText = Convert.ToBase64String(memoryStream.ToArray());
            }
            return clearText;
        }
        public static string Decrypt(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                encryptor.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                encryptor.IV = new byte[16];
                using MemoryStream memoryStream = new();
                CryptoStream cryptoStream = new(memoryStream, encryptor.CreateDecryptor(), CryptoStreamMode.Write);
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length); cryptoStream.Close();
                cipherText = Encoding.Unicode.GetString(memoryStream.ToArray());
            }
            return cipherText;
        }
    }
}
