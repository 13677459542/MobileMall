using System.Security.Cryptography;
using System.Text;

namespace MobileMallWebApi.Utils.Encryption
{
    /// <summary>
    /// //RSA加解密 使用OpenSSL的公钥加密/私钥解密
    /// RSA2密钥对获取（通过支付宝开放平台密钥工具）：1、下载支付宝开放平台密钥工具，生成密钥后获得公钥私钥。2、将私钥在格式转换中转换成PKCS1格式，获得转换后的私钥。地址：https://opendocs.alipay.com/mini/02c7i5
    /// RSA2密钥对获取（通过其他渠道生成）：1、生成密钥后获得公钥私钥。2、将私钥在支付宝开放平台密钥工具格式转换中转换成PKCS1格式，获得转换后的私钥。
    /// 在线加密测试地址：https://www.json.cn/encrypt/rsa/#google_vignette
    /// </summary>
    public class RSAHelper
    {
        private readonly RSA _privateKeyRsaProvider;
        private readonly RSA _publicKeyRsaProvider;
        private readonly HashAlgorithmName _hashAlgorithmName;
        private readonly Encoding _encoding;

        /// <summary>
        /// 2048 公钥
        /// </summary>
        private readonly string defaultpublicKey = @"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA3Pg+4SnPGW71Fm3tTxzzb0n1b41Pr1GuXvO4rA1VOX+SrVz+K8g98qyOF6RM2iOdaJBWat6IVjTuTLwVw+m2Q5aHlHSxkCX/RIBJNt9qIaOytbnn711ssecN00W+NFuiotLLeVP+ttSb28HGCXaGvNe1PFswDh0ZJMcMGY8JvC+h135nQNCQ0hG06vj0KzrmeIxRRNX1AQFIQz+7/hy/oJ/o0XJXrgGdH2e5e3Kox260G9QRruQZ4HGMrQ7VvaqeJ1v14qHLVI0ij3ycPeYJQ+tjXfjcheUppaTSuT2pdQRTY97nkPY//e3zXs4MKzlL3GAY+a3CfqjWRSjiOqTSywIDAQAB";
        /// <summary>
        ///  //2048 私钥
        /// </summary>
        private readonly string defaultprivateKey = @"MIIEowIBAAKCAQEA3Pg+4SnPGW71Fm3tTxzzb0n1b41Pr1GuXvO4rA1VOX+SrVz+K8g98qyOF6RM2iOdaJBWat6IVjTuTLwVw+m2Q5aHlHSxkCX/RIBJNt9qIaOytbnn711ssecN00W+NFuiotLLeVP+ttSb28HGCXaGvNe1PFswDh0ZJMcMGY8JvC+h135nQNCQ0hG06vj0KzrmeIxRRNX1AQFIQz+7/hy/oJ/o0XJXrgGdH2e5e3Kox260G9QRruQZ4HGMrQ7VvaqeJ1v14qHLVI0ij3ycPeYJQ+tjXfjcheUppaTSuT2pdQRTY97nkPY//e3zXs4MKzlL3GAY+a3CfqjWRSjiOqTSywIDAQABAoIBABGePsU3ztS5or5LEs9qq4OFY2r62rj2dkSz11brKTm7EOLUYJ+fs5tpuVqWiwTJhNTAIrkaonGH1DLiEYoxVDWcsZVbSIe0aon3qzQTfs7NJ9k9crSvFOo1AJvbxQfqVn6iTVQ7J4/u8Q5bK4MNpD3iT7JO7aHycqgasWhISKpsVo7gWRolMf6VXmFOkDFHh8B2Pkcb8yq0duhSvfsVcy1MJq/9dkRqRTiCCZmNZER5ceq/q6xdnEcfjawnZp08lYSVtTe0A2FclYwqPyvmoNDgpCu2nxgxhPFGq3cmzjsKgM5VMoxb51gniG+lQMDmY48OOlCpjw2eRf2UTfBNfiECgYEA71WwnTJ9zlaocx4yCcUYpUId/Z+V3TjAHmqkkDIBuBYdwzsmYoTuTpHDx3NA/NJRegpH720VA0WfeHhRfdqfiGCSbJ3qD7YIB2NCXopadBoqx9FvqVSnABxtfixCmFWipzsFw0k2wOpeJNDk6YcpQyBEu4S3Maj2Bhuk/s7t8ScCgYEA7FsvtY2XFJqJN84aQmV+gVwntZA86UXjaV3ZYErszQIERUMAudIavSOoai5C5KGtq0NxCW7A4bkCDVyIsCwMUmd0M6Be35wriWai2vVu3u/OE/yfqgHajf8DGd3vXyh9o12H5v8envN6Wh/CUGRZf/GEy1GhM+u2bNA8tcuzj70CgYEAnSQwICaEv7PaSitrQ0rr0aXFtz7O0T9vtQjkH+EVi97Jj+QIYetR5LiESTJ9WwJkiLKzZJrEjy9pc1ncd7vRv2NZAIP2qHYmc2NSsmw4075SlHwIyq9QLxx7L7qzxv2DHDX+pKgvkR7QzW9yvXoHN5G6TzzmY27CimQgQ0VuqUUCgYBMv7t5R9X0Uc4W+e0a/Fwc43Ddi03MLe6Pi3MHyqykUXBTkVNOA8S9ADQy7ny4Qyvivg6ZkoY9hdb9wbt9AYCqzX81OHE2ST716gcd9K6g49vWL6UlDl8K1vEJ2EBfdQV/I+L6hoNJ+CQV2dQ+SKerXSDS6NngwzzEjsX3/oJ7PQKBgE/gciY4CuViwM5UeRxZ+kLYPeNnJOnrCj/TthXFkmPxpa/KD9MaBZo8znHDcxRZX1Y/e7Q3yvUYwejOE9jYehieoVEa47WNbnQHFAJCRY6gnnBwro5eLwn4SSGY8+Wu2Dol2lfmAHhVlk0+Zugc3rvPpkF9sKEDzspb94AEtpam";

