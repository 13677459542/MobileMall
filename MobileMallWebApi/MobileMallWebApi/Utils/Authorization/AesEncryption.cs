using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace MobileMallWebApi.Utils.Authorization
{
    /// <summary>
    /// AES对称加密工具类
    /// </summary>
    public class AesEncryption
    {
        /// <summary>
        /// 密钥(前端请求时需要使用非对称加密传输，避免密钥暴露)
        /// </summary>
        public static byte[] key = Encoding.UTF8.GetBytes("12345678123456781234567812345678");  //32位，自己可以定义
        /// <summary>
        /// 初始化向量(随机或伪随机生成的值)
        /// </summary>
        public static byte[] iv = Encoding.UTF8.GetBytes("1234567812345678"); //16位，自己可以定义

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="Key"></param>
        /// <param name="IV"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException(nameof(Key));
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException(nameof(IV));

            // 用于存储加密后的字节数组
            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // 返回 Base64 编码的字符串
            return Convert.ToBase64String(encrypted);
        }


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="Key"></param>
        /// <param name="IV"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string DecryptStringFromBytes_Aes(string cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException(nameof(Key));
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException(nameof(IV));

            string plaintext = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // 将 Base64 编码的字符串转换为字节数组
                byte[] byteCipherText = Convert.FromBase64String(cipherText);
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(byteCipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            //// 解析 JSON 字符串，去掉多余的反斜杠
            //plaintext = plaintext.Replace("\\\"", "\"");
            return plaintext;
        }
    }
}
