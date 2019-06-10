using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MusicIdentifierAPI.Utils
{
    public class StringCipher
    {
        public static string Encrypt(string clearText, string encryptionKey)
        {
            var clearBytes = Encoding.Unicode.GetBytes(clearText);
            using var encryption = Aes.Create();
            var pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            if (encryption == null) return clearText;
            encryption.Key = pdb.GetBytes(32);
            encryption.IV = pdb.GetBytes(16);
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryption.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(clearBytes, 0, clearBytes.Length);
            cs.Close();
            clearText = Convert.ToBase64String(ms.ToArray());

            return clearText;
        }
        public static string Decrypt(string cipherText, string encryptionKey)
        {
            cipherText = cipherText.Replace(" ", "+");
            var cipherBytes = Convert.FromBase64String(cipherText);
            using var encryption = Aes.Create();
            var pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            if (encryption == null) return cipherText;
            encryption.Key = pdb.GetBytes(32);
            encryption.IV = pdb.GetBytes(16);
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryption.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherBytes, 0, cipherBytes.Length);
            cs.Close();
            cipherText = Encoding.Unicode.GetString(ms.ToArray());
            return cipherText;
        }
    }
}