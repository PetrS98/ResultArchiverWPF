using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace WPFUtilsLib.Helpers
{
    public class FileEncryptionManager
    {
        public static void EncryptFile(string filePath, string key)
        {
            byte[] plainContent = File.ReadAllBytes(filePath);
            using (var des = DES.Create())
            {
                des.IV = Encoding.UTF8.GetBytes(key);
                des.Key = Encoding.UTF8.GetBytes(key);
                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.PKCS7;


                using (var memStream = new MemoryStream())
                {
                    CryptoStream cryptoStream = new(memStream, des.CreateEncryptor(),
                        CryptoStreamMode.Write);

                    cryptoStream.Write(plainContent, 0, plainContent.Length);
                    cryptoStream.FlushFinalBlock();
                    File.WriteAllBytes(filePath, memStream.ToArray());
                }
            }
        }

        public static void DecryptFile(string filePath, string key)
        {
            byte[] encrypted = File.ReadAllBytes(filePath);
            using (var des = DES.Create())
            {
                des.IV = Encoding.UTF8.GetBytes(key);
                des.Key = Encoding.UTF8.GetBytes(key);
                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.PKCS7;


                using (var memStream = new MemoryStream())
                {
                    CryptoStream cryptoStream = new(memStream, des.CreateDecryptor(),
                        CryptoStreamMode.Write);

                    cryptoStream.Write(encrypted, 0, encrypted.Length);
                    cryptoStream.FlushFinalBlock();
                    File.WriteAllBytes(filePath, memStream.ToArray());
                }
            }
        }
    }
}