        /// <summary>
        /// 实例化RSAHelper
        /// </summary>
        /// <param name="rsaType">加密算法类型 RSA SHA1;RSA2 SHA256 密钥长度至少为2048</param>
        /// <param name="encoding">编码类型</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="publicKey">公钥</param>
        public RSAHelper(RSAType rsaType, Encoding encoding, string privateKey = "", string publicKey = "")
        {
            _encoding = encoding;
            if (!string.IsNullOrEmpty(privateKey))
                _privateKeyRsaProvider = CreateRsaProviderFromPrivateKey(privateKey);
            else
                _privateKeyRsaProvider = CreateRsaProviderFromPrivateKey(defaultprivateKey);

            if (!string.IsNullOrEmpty(publicKey))
                _publicKeyRsaProvider = CreateRsaProviderFromPublicKey(publicKey);
            else
                _publicKeyRsaProvider = CreateRsaProviderFromPublicKey(defaultpublicKey);

            _hashAlgorithmName = rsaType == RSAType.RSA ? HashAlgorithmName.SHA1 : HashAlgorithmName.SHA256;
        }

        #region 使用私钥签名

        /// <summary>
        /// 使用私钥签名
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <returns></returns>
        public string Sign(string data)
        {
            byte[] dataBytes = _encoding.GetBytes(data);

            var signatureBytes = _privateKeyRsaProvider.SignData(dataBytes, _hashAlgorithmName, RSASignaturePadding.Pkcs1);

            return Convert.ToBase64String(signatureBytes);
        }

        #endregion

        #region 使用公钥验证签名

        /// <summary>
        /// 使用公钥验证签名
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <param name="sign">签名</param>
        /// <returns></returns>
        public bool Verify(string data, string sign)
        {
            byte[] dataBytes = _encoding.GetBytes(data);
            byte[] signBytes = Convert.FromBase64String(sign);

            var verify = _publicKeyRsaProvider.VerifyData(dataBytes, signBytes, _hashAlgorithmName, RSASignaturePadding.Pkcs1);

            return verify;
        }

        #endregion

        #region 解密

        public string Decrypt(string cipherText)
        {
            if (_privateKeyRsaProvider == null)
            {
                throw new Exception("_privateKeyRsaProvider is null");
            }
            return Encoding.UTF8.GetString(_privateKeyRsaProvider.Decrypt(Convert.FromBase64String(cipherText), RSAEncryptionPadding.Pkcs1));
        }

        #endregion

        #region 加密

        public string Encrypt(string text)
        {
            if (_publicKeyRsaProvider == null)
            {
                throw new Exception("_publicKeyRsaProvider is null");
            }
            return Convert.ToBase64String(_publicKeyRsaProvider.Encrypt(Encoding.UTF8.GetBytes(text), RSAEncryptionPadding.Pkcs1));
        }

        #endregion

        #region 使用私钥创建RSA实例

        public RSA CreateRsaProviderFromPrivateKey(string privateKey)
        {
            var privateKeyBits = Convert.FromBase64String(privateKey);

            var rsa = RSA.Create();
            var rsaParameters = new RSAParameters();

            using (BinaryReader binr = new BinaryReader(new MemoryStream(privateKeyBits)))
            {
                byte bt = 0;
                ushort twobytes = 0;
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    throw new Exception("Unexpected value read binr.ReadUInt16()");

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)
                    throw new Exception("Unexpected version");

                bt = binr.ReadByte();
                if (bt != 0x00)
                    throw new Exception("Unexpected value read binr.ReadByte()");

                rsaParameters.Modulus = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.Exponent = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.D = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.P = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.Q = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.DP = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.DQ = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.InverseQ = binr.ReadBytes(GetIntegerSize(binr));
            }

            rsa.ImportParameters(rsaParameters);
            return rsa;
        }

        #endregion

        #region 使用公钥创建RSA实例

        public RSA CreateRsaProviderFromPublicKey(string publicKeyString)
        {
            // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
            byte[] seqOid = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];

            var x509Key = Convert.FromBase64String(publicKeyString);

            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            using (MemoryStream mem = new MemoryStream(x509Key))
            {
                using (BinaryReader binr = new BinaryReader(mem))  //wrap Memory Stream with BinaryReader for easy reading
                {
                    byte bt = 0;
                    ushort twobytes = 0;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    seq = binr.ReadBytes(15);       //read the Sequence OID
                    if (!CompareBytearrays(seq, seqOid))    //make sure Sequence for OID is correct
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8203)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    bt = binr.ReadByte();
                    if (bt != 0x00)     //expect null byte next
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    twobytes = binr.ReadUInt16();
                    byte lowbyte = 0x00;
                    byte highbyte = 0x00;

                    if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
                        lowbyte = binr.ReadByte();  // read next bytes which is bytes in modulus
                    else if (twobytes == 0x8202)
                    {
                        highbyte = binr.ReadByte(); //advance 2 bytes
                        lowbyte = binr.ReadByte();
                    }
                    else
                        return null;
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
                    int modsize = BitConverter.ToInt32(modint, 0);

                    int firstbyte = binr.PeekChar();
                    if (firstbyte == 0x00)
                    {   //if first byte (highest order) of modulus is zero, don't include it
                        binr.ReadByte();    //skip this null byte
                        modsize -= 1;   //reduce modulus buffer size by 1
                    }

                    byte[] modulus = binr.ReadBytes(modsize);   //read the modulus bytes

                    if (binr.ReadByte() != 0x02)            //expect an Integer for the exponent data
                        return null;
                    int expbytes = (int)binr.ReadByte();        // should only need one byte for actual exponent data (for all useful values)
                    byte[] exponent = binr.ReadBytes(expbytes);

                    // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                    var rsa = RSA.Create();
                    RSAParameters rsaKeyInfo = new RSAParameters
                    {
                        Modulus = modulus,
                        Exponent = exponent
                    };
                    rsa.ImportParameters(rsaKeyInfo);

                    return rsa;
                }

            }
        }

        #endregion

        #region 导入密钥算法

        private int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();
            else
            if (bt == 0x82)
            {
                var highbyte = binr.ReadByte();
                var lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;
            }

            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }

        private bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

        #endregion

    }

    /// <summary>
    /// RSA算法类型
    /// </summary>
    public enum RSAType
    {
        /// <summary>
        /// SHA1
        /// </summary>
        RSA = 0,
        /// <summary>
        /// RSA2 密钥长度至少为2048
        /// SHA256
        /// </summary>
        RSA2
    }
}
